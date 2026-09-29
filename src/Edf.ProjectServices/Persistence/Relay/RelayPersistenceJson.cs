using System.Text.Json;
using System.Text.Json.Serialization;
using Edf.Domain.Relay;

namespace Edf.ProjectServices.Persistence.Relay;

internal static class RelayPersistenceJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        },
    };

    public static string SerializeAdvisory(AgentSessionAdvisory advisory) =>
        JsonSerializer.Serialize(advisory, Options);

    public static AgentSessionAdvisory DeserializeAdvisory(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return AgentSessionAdvisory.None;
        }

        return JsonSerializer.Deserialize<AgentSessionAdvisory>(json, Options);
    }

    public static string SerializeGovernanceCritical(RelayGovernanceCriticalState state) =>
        JsonSerializer.Serialize(state, Options);

    public static RelayGovernanceCriticalState DeserializeGovernanceCritical(string json) =>
        JsonSerializer.Deserialize<RelayGovernanceCriticalState>(json, Options)
        ?? throw new InvalidOperationException("Governance-critical JSON was empty.");

    public static string? SerializeTier0(Tier0RelaySnapshot? snapshot) =>
        snapshot is null ? null : JsonSerializer.Serialize(snapshot, Options);

    public static Tier0RelaySnapshot? DeserializeTier0(string? json) =>
        string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<Tier0RelaySnapshot>(json, Options);

    public static string SerializeStructuralAgreement(GovernedRelayStructuralAgreement agreement) =>
        JsonSerializer.Serialize(agreement, Options);

    public static GovernedRelayStructuralAgreement DeserializeStructuralAgreement(string json) =>
        JsonSerializer.Deserialize<GovernedRelayStructuralAgreement>(json, Options)
        ?? GovernedRelayPackage.DefaultStructuralAgreement;

    public static string SerializeDiagnostics(IReadOnlyList<RelayValidationDiagnostic> diagnostics) =>
        JsonSerializer.Serialize(diagnostics, Options);

    public static IReadOnlyList<RelayValidationDiagnostic> DeserializeDiagnostics(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<RelayValidationDiagnostic>();
        }

        var deserialized = JsonSerializer.Deserialize<List<RelayValidationDiagnostic>>(json, Options);
        return deserialized is null ? Array.Empty<RelayValidationDiagnostic>() : deserialized;
    }
}
