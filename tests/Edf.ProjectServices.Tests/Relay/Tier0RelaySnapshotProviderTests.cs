using System.Diagnostics;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.ProjectServices.Relay;
using InfrastructureTier0Provider = Edf.ProjectServices.Relay.Tier0RelaySnapshotProvider;

namespace Edf.ProjectServices.Tests.Relay;

public class Tier0RelaySnapshotProviderTests
{
    [Fact]
    public void Capture_GitRepositoryWithHead_ReturnsCommitSha()
    {
        using var workspace = DisposableProjectRoot.CreateGitRepositoryWithInitialCommit();

        var provider = new InfrastructureTier0Provider();
        var snapshot = provider.Capture(workspace.ProjectRoot);

        Assert.NotNull(snapshot.GitHeadCommit);
        Assert.Equal(workspace.ExpectedHeadCommit, snapshot.GitHeadCommit);
        Assert.IsType<Tier0RelaySnapshot>(snapshot);
    }

    [Fact]
    public void Capture_NonGitProjectRoot_ReturnsSnapshotWithoutGitHead()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();

        var snapshot = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        Assert.Null(snapshot.GitHeadCommit);
    }

    [Fact]
    public void Capture_KnownGovernedPathPresent_IsListed()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();
        File.WriteAllText(Path.Combine(workspace.RootPath, "ARCHITECTURE_DECISIONS.md"), "# ADR index\n");

        var snapshot = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        Assert.Contains("ARCHITECTURE_DECISIONS.md", snapshot.KnownPathsPresent);
    }

    [Fact]
    public void Capture_MissingKnownPath_IsNotListed()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();

        var snapshot = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        Assert.DoesNotContain("ARCHITECTURE_DECISIONS.md", snapshot.KnownPathsPresent);
        Assert.DoesNotContain("PROJECT_INDEX.md", snapshot.KnownPathsPresent);
    }

    [Fact]
    public void Capture_ImplementationRoadmapStatus_ExtractsNarrowMetadata()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();
        var roadmapDir = Path.Combine(workspace.RootPath, "docs", "Development");
        Directory.CreateDirectory(roadmapDir);
        File.WriteAllText(
            Path.Combine(roadmapDir, "Implementation_Roadmap.md"),
            """
            # Roadmap

            > **Status:** Draft

            """);

        var snapshot = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        Assert.True(snapshot.NarrowMetadata.TryGetValue(
            Tier0NarrowMetadataKeys.ImplementationRoadmapStatus,
            out var status));
        Assert.Equal("Draft", status);
    }

    [Fact]
    public void Capture_MissingRoadmapStatus_DoesNotInferMetadata()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();

        var snapshot = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        Assert.Empty(snapshot.NarrowMetadata);
    }

    [Fact]
    public void Capture_DoesNotModifyRepositoryFiles()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();
        File.WriteAllText(Path.Combine(workspace.RootPath, "marker.txt"), "stable");
        var before = Directory.GetFiles(workspace.RootPath, "*", SearchOption.AllDirectories)
            .Select(path => (path, Hash: File.ReadAllText(path)))
            .ToDictionary(x => x.path, x => x.Hash);

        _ = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        var after = Directory.GetFiles(workspace.RootPath, "*", SearchOption.AllDirectories)
            .Select(path => (path, Hash: File.ReadAllText(path)))
            .ToDictionary(x => x.path, x => x.Hash);

        Assert.Equal(before.Count, after.Count);
        foreach (var (path, hash) in before)
        {
            Assert.Equal(hash, after[path]);
        }
    }

    [Fact]
    public void Capture_DoesNotCreateProjectConcordDirectory()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();

        _ = new InfrastructureTier0Provider().Capture(workspace.ProjectRoot);

        Assert.False(Directory.Exists(Path.Combine(workspace.RootPath, ".projectconcord")));
    }

    [Fact]
    public void Capture_GitResolverFailure_ReturnsSnapshotWithoutGitHead()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();
        Directory.CreateDirectory(Path.Combine(workspace.RootPath, ".git"));

        var provider = new InfrastructureTier0Provider(new FailingGitHeadCommitResolver());
        var snapshot = provider.Capture(workspace.ProjectRoot);

        Assert.Null(snapshot.GitHeadCommit);
    }

    [Fact]
    public void ProjectServicesAssembly_DoesNotReferenceEdfEngine()
    {
        var references = typeof(InfrastructureTier0Provider).Assembly
            .GetReferencedAssemblies()
            .Select(r => r.Name)
            .ToList();

        Assert.DoesNotContain("Edf.Engine", references);
    }

    [Fact]
    public void KnownPaths_AreFixedGovernedSetOnly()
    {
        Assert.Equal(
            new[]
            {
                "ARCHITECTURE_DECISIONS.md",
                "PROJECT_INDEX.md",
                "docs/Program/Gate_Reviews/",
            },
            Tier0RelayKnownPaths.All);
    }

    [Fact]
    public void ApplicationProvider_ReturnsTier0RelaySnapshotDto()
    {
        using var workspace = DisposableProjectRoot.CreatePlainDirectory();
        Edf.Application.Relay.ITier0RelaySnapshotProvider provider =
            new Edf.Application.Relay.Tier0RelaySnapshotProvider();

        var snapshot = provider.CaptureSnapshot(workspace.ProjectRoot);

        Assert.IsType<Tier0RelaySnapshot>(snapshot);
    }

    private sealed class FailingGitHeadCommitResolver : IGitHeadCommitResolver
    {
        public string? TryResolveHeadCommit(string projectRootAbsolutePath) => null;
    }
}

internal sealed class DisposableProjectRoot : IDisposable
{
    private readonly DirectoryInfo _tempDirectory;

    public string RootPath { get; }

    public ProjectRoot ProjectRoot { get; }

    public string? ExpectedHeadCommit { get; private set; }

    private DisposableProjectRoot(DirectoryInfo tempDirectory)
    {
        _tempDirectory = tempDirectory;
        RootPath = tempDirectory.FullName;
        ProjectRoot = ProjectRoot.Create(RootPath);
    }

    public static DisposableProjectRoot CreatePlainDirectory()
    {
        var dir = Directory.CreateTempSubdirectory("edf-a2-t2-root-");
        return new DisposableProjectRoot(dir);
    }

    public static DisposableProjectRoot CreateGitRepositoryWithInitialCommit()
    {
        var dir = Directory.CreateTempSubdirectory("edf-a2-t2-git-");
        var rootPath = dir.FullName;
        RunGit(rootPath, "init");
        RunGit(rootPath, "config user.email test@projectconcord.local");
        RunGit(rootPath, "config user.name ProjectConcord Test");
        File.WriteAllText(Path.Combine(rootPath, "README.md"), "tier-0 fixture");
        RunGit(rootPath, "add README.md");
        RunGit(rootPath, "commit -m \"tier-0 fixture\"");

        var head = RunGit(rootPath, "rev-parse HEAD").Trim();
        var workspace = new DisposableProjectRoot(dir) { ExpectedHeadCommit = head };
        return workspace;
    }

    public void Dispose()
    {
        try
        {
            _tempDirectory.Delete(recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static string RunGit(string workingDirectory, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Failed to start git for Tier-0 test fixture.");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"git {arguments} failed: {stderr}");
        }

        return stdout;
    }
}
