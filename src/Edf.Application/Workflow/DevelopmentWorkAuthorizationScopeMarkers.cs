using System.Text.Json;

namespace Edf.Application.Workflow;

public static class DevelopmentWorkAuthorizationScopeMarkers
{
    public static string? Serialize(IReadOnlyList<string>? markers)
    {
        if (markers is null || markers.Count == 0)
        {
            return null;
        }

        return JsonSerializer.Serialize(markers);
    }

    public static IReadOnlyList<string> Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Array.Empty<string>();
        }

        return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
    }
}
