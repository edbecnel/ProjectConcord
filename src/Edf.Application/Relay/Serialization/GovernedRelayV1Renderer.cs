namespace Edf.Application.Relay.Serialization;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Edf.Domain.Relay;

public sealed class GovernedRelayV1Renderer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public string Render(GovernedRelayPackage package)
    {
        ArgumentNullException.ThrowIfNull(package);

        var renderVersion = package.RenderVersion ?? RelayRenderVersion.V1;
        var envelope = GovernedRelayEnvelopeV1Mapper.ToDto(package);
        var machineJson = JsonSerializer.Serialize(envelope, JsonOptions);

        var builder = new StringBuilder();
        builder.AppendLine($"{GovernedRelayV1Format.RenderVersionLinePrefix} {renderVersion.Major}");
        builder.AppendLine();
        builder.AppendLine($"```{GovernedRelayV1Format.MachineBlockFenceLanguage}");
        builder.AppendLine(machineJson.TrimEnd());
        builder.AppendLine("```");
        builder.AppendLine();
        builder.Append(GovernedRelayGovernanceProjections.Render(envelope));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(GovernedRelayV1Format.PaCursorReminder);

        return builder.ToString().TrimEnd() + Environment.NewLine;
    }
}
