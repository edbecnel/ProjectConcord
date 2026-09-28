using Edf.Desktop.ViewModels;
using Edf.Domain.Projects;

namespace Edf.Desktop.Tests;

public class MainWindowViewModelTests
{
    [Fact]
    public void Startup_RefreshesRecent_DoesNotOpenProject()
    {
        var workspace = new FakeProjectWorkspaceService();
        var projectId = ProjectConcordProjectId.New();
        workspace.SetRecentProjects(
        [
            CreateRecentEntry(projectId, LocatorAvailability.Available, isLastActive: true),
        ]);

        _ = new MainWindowViewModel(workspace, (_, _) => Task.FromResult<string?>(null));

        Assert.True(workspace.ListRecentProjectsCallCount >= 1);
        Assert.Equal(0, workspace.OpenProjectRootCallCount);
        Assert.Equal(0, workspace.OpenProjectByIdCallCount);
    }

    [Fact]
    public void RelocateCommand_IsDisabled_WhenLocatorAvailable()
    {
        var entry = CreateRecentEntry(ProjectConcordProjectId.New(), LocatorAvailability.Available);
        var item = CreateItemViewModel(entry);

        Assert.False(((AsyncRelayCommand)item.RelocateCommand).CanExecute(null));
        Assert.True(((AsyncRelayCommand)item.OpenCommand).CanExecute(null));
    }

    [Fact]
    public void RelocateCommand_IsEnabled_WhenLocatorMissingOnDisk()
    {
        var entry = CreateRecentEntry(ProjectConcordProjectId.New(), LocatorAvailability.MissingOnDisk);
        var item = CreateItemViewModel(entry);

        Assert.True(((AsyncRelayCommand)item.RelocateCommand).CanExecute(null));
        Assert.False(((AsyncRelayCommand)item.OpenCommand).CanExecute(null));
    }

    [Fact]
    public void CloseProject_ClearsActiveSession_KeepsRecent()
    {
        var workspace = new FakeProjectWorkspaceService();
        workspace.SetRecentProjects(
        [
            CreateRecentEntry(ProjectConcordProjectId.New(), LocatorAvailability.Available),
        ]);
        var vm = new MainWindowViewModel(workspace, (_, _) => Task.FromResult<string?>(null));

        ((RelayCommand)vm.CloseProjectCommand).Execute(null);

        Assert.False(vm.HasActiveProject);
        Assert.True(vm.HasRecentProjects);
    }

    private static RecentProjectItemViewModel CreateItemViewModel(RecentProjectEntry entry)
    {
        var workspace = new FakeProjectWorkspaceService();
        var host = new MainWindowViewModel(workspace, (_, _) => Task.FromResult<string?>(null));
        return new RecentProjectItemViewModel(entry, host);
    }

    private static RecentProjectEntry CreateRecentEntry(
        ProjectConcordProjectId projectId,
        LocatorAvailability availability,
        bool isLastActive = false)
    {
        var path = availability == LocatorAvailability.Available
            ? Path.GetFullPath(Path.Combine(Path.GetTempPath(), "edf-a1c-available-" + Guid.NewGuid().ToString("N")))
            : Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing");

        if (availability == LocatorAvailability.Available)
        {
            Directory.CreateDirectory(path);
        }

        return new RecentProjectEntry(
            projectId,
            "demo",
            ProjectLocator.FromPath(path),
            availability,
            DateTimeOffset.UtcNow,
            isLastActive);
    }
}
