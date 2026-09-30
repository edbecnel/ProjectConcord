namespace Edf.Application.Relay.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Edf.Domain.Relay;

public sealed class GovernedRelayV1Importer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    private static readonly Regex MachineBlockRegex = new(
        $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\r?\\n([\\s\\S]*?)\\r?\\n```",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public GovernedRelayImportAttempt Import(string renderedText)
    {
        var accumulator = new RelayValidationAccumulator();

        if (string.IsNullOrWhiteSpace(renderedText))
        {
            accumulator.AddMalformed(
                RelayValidationCodes.MachineBlockMissing,
                "Rendered relay text is empty.");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        if (!TryParseRenderVersion(renderedText, out var renderMajor, out var renderError))
        {
            accumulator.AddMalformed(
                renderError!.Contains("missing", StringComparison.OrdinalIgnoreCase)
                    ? RelayValidationCodes.RenderVersionMissing
                    : RelayValidationCodes.RenderVersionUnsupported,
                renderError!);
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        var renderVersion = new RelayRenderVersion(renderMajor, 0);
        if (renderMajor != RelayRenderVersion.V1.Major)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.RenderVersionUnsupported,
                $"Render version major {renderMajor} is not supported.");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        var machineMatch = MachineBlockRegex.Match(renderedText);
        if (!machineMatch.Success)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.MachineBlockMissing,
                $"Required machine block fenced with {GovernedRelayV1Format.MachineBlockFenceLanguage} is missing.");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        var machineJson = machineMatch.Groups[1].Value.Trim();
        GovernedRelayEnvelopeV1Dto? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<GovernedRelayEnvelopeV1Dto>(machineJson, JsonOptions);
        }
        catch (JsonException ex)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.MachineBlockInvalidJson,
                $"Machine relay block JSON is invalid: {ex.Message}");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        if (envelope is null)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.MachineBlockInvalidJson,
                "Machine relay block JSON deserialized to null.");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        var schema = new RelaySchemaVersion(envelope.SchemaVersionMajor, envelope.SchemaVersionMinor);
        if (schema.Major != RelaySchemaVersion.Current.Major
            || schema.Minor > RelaySchemaVersion.Current.Minor)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.SchemaVersionUnsupported,
                $"Schema version {schema} is not supported.");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        if (!GovernedRelayGovernanceProjections.TryParse(renderedText, out var projections, out var projectionError))
        {
            accumulator.AddMalformed(
                RelayValidationCodes.GovernanceProjectionMismatch,
                projectionError ?? "Human governance projections could not be parsed.");
            return GovernedRelayImportAttempt.Failed(accumulator.ToResult());
        }

        var projectionsAgree = GovernedRelayGovernanceProjections.ProjectionsAgree(envelope, projections!);
        var structuralAgreement = new GovernedRelayStructuralAgreement(
            RequiresMachineBlock: true,
            MachineBlockPresent: true,
            RequiresGovernanceProjectionAgreement: true,
            GovernanceProjectionsAgree: projectionsAgree);

        if (!projectionsAgree)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.GovernanceProjectionMismatch,
                "Machine block and projected governance-critical fields disagree.");
        }

        var package = GovernedRelayEnvelopeV1Mapper.ToPackage(envelope, renderVersion, structuralAgreement);
        return new GovernedRelayImportAttempt(package, accumulator.ToResult());
    }

    private static bool TryParseRenderVersion(string text, out int major, out string? error)
    {
        major = 0;
        error = null;
        foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!line.StartsWith(GovernedRelayV1Format.RenderVersionLinePrefix, StringComparison.Ordinal))
            {
                continue;
            }

            var versionText = line[GovernedRelayV1Format.RenderVersionLinePrefix.Length..].Trim();
            if (!int.TryParse(versionText.Split('.')[0], out major))
            {
                error = $"Render version '{versionText}' is not valid.";
                return false;
            }

            return true;
        }

        error = "Render version marker is missing.";
        return false;
    }
}

public sealed record GovernedRelayImportAttempt(
    GovernedRelayPackage? Package,
    RelayValidationResult StructuralResult)
{
    public static GovernedRelayImportAttempt Failed(RelayValidationResult result) =>
        new(null, result);
}
