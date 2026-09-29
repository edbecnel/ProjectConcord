namespace Edf.ProjectServices.Relay;

public interface IGitHeadCommitResolver
{
    /// <summary>
    /// Returns the current HEAD commit id for a Git repository root, or null when unavailable.
    /// </summary>
    string? TryResolveHeadCommit(string projectRootAbsolutePath);
}
