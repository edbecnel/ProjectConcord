namespace Edf.ProjectServices.Relay;

using System.Text.RegularExpressions;

/// <summary>
/// Narrow, file-specific metadata extraction (not a Markdown or EDF parser).
/// </summary>
public static partial class Tier0NarrowMetadataReader
{
    private const int MaxStatusScanLines = 64;

    public static IReadOnlyDictionary<string, string> Read(string projectRootAbsolutePath)
    {
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal);
        TryReadImplementationRoadmapStatus(projectRootAbsolutePath, metadata);
        return metadata;
    }

    private static void TryReadImplementationRoadmapStatus(
        string projectRootAbsolutePath,
        IDictionary<string, string> metadata)
    {
        var relativePath = "docs/Development/Implementation_Roadmap.md";
        var absolutePath = Path.Combine(projectRootAbsolutePath, relativePath);
        if (!File.Exists(absolutePath))
        {
            return;
        }

        try
        {
            var status = ReadStatusLine(absolutePath);
            if (!string.IsNullOrWhiteSpace(status))
            {
                metadata[Tier0NarrowMetadataKeys.ImplementationRoadmapStatus] = status;
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static string? ReadStatusLine(string absolutePath)
    {
        using var reader = new StreamReader(absolutePath);
        for (var lineNumber = 0; lineNumber < MaxStatusScanLines; lineNumber++)
        {
            var line = reader.ReadLine();
            if (line is null)
            {
                break;
            }

            var match = StatusLinePattern().Match(line);
            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }
        }

        return null;
    }

    [GeneratedRegex(@"^\>\s*\*\*Status:\*\*\s*(.+?)\s*$", RegexOptions.CultureInvariant)]
    private static partial Regex StatusLinePattern();
}
