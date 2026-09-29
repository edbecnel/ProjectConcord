namespace Edf.ProjectServices.Relay;

using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Read-only Tier-0 snapshot capture against an explicit Project Root (infrastructure).
/// </summary>
public sealed class Tier0RelaySnapshotProvider
{
    private readonly IGitHeadCommitResolver _gitHeadCommitResolver;

    public Tier0RelaySnapshotProvider()
        : this(new GitHeadCommitResolver())
    {
    }

    public Tier0RelaySnapshotProvider(IGitHeadCommitResolver gitHeadCommitResolver)
    {
        _gitHeadCommitResolver = gitHeadCommitResolver ?? throw new ArgumentNullException(nameof(gitHeadCommitResolver));
    }

    public Tier0RelaySnapshot Capture(ProjectRoot projectRoot)
    {
        ArgumentNullException.ThrowIfNull(projectRoot);

        var rootPath = projectRoot.AbsolutePath;
        var gitHead = _gitHeadCommitResolver.TryResolveHeadCommit(rootPath);
        var knownPathsPresent = CollectKnownPathsPresent(rootPath);
        var narrowMetadata = Tier0NarrowMetadataReader.Read(rootPath);

        return new Tier0RelaySnapshot(gitHead, knownPathsPresent, narrowMetadata);
    }

    private static IReadOnlyList<string> CollectKnownPathsPresent(string projectRootAbsolutePath)
    {
        var present = new List<string>(Tier0RelayKnownPaths.All.Count);

        foreach (var relativePath in Tier0RelayKnownPaths.All)
        {
            var absolutePath = Path.Combine(projectRootAbsolutePath, relativePath);
            if (File.Exists(absolutePath) || Directory.Exists(absolutePath))
            {
                present.Add(relativePath);
            }
        }

        return present;
    }
}
