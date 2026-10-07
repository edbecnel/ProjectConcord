using Edf.Application.Composition;
using Edf.Application.Relay;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Application.Workflow;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

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

    [Fact]
    public void OpenGovernedExchange_FromPostPlanningDwa_SwitchesToLegacyExchange_ClearsGuidedContext()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var vm = new MainWindowViewModel(
            services.Workspace,
            services.RelayWorkflow,
            (_, _) => Task.FromResult<string?>(null),
            workflowServices: services.Workflow);

        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-shell-" + Guid.NewGuid().ToString("N")));
        vm.OpenProjectRootAtPath(dir.FullName);
        var projectId = services.Workspace.CurrentProjectId!.Value;

        ((AsyncRelayCommand)vm.WorkState!.StartGewBootstrapCommand).Execute(null);
        var instance = services.Workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        services.Workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        vm.WorkState.RefreshFromProjection();
        Assert.True(vm.WorkState.CanObtainPlanningAuthorization);
        ((RelayCommand)vm.WorkState.ObtainPlanningAuthorizationCommand).Execute(null);
        Assert.True(vm.IsPlanningAuthorizationGuidedVisible);

        services.Workflow.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            RelayValidationResult.Valid([]));
        vm.WorkState.RefreshFromProjection();

        Assert.True(vm.WorkState.CanOpenGovernedExchange);
        ((RelayCommand)vm.WorkState.OpenGovernedExchangeCommand).Execute(null);

        Assert.Equal(OperatorShellTab.Exchange, vm.SelectedOperatorTab);
        Assert.False(vm.IsAnyGuidedExchangeVisible);
        Assert.True(vm.IsLegacyExchangeVisible);
        Assert.False(vm.IsPlanningAuthorizationGuidedVisible);
    }
}
