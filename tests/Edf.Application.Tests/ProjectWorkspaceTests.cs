using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Domain.Projects;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Tests;

public class ProjectRootTests
{
    [Fact]
    public void Create_WithExistingDirectory_ReturnsFullPath()
    {
        var temp = Directory.CreateTempSubdirectory("edf-m1-root-");

        try
        {
            var root = ProjectRoot.Create(temp.FullName);

            Assert.Equal(Path.GetFullPath(temp.FullName), root.AbsolutePath);
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void Create_WithMissingDirectory_ThrowsDirectoryNotFoundException()
    {
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Assert.Throws<DirectoryNotFoundException>(() => ProjectRoot.Create(missing));
    }
}

public class ProjectWorkspaceServiceTests
{
    [Fact]
    public void OpenProjectRoot_WithExistingDirectory_SucceedsAndSetsCurrentRoot()
    {
        var temp = Directory.CreateTempSubdirectory("edf-m1-open-");
        var service = CreateService();

        try
        {
            var result = service.OpenProjectRoot(temp.FullName);

            Assert.True(result.Success);
            Assert.NotNull(result.Root);
            Assert.NotNull(result.ProjectId);
            Assert.Equal(result.Root, service.CurrentRoot);
            Assert.Equal(result.ProjectId, service.CurrentProjectId);
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void OpenProjectRoot_WithMissingDirectory_FailsWithoutSettingCurrentRoot()
    {
        var service = CreateService();
        var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var result = service.OpenProjectRoot(missing);

        Assert.False(result.Success);
        Assert.Null(service.CurrentRoot);
        Assert.Null(service.CurrentProjectId);
        Assert.False(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    private static ProjectWorkspaceService CreateService() =>
        new(
            new ProjectRootResolver(),
            new DegenerateAdministratorActor("test-admin"),
            new InMemoryUserApplicationStatePersistence(),
            new LocalProjectRuntime());
}

public class DegenerateAdministratorActorTests
{
    [Fact]
    public void Actor_HasAdministratorRole()
    {
        var actor = new DegenerateAdministratorActor("solo-engineer");

        Assert.True(actor.IsAdministrator);
        Assert.Equal(ProjectRole.Administrator, actor.Role);
        Assert.Equal("solo-engineer", actor.DisplayName);
    }
}

public class LayeringTests
{
    [Fact]
    public void ApplicationProject_ReferencesExpectedAssemblies()
    {
        var assembly = typeof(ProjectWorkspaceService).Assembly;
        var referenced = assembly.GetReferencedAssemblies().Select(a => a.Name).ToHashSet();

        Assert.Contains("Edf.Domain", referenced);
        Assert.Contains("Edf.Engine", referenced);
        Assert.Contains("Edf.Identity", referenced);
        Assert.Contains("Edf.ProjectServices", referenced);
    }
}
