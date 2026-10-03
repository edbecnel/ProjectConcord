namespace Edf.Domain.Workflow;

public sealed record GovernedBaselineReference(GovernedBaselineKind Kind, string? Value)
{
    public static GovernedBaselineReference Unspecified => new(GovernedBaselineKind.Unspecified, null);

    public static GovernedBaselineReference FromGitCommit(string commitId)
    {
        if (string.IsNullOrWhiteSpace(commitId))
        {
            throw new ArgumentException("Git commit id must be non-empty.", nameof(commitId));
        }

        return new GovernedBaselineReference(GovernedBaselineKind.GitCommit, commitId);
    }
}
