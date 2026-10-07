namespace Edf.Application.Relay;

using System.Text.Json;
using Edf.Domain.Relay;

/// <summary>
/// Durable provenance payload for <see cref="RelayProvenanceEventType.PackageProduced"/> on PA review exports.
/// </summary>
public static class RelayPaReviewExportProvenance
{
    public const string ProfilePropertyName = "paHandoverResponseProfile";

    public static string CreatePayload(PaHandoverResponseProfile responseProfile) =>
        JsonSerializer.Serialize(new Dictionary<string, string>
        {
            [ProfilePropertyName] = responseProfile.ToString(),
        });

    public static PaHandoverResponseProfile? TryReadProfile(RelayProvenanceEvent provenanceEvent)
    {
        if (provenanceEvent.EventType != RelayProvenanceEventType.PackageProduced)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(provenanceEvent.PayloadJson)
            || provenanceEvent.PayloadJson == "{}")
        {
            return PaHandoverResponseProfile.PlanningEntry;
        }

        try
        {
            using var doc = JsonDocument.Parse(provenanceEvent.PayloadJson);
            if (!doc.RootElement.TryGetProperty(ProfilePropertyName, out var prop))
            {
                return PaHandoverResponseProfile.PlanningEntry;
            }

            var text = prop.GetString();
            return Enum.TryParse<PaHandoverResponseProfile>(text, ignoreCase: false, out var profile)
                ? profile
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
