using Edf.Application.Composition;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Application.Workflow;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Desktop.Tests;

public class GovernedWorkStateViewModelTests
{
    [Fact]
    public void DesktopComposition_ExposesWorkflowServices_OnSharedPersistence()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();

        Assert.NotNull(services.Workflow);
        Assert.NotNull(services.Workflow.WorkStateOperatorProjection);
        Assert.NotNull(services.Workflow.WorkflowInstances);
        Assert.NotNull(services.Workflow.IntakePlanningEntryTransitions);
        Assert.NotNull(services.Workspace);
        Assert.NotNull(services.RelayWorkflow);
    }

    [Fact]
    public void EmptyProject_CurrentWorkEmpty_BootstrapAvailable()
    {
        var (vm, projectId) = CreateWorkStateViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);

        Assert.False(vm.HasCurrentWorkItems);
        Assert.True(vm.CanStartGewBootstrap);
        Assert.Contains("not authorization", vm.EmptyStateMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GewBootstrap_CreatesStandardProfileInstance_AndRefreshesCurrentWork() // async for command completion delay
    {
        var (vm, projectId) = CreateWorkStateViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);

        ((AsyncRelayCommand)vm.StartGewBootstrapCommand).Execute(null);
        await Task.Delay(50);

        Assert.True(vm.HasCurrentWorkItems);
        Assert.Single(vm.CurrentWorkItems);
        Assert.False(vm.CanStartGewBootstrap);
        Assert.Contains("Standard profile", vm.CurrentWorkItems[0].WorkflowSummary, StringComparison.Ordinal);
        Assert.Contains("not grant implementation authorization", vm.StatusMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProjectionFidelity_PreservesIndeterminateSemantics_AndDoesNotImplyAuthorization()
    {
        var (vm, projectId) = CreateWorkStateViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)vm.StartGewBootstrapCommand).Execute(null);

        Assert.Contains("not authorization", vm.CandidateFrontierLine ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not determined", vm.FullyGovernedFrontierLine ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("indeterminate", vm.CurrentWorkItems[0].ActionabilityLine, StringComparison.OrdinalIgnoreCase);

        Assert.Contains("Project Architect approval", vm.OperatorSituationSummary ?? string.Empty, StringComparison.Ordinal);
        Assert.Contains("not determined", vm.CurrentWorkItems[0].OperatorSituationSummary, StringComparison.OrdinalIgnoreCase);

        if (vm.WorkflowNextActions.Count > 0)
        {
            foreach (var action in vm.WorkflowNextActions)
            {
                Assert.Contains("not authorization", action.PermissionLine, StringComparison.OrdinalIgnoreCase);
                Assert.DoesNotContain("Authorized", action.PermissionLine, StringComparison.OrdinalIgnoreCase);
            }
        }
    }

    [Fact]
    public void AfterBootstrap_OperatorSummary_UsesHumanizedStage_NotRawPlaceCodes()
    {
        var (vm, projectId) = CreateWorkStateViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)vm.StartGewBootstrapCommand).Execute(null);

        var item = vm.CurrentWorkItems[0];
        Assert.Contains("Intake", item.OperatorHeadline, StringComparison.Ordinal);
        Assert.DoesNotContain("gew.v1.place", item.OperatorHeadline, StringComparison.Ordinal);
        Assert.DoesNotContain("gew.v1.place", item.OperatorSituationSummary, StringComparison.Ordinal);
        Assert.DoesNotContain("gew.v1.place", vm.OperatorSituationSummary ?? string.Empty, StringComparison.Ordinal);
        Assert.Contains("gew.v1.place", item.TechnicalDetail, StringComparison.Ordinal);
        Assert.Contains("intake", item.OperatorSituationSummary, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AfterBootstrap_OperatorSummary_ReflectsNoDwa_AndNextStepNotAuthorization()
    {
        var (vm, projectId) = CreateWorkStateViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)vm.StartGewBootstrapCommand).Execute(null);

        Assert.Contains("Project Architect approval", vm.OperatorSituationSummary ?? string.Empty, StringComparison.Ordinal);
        Assert.True(vm.CanOpenExchange);
        Assert.Contains("guided exchange", vm.PlanningEntryActionExplanation ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.False(vm.CanEnterGovernedPlanning);
        Assert.False(vm.HasOperatorNextStepSummary);
        Assert.Contains("Where you are: Intake", vm.OperatorSituationHeadline ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public async Task PlanningEntryQualifyingImport_EnablesEnterGovernedPlanning_NotImplementationAuth()
    {
        var (workState, relay, projectId) = CreateWorkStateWithRelay();
        workState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        relay.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)workState.StartGewBootstrapCommand).Execute(null);
        await Task.Delay(50);

        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(projectId);
        relay.PaImportText = new PaAdapterShimForMvp1().Render(package);
        ((AsyncRelayCommand)relay.ImportPaHandoverCommand).Execute(null);
        await Task.Delay(50);

        workState.RefreshFromProjection();

        Assert.True(workState.CanEnterGovernedPlanning);
        Assert.False(workState.CanOpenExchange);
        Assert.Contains("does not authorize implementation", workState.OperatorSituationSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("implementation authorized", workState.OperatorSituationSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        ((AsyncRelayCommand)workState.EnterGovernedPlanningCommand).Execute(null);
        await Task.Delay(50);

        Assert.Contains("Governed Planning", workState.OperatorSituationHeadline ?? string.Empty, StringComparison.Ordinal);
        Assert.Contains("gew.v1.place", workState.CurrentWorkItems[0].TechnicalDetail, StringComparison.Ordinal);
    }

    [Fact]
    public void PlanningGoverned_WithoutPlanningDwa_ShowsObtainAuthorizationAction()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var (workState, projectId) = CreateWorkStateViewModel(persistence);
        workState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)workState.StartGewBootstrapCommand).Execute(null);
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        workState.RefreshFromProjection();

        Assert.True(workState.CanObtainPlanningAuthorization);
        Assert.False(workState.CanOpenGovernedExchange);
        Assert.Contains("Obtain Planning Authorization", workState.OperatorNextStepSummary ?? string.Empty, StringComparison.Ordinal);
        Assert.Contains("not currently authorized", workState.OperatorSituationSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("implementation authorized", workState.OperatorSituationSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PlanningGoverned_AfterGrant_RecomputesWithoutObtainAction()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var actor = new DegenerateAdministratorActor();
        var workspace = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            actor,
            persistence,
            new LocalProjectRuntime());
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-mvp1-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);
        var projectId = open.ProjectId!.Value;

        var launchAuth = false;
        var launchGovernedExchange = false;
        var workState = new GovernedWorkStateViewModel(
            workspace,
            workflow,
            null,
            () => { },
            () => launchAuth = true,
            () => launchGovernedExchange = true);
        workState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)workState.StartGewBootstrapCommand).Execute(null);
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        workState.RefreshFromProjection();
        Assert.True(workState.CanObtainPlanningAuthorization);

        workState.ObtainPlanningAuthorizationCommand.Execute(null);
        Assert.True(launchAuth);

        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        workflow.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            RelayValidationResult.Valid([]));
        workState.RefreshFromProjection();

        Assert.False(workState.CanObtainPlanningAuthorization);
        Assert.True(workState.CanOpenGovernedExchange);
        Assert.Contains("Planning development work authorization is durably on record", workState.OperatorSituationSummary ?? string.Empty, StringComparison.Ordinal);
        Assert.Contains("Repository implementation is not authorized", workState.OperatorSituationSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Governed Exchange", workState.OperatorNextStepSummary ?? string.Empty, StringComparison.Ordinal);
        Assert.DoesNotContain("Whether you may execute is not determined", workState.OperatorNextStepSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        var recoveryBefore = workflow.WorkStateRecovery.RecoverForProject(projectId);
        var placeBefore = recoveryBefore.ActiveInstances.Single().TopologyPlaceId.Value;
        var dwaCountBefore = recoveryBefore.DevelopmentWorkAuthorizations.Count;

        ((RelayCommand)workState.OpenGovernedExchangeCommand).Execute(null);
        Assert.True(launchGovernedExchange);

        var recoveryAfter = workflow.WorkStateRecovery.RecoverForProject(projectId);
        Assert.Equal(placeBefore, recoveryAfter.ActiveInstances.Single().TopologyPlaceId.Value);
        Assert.Equal(dwaCountBefore, recoveryAfter.DevelopmentWorkAuthorizations.Count);
    }

    [Fact]
    public void PlanningGoverned_WithStopActive_DoesNotOfferGovernedExchangeContinuation()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var (workState, projectId) = CreateWorkStateViewModel(persistence);
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        workState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)workState.StartGewBootstrapCommand).Execute(null);
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        workflow.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            RelayValidationResult.Valid([]));
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        workflow.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority);
        workState.RefreshFromProjection();

        Assert.False(workState.CanOpenGovernedExchange);
        Assert.False(workState.CanObtainPlanningAuthorization);
    }

    [Fact]
    public void PlanningGoverned_PostGrant_ReopenedWorkStateViewModel_PreservesGovernedExchangeCta()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var actor = new DegenerateAdministratorActor();
        var workspace = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            actor,
            persistence,
            new LocalProjectRuntime());
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-mvp1-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);
        var projectId = open.ProjectId!.Value;

        var workState = new GovernedWorkStateViewModel(workspace, workflow);
        workState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        ((AsyncRelayCommand)workState.StartGewBootstrapCommand).Execute(null);
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        workflow.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            RelayValidationResult.Valid([]));

        var reopened = new GovernedWorkStateViewModel(workspace, workflow);
        reopened.OnActiveProjectChanged(projectId, hasActiveProject: true);

        Assert.True(reopened.CanOpenGovernedExchange);
        Assert.False(reopened.CanObtainPlanningAuthorization);
    }

    [Fact]
    public void RelayImport_DoesNotRegress_AndRefreshesWorkStateCallback()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var actor = new DegenerateAdministratorActor();
        var workspace = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            actor,
            persistence,
            new LocalProjectRuntime());
        var relay = GovernedRelayP0WorkflowService.Create(persistence);
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);

        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-mvp1-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);
        var projectId = open.ProjectId!.Value;

        var workState = new GovernedWorkStateViewModel(workspace, workflow);
        var refreshCount = 0;
        var relayVm = new RelayWorkflowViewModel(
            relay,
            workspace,
            _ => Task.CompletedTask,
            automatedTransport: null,
            operatorProjections: null,
            onOperatorWorkStateMayHaveChanged: () =>
            {
                refreshCount++;
                workState.RefreshFromProjection();
            });

        workState.OnActiveProjectChanged(projectId, hasActiveProject: true);
        relayVm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        var baselineRefreshCount = refreshCount;

        relayVm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        relayVm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        var incomplete = RelaySerializationFixtures.CreateBasePackage(
            new RelayGovernanceDirectiveFlags(true, true),
            authorizationDispositionPresent: true,
            workContextPresent: true) with { ProjectId = projectId };
        relayVm.PaImportText = new PaAdapterShimForMvp1().Render(incomplete);
        ((AsyncRelayCommand)relayVm.ImportPaHandoverCommand).Execute(null);

        Assert.True(refreshCount > baselineRefreshCount);
        Assert.False(workState.HasCurrentWorkItems);
    }

    private static (
        GovernedWorkStateViewModel WorkState,
        RelayWorkflowViewModel Relay,
        Edf.Domain.Projects.ProjectConcordProjectId ProjectId) CreateWorkStateWithRelay()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var actor = new DegenerateAdministratorActor();
        var workspace = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            actor,
            persistence,
            new LocalProjectRuntime());
        var relayService = GovernedRelayP0WorkflowService.Create(persistence);
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);

        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-mvp1-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);

        RelayWorkflowViewModel? relayRef = null;
        var workState = new GovernedWorkStateViewModel(
            workspace,
            workflow,
            () => relayRef!.ConsumedPaHandover,
            () => { });
        relayRef = new RelayWorkflowViewModel(
            relayService,
            workspace,
            _ => Task.CompletedTask,
            null,
            null,
            () => workState.RefreshFromProjection());

        return (workState, relayRef, open.ProjectId!.Value);
    }

    private static (GovernedWorkStateViewModel Vm, Edf.Domain.Projects.ProjectConcordProjectId ProjectId) CreateWorkStateViewModel(
        InMemoryUserApplicationStatePersistence? persistence = null)
    {
        persistence ??= new InMemoryUserApplicationStatePersistence();
        var actor = new DegenerateAdministratorActor();
        var workspace = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            actor,
            persistence,
            new LocalProjectRuntime());
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);

        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-mvp1-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);

        var vm = new GovernedWorkStateViewModel(workspace, workflow);
        return (vm, open.ProjectId!.Value);
    }

    private sealed class PaAdapterShimForMvp1
    {
        private readonly Edf.Application.Relay.ProjectArchitect.ProjectArchitectManualAdapter _inner = new();

        public string Render(GovernedRelayPackage package) => _inner.RenderPaReviewPackage(package);
    }
}
