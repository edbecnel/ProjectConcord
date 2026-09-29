namespace Edf.ProjectServices.Relay;

using System.Diagnostics;
using System.Text.RegularExpressions;

public sealed partial class GitHeadCommitResolver : IGitHeadCommitResolver
{
    private static readonly Regex CommitShaPattern = CommitShaRegex();

    public string? TryResolveHeadCommit(string projectRootAbsolutePath)
    {
        if (string.IsNullOrWhiteSpace(projectRootAbsolutePath))
        {
            return null;
        }

        var gitDirectory = Path.Combine(projectRootAbsolutePath, ".git");
        if (!Directory.Exists(gitDirectory) && !File.Exists(gitDirectory))
        {
            return null;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse HEAD",
                WorkingDirectory = projectRootAbsolutePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using var process = Process.Start(startInfo);
            if (process is null)
            {
                return null;
            }

            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
            {
                return null;
            }

            return CommitShaPattern.IsMatch(output) ? output : null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    [GeneratedRegex("^[0-9a-fA-F]{7,40}$", RegexOptions.CultureInvariant)]
    private static partial Regex CommitShaRegex();
}
