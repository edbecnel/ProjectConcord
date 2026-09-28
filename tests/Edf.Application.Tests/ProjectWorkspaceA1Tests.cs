using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Domain.Projects;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Tests;

public class ProjectWorkspaceA1Tests
{
    [Fact]
    public void FirstOpen_CreatesProjectId_SecondOpenSamePath_PreservesId()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-open-");
        var service = CreateService();

        try
        {
            var first = service.OpenProjectRoot(temp.FullName);
            var second = service.OpenProjectRoot(temp.FullName);

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.Equal(first.ProjectId, second.ProjectId);
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void OpenAtNewPathAfterMove_CreatesNewProjectId_WithoutReconcile()
    {
        var original = Directory.CreateTempSubdirectory("edf-a1-moved-old-");
        var moved = Directory.CreateTempSubdirectory("edf-a1-moved-new-");
        var service = CreateService();

        try
        {
            var first = service.OpenProjectRoot(original.FullName);
            Assert.True(first.ProjectId.HasValue);
            var originalId = first.ProjectId!.Value;

            original.Delete();

            var atNewPath = service.OpenProjectRoot(moved.FullName);
            Assert.True(atNewPath.Success);
            Assert.NotEqual(originalId, atNewPath.ProjectId);
        }
        finally
        {
            moved.Delete();
        }
    }

    [Fact]
    public void OpenUnregisteredLocator_CreatesNewProjectId()
    {
        var firstDir = Directory.CreateTempSubdirectory("edf-a1-a-");
        var secondDir = Directory.CreateTempSubdirectory("edf-a1-b-");
        var service = CreateService();

        try
        {
            var first = service.OpenProjectRoot(firstDir.FullName);
            var second = service.OpenProjectRoot(secondDir.FullName);

            Assert.True(first.Success);
            Assert.True(second.Success);
            Assert.NotEqual(first.ProjectId, second.ProjectId);
        }
        finally
        {
            firstDir.Delete();
            secondDir.Delete();
        }
    }

    [Fact]
    public void FailedOpen_DoesNotCorruptCurrentSession()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-session-");
        var service = CreateService();

        try
        {
            var opened = service.OpenProjectRoot(temp.FullName);
            Assert.True(opened.Success);

            var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            var failed = service.OpenProjectRoot(missing);

            Assert.False(failed.Success);
            Assert.Equal(opened.ProjectId, service.CurrentProjectId);
            Assert.Equal(opened.Root, service.CurrentRoot);
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void RecentOrdering_MostRecentlyOpenedFirst()
    {
        var dirA = Directory.CreateTempSubdirectory("edf-a1-recent-a-");
        var dirB = Directory.CreateTempSubdirectory("edf-a1-recent-b-");
        var service = CreateService();

        try
        {
            service.OpenProjectRoot(dirA.FullName);
            service.OpenProjectRoot(dirB.FullName);
            service.OpenProjectRoot(dirA.FullName);

            var recent = service.ListRecentProjects();
            Assert.Equal(2, recent.Count);
            Assert.Equal(dirA.FullName, Path.GetFullPath(recent[0].RegisteredLocator.NormalizedAbsolutePath));
        }
        finally
        {
            dirA.Delete();
            dirB.Delete();
        }
    }

    [Fact]
    public void RemoveFromRecent_PreservesRegistryIdentity()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-remove-");
        var registry = new InMemoryProjectRegistry();
        var service = CreateService(registry);

        try
        {
            var opened = service.OpenProjectRoot(temp.FullName);
            Assert.True(opened.ProjectId.HasValue);

            var projectId = opened.ProjectId!.Value;
            service.RemoveFromRecent(projectId);
            Assert.Empty(service.ListRecentProjects());
            Assert.NotNull(registry.GetById(projectId));
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void ReconcileProjectLocator_PreservesProjectId()
    {
        var original = Directory.CreateTempSubdirectory("edf-a1-old-");
        var relocated = Directory.CreateTempSubdirectory("edf-a1-new-");
        var service = CreateService();

        try
        {
            var opened = service.OpenProjectRoot(original.FullName);
            Assert.True(opened.ProjectId.HasValue);
            var projectId = opened.ProjectId!.Value;

            original.Delete();

            var reconcile = service.ReconcileProjectLocator(projectId, relocated.FullName);
            Assert.True(reconcile.Success);

            var reopened = service.OpenProjectById(projectId);
            Assert.True(reopened.Success);
            Assert.Equal(projectId, reopened.ProjectId);
        }
        finally
        {
            relocated.Delete();
        }
    }

    [Fact]
    public void CloseProject_ClearsSession_PreservesRegistryAndRecent()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-close-");
        var service = CreateService();

        try
        {
            var opened = service.OpenProjectRoot(temp.FullName);
            Assert.True(opened.ProjectId.HasValue);

            service.CloseProject();
            Assert.Null(service.CurrentRoot);
            Assert.Null(service.CurrentProjectId);
            Assert.Single(service.ListRecentProjects());
            Assert.True(service.ListRecentProjects()[0].ProjectId.Value != Guid.Empty);
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void OpenProjectById_FailsWhenLocatorMissingOnDisk()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-missing-");
        var service = CreateService();

        var opened = service.OpenProjectRoot(temp.FullName);
        Assert.True(opened.ProjectId.HasValue);
        var projectId = opened.ProjectId!.Value;

        temp.Delete();

        var result = service.OpenProjectById(projectId);
        Assert.False(result.Success);
    }

    [Fact]
    public void ListRecent_DerivesLocatorAvailability_FromFilesystem()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-avail-");
        var service = CreateService();

        var opened = service.OpenProjectRoot(temp.FullName);
        Assert.True(opened.ProjectId.HasValue);

        temp.Delete();

        var recent = service.ListRecentProjects();
        Assert.Single(recent);
        Assert.Equal(LocatorAvailability.MissingOnDisk, recent[0].LocatorAvailability);
    }

    [Fact]
    public void OpenProjectRoot_DoesNotCreateProjectConcordDirectory()
    {
        var temp = Directory.CreateTempSubdirectory("edf-a1-nopc-");
        var service = CreateService();

        try
        {
            var result = service.OpenProjectRoot(temp.FullName);
            Assert.True(result.Success);

            var projectConcordPath = Path.Combine(temp.FullName, ".projectconcord");
            Assert.False(Directory.Exists(projectConcordPath));
        }
        finally
        {
            temp.Delete();
        }
    }

    [Fact]
    public void MaxRecentProjects_TrimsToPolicyLimit()
    {
        var registry = new InMemoryProjectRegistry(maxRecentProjects: 2);
        var service = CreateService(registry);
        var dirs = new List<DirectoryInfo>();

        try
        {
            for (var i = 0; i < 3; i++)
            {
                var dir = Directory.CreateTempSubdirectory($"edf-a1-max-{i}-");
                dirs.Add(dir);
                service.OpenProjectRoot(dir.FullName);
            }

            Assert.Equal(2, service.ListRecentProjects().Count);
        }
        finally
        {
            foreach (var dir in dirs)
            {
                dir.Delete();
            }
        }
    }

    [Fact]
    public void LastActiveProject_IsFlaggedInRecentList()
    {
        var dirA = Directory.CreateTempSubdirectory("edf-a1-last-a-");
        var dirB = Directory.CreateTempSubdirectory("edf-a1-last-b-");
        var service = CreateService();

        try
        {
            service.OpenProjectRoot(dirA.FullName);
            service.OpenProjectRoot(dirB.FullName);

            var recent = service.ListRecentProjects();
            var lastActive = recent.Single(e => e.IsLastActive);
            Assert.Equal(service.CurrentProjectId, lastActive.ProjectId);
        }
        finally
        {
            dirA.Delete();
            dirB.Delete();
        }
    }

    private static ProjectWorkspaceService CreateService(InMemoryProjectRegistry? registry = null) =>
        new(
            new ProjectRootResolver(),
            new DegenerateAdministratorActor("test-admin"),
            registry ?? new InMemoryProjectRegistry(),
            new InMemoryUserPreferencesStore(),
            new LocalProjectRuntime());
}
