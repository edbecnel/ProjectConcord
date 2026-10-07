using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
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

public class ExchangeGovernedContextViewModelTests
{
    [Fact]
    public void PlanningRegionContinuation_ShowsGovernedExchangeContext_AndRecommendsPlan()
    {
        var (vm, workflow, projectId) = CreateRelayWithWorkflowProjection();
        SetupPlanningGovernedWithPlanningDwa(projectId, workflow);
        vm.Relay.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.Relay.RefreshGovernedExchangeContext(applyRecommendedPlan: true);

        Assert.True(vm.Relay.HasGovernedExchangeContext);
        Assert.Contains("Governed Planning", vm.Relay.GovernedExchangeContextSummary, StringComparison.Ordinal);
        Assert.Contains("Planning development work authorization is on record", vm.Relay.GovernedExchangeContextSummary, StringComparison.Ordinal);
        Assert.Contains("Repository implementation is not authorized", vm.Relay.GovernedExchangeContextSummary, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Recommended Engineering Agent mode: Plan", vm.Relay.GovernedExchangeContextSummary, StringComparison.Ordinal);
        Assert.Contains("routing/execution intent", vm.Relay.GovernedExchangeContextSummary, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Choosing Agent does not grant implementation permission", vm.Relay.GovernedExchangeContextSummary, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(EngineeringAgentMode.Plan, vm.Relay.EngineeringAgentMode);
        Assert.True(vm.Relay.ShowPlanningRegionModeRecommendation);
    }

    [Fact]
    public void PlanningRegionContinuation_AgentSelectable_DoesNotImplyImplementationAuthorization()
    {
        var (vm, workflow, projectId) = CreateRelayWithWorkflowProjection();
        SetupPlanningGovernedWithPlanningDwa(projectId, workflow);
        vm.Relay.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.Relay.RefreshGovernedExchangeContext(applyRecommendedPlan: true);

        vm.Relay.EngineeringAgentMode = EngineeringAgentMode.Agent;

        Assert.Equal(EngineeringAgentMode.Agent, vm.Relay.EngineeringAgentMode);
        Assert.True(vm.Relay.HasGovernedExchangeContext);
        Assert.Contains("does not grant implementation permission", vm.Relay.GovernedExchangeContextSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PlanningGoverned_WithoutPlanningDwa_DoesNotShowContinuationContext()
    {
        var (vm, workflow, projectId) = CreateRelayWithWorkflowProjection();
        vm.WorkState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)vm.WorkState.StartGewBootstrapCommand).Execute(null);
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        vm.Relay.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.Relay.RefreshGovernedExchangeContext(applyRecommendedPlan: true);

        Assert.False(vm.Relay.HasGovernedExchangeContext);
    }

    [Fact]
    public void StopActive_DoesNotShowPlanningContinuationContext()
    {
        var (vm, workflow, projectId) = CreateRelayWithWorkflowProjection();
        SetupPlanningGovernedWithPlanningDwa(projectId, workflow);
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        vm.Relay.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.Relay.RefreshGovernedExchangeContext(applyRecommendedPlan: true);

        Assert.False(vm.Relay.HasGovernedExchangeContext);
    }

    [Fact]
    public void PriorModeControls_HiddenWhenPlanWithNoPrior_AndShownWhenAgentSelected()
    {
        var (vm, _, projectId) = CreateRelayWithWorkflowProjection();
        vm.Relay!.OnActiveProjectChanged(projectId, hasActiveProject: true);

        Assert.False(vm.Relay.ShowPriorEngineeringAgentModeControls);

        vm.Relay.EngineeringAgentMode = EngineeringAgentMode.Agent;

        Assert.True(vm.Relay.ShowPriorEngineeringAgentModeControls);
    }

    private static void SetupPlanningGovernedWithPlanningDwa(
        Edf.Domain.Projects.ProjectConcordProjectId projectId,
        WorkflowApplicationServices workflow)
    {
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        if (instance.TopologyPlaceId.Value != GewV1TopologyPlaces.PlanningGoverned)
        {
            workflow.WorkflowInstances.RecordGovernedTopologyTransition(
                instance.InstanceId,
                TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
                instance.ResourceVersion,
                GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                    GovernedCorrelationId.New(),
                    GovernedPackageId.New()));
        }

        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        workflow.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            RelayValidationResult.Valid([]));
    }

    private static (MainWindowViewModel Vm, WorkflowApplicationServices Workflow, Edf.Domain.Projects.ProjectConcordProjectId ProjectId) CreateRelayWithWorkflowProjection()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var vm = new MainWindowViewModel(
            services.Workspace,
            services.RelayWorkflow,
            (_, _) => Task.FromResult<string?>(null),
            workflowServices: services.Workflow);
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-exchange-ctx-" + Guid.NewGuid().ToString("N")));
        vm.OpenProjectRootAtPath(dir.FullName);
        ((AsyncRelayCommand)vm.WorkState!.StartGewBootstrapCommand).Execute(null);
        return (vm, services.Workflow, services.Workspace.CurrentProjectId!.Value);
    }
}
