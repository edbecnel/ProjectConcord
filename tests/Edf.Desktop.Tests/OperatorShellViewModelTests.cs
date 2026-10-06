using Edf.Application.Composition;
using Edf.Desktop.ViewModels;

namespace Edf.Desktop.Tests;

public class OperatorShellViewModelTests
{
    [Fact]
    public void DefaultOperatorTab_IsCurrentWork()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var vm = new MainWindowViewModel(
            services.Workspace,
            services.RelayWorkflow,
            (_, _) => Task.FromResult<string?>(null),
            workflowServices: services.Workflow);

        Assert.Equal(OperatorShellTab.CurrentWork, vm.SelectedOperatorTab);
        Assert.Equal(0, vm.SelectedOperatorTabIndex);
    }

    [Fact]
    public void OperatorShell_ExposesThreeLogicalAreas()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var vm = new MainWindowViewModel(
            services.Workspace,
            services.RelayWorkflow,
            (_, _) => Task.FromResult<string?>(null),
            workflowServices: services.Workflow);

        Assert.NotNull(vm.WorkState);
        Assert.NotNull(vm.Relay);
        Assert.NotNull(vm.Relay!.GeneratePaReviewCommand);
        Assert.NotNull(vm.WorkState!.StartGewBootstrapCommand);
    }

    [Fact]
    public void OpenProject_SwitchesToCurrentWorkTab_AndRefreshesWorkState()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var vm = new MainWindowViewModel(
            services.Workspace,
            services.RelayWorkflow,
            (_, _) => Task.FromResult<string?>(null),
            workflowServices: services.Workflow);

        vm.SelectedOperatorTabIndex = (int)OperatorShellTab.Projects;
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-shell-" + Guid.NewGuid().ToString("N")));
        vm.OpenProjectRootAtPath(dir.FullName);

        Assert.True(vm.HasActiveProject);
        Assert.Equal(OperatorShellTab.CurrentWork, vm.SelectedOperatorTab);
        Assert.True(vm.WorkState!.CanStartGewBootstrap);
    }
}
