using Edf.Application.Composition;
using Edf.Application.Operator.PlanningRegion;
using Edf.Application.Operator.WorkContinuity;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Workflow;
using Edf.Application.Tests.Relay;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Application.Workflow;
using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Desktop.Tests;

public class PlanningRegionGuidedExchangeViewModelTests
{
    [Fact]
    public void DisplayNameContainingMvr0005_RequiresSubjectConfirmation_NotGovernedReference()
    {
        var harness = PlanningRegionHarness.Create(projectFolderName: "workspace-MVR-0005-disposable");

        Assert.True(harness.Guided.ShowConfirmSubject);
        Assert.Null(harness.Persistence.OperatorWorkFocus.GetActiveForProject(harness.ProjectId));
    }

    [Fact]
    public void OperatorConfirmation_EmptySubjectDoesNotAdvance_OrMutateRelay()
    {
        var harness = PlanningRegionHarness.Create();

        ((RelayCommand)harness.Guided.ConfirmSubjectCommand).Execute(null);

        Assert.True(harness.Guided.ShowConfirmSubject);
        Assert.Equal(0, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Null(harness.Persistence.OperatorWorkFocus.GetActiveForProject(harness.ProjectId));
    }

    [Fact]
    public void OperatorConfirmation_ValidSubject_RecordsOperatorConfirmed_DoesNotMutateWorkflow()
    {
        var harness = PlanningRegionHarness.Create();
        var instanceBefore = harness.Workflow.WorkStateRecovery.RecoverForProject(harness.ProjectId).ActiveInstances.Single();
        var dwaBefore = harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId).Count;

        harness.Guided.PendingSubjectConfirmation = "Disposable planning-region subject";
        ((RelayCommand)harness.Guided.ConfirmSubjectCommand).Execute(null);

        var focus = harness.Persistence.OperatorWorkFocus.GetActiveForProject(harness.ProjectId);
        Assert.NotNull(focus);
        Assert.Equal(OperatorWorkFocusSubjectProvenance.OperatorConfirmed, focus!.SubjectProvenance);
        Assert.Null(focus.GovernedReferenceKey);
        Assert.False(harness.Guided.ShowConfirmSubject);

        var instanceAfter = harness.Workflow.WorkStateRecovery.RecoverForProject(harness.ProjectId).ActiveInstances.Single();
        Assert.Equal(instanceBefore.InstanceId, instanceAfter.InstanceId);
        Assert.Equal(dwaBefore, harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId).Count);
        Assert.Equal(0, harness.CountingRelay.CommitPaHandoverCallCount);
    }

    [Fact]
    public void PaResponseValidate_WithoutAttestation_DoesNotConsume()
    {
        var harness = PlanningRegionHarness.Create();
        WireToBringBackPaResponse(harness);
        var review = harness.LatestPlanningRegionReviewExport();
        harness.Guided.PaResponseDraft = PaHandoverGuidedExchangeTestArtifacts.RenderIncompleteMissingEngineeringAgentMode(
            harness.ProjectId,
            review.CorrelationId);
        harness.Guided.ImportAttestationConfirmed = false;

        ((AsyncRelayCommand)harness.Guided.ValidatePaResponseCommand).Execute(null);

        Assert.Equal(0, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(0, CountPackageConsumed(harness.Persistence, harness.ProjectId));
    }

    [Fact]
    public void PaResponseValidate_WithAttestation_EnablesValidation_DoesNotCreateImplementationDwa()
    {
        var harness = PlanningRegionHarness.Create();
        WireToBringBackPaResponse(harness);
        var review = harness.LatestPlanningRegionReviewExport();
        harness.Guided.PaResponseDraft =
            PaHandoverGuidedExchangeTestArtifacts.RenderValidPlanningDevelopmentWorkAuthorizationHandover(
                harness.ProjectId,
                review.CorrelationId);
        harness.Guided.ImportAttestationConfirmed = true;

        var implBefore = harness.Persistence.DevelopmentWorkAuthorizations
            .ListByProject(harness.ProjectId)
            .Count(a => a.AuthorizationKind == DevelopmentWorkAuthorizationKind.Implementation);

        ((AsyncRelayCommand)harness.Guided.ValidatePaResponseCommand).Execute(null);

        Assert.Equal(1, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(0, implBefore);
        Assert.Empty(
            harness.Persistence.DevelopmentWorkAuthorizations
                .ListByProject(harness.ProjectId)
                .Where(a => a.AuthorizationKind == DevelopmentWorkAuthorizationKind.Implementation));
    }

    [Fact]
    public void IncompletePaResponse_WithAttestation_DoesNotConsume_OffersCorrection()
    {
        var harness = PlanningRegionHarness.Create();
        WireToBringBackPaResponse(harness);
        var review = harness.LatestPlanningRegionReviewExport();
        harness.Guided.PaResponseDraft = PaHandoverGuidedExchangeTestArtifacts.RenderIncompleteMissingEngineeringAgentMode(
            harness.ProjectId,
            review.CorrelationId);
        harness.Guided.ImportAttestationConfirmed = true;

        ((AsyncRelayCommand)harness.Guided.ValidatePaResponseCommand).Execute(null);

        Assert.Equal(0, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(0, CountPackageConsumed(harness.Persistence, harness.ProjectId));
        Assert.True(harness.Guided.ShowCopyCorrectionRequest);
        Assert.Equal(PlanningRegionGuidedStep.BringBackPaResponse, harness.Guided.CurrentStep);
    }

    [Fact]
    public void StopActive_BlocksPrimaryActions_DoesNotOverrideStop()
    {
        var harness = PlanningRegionHarness.Create();
        ConfirmSubjectIfNeeded(harness);
        var instance = harness.Workflow.WorkStateRecovery.RecoverForProject(harness.ProjectId).ActiveInstances.Single();
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        harness.Workflow.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority);

        harness.Guided.Refresh();

        Assert.True(harness.Guided.ShowBlockedByStop);
        Assert.False(harness.Guided.HasPrimaryAction);
        Assert.NotEqual("Send to Project Architect", harness.Guided.PrimaryActionLabel);
        Assert.NotEqual("Send to Engineering Agent", harness.Guided.PrimaryActionLabel);
        Assert.True(harness.Persistence.WorkflowInstanceStops.GetSummary(instance.InstanceId)!.IsStopActive);
    }

    [Fact]
    public void PostPlanningDwa_HasPlanningAuthorization_NotImplementation_InProjection()
    {
        var harness = PlanningRegionHarness.Create();
        ConfirmSubjectIfNeeded(harness);

        var projection = harness.Workflow.WorkStateOperatorProjection.ProjectForProject(harness.ProjectId, harness.Root.AbsolutePath);
        var work = projection.CurrentWork.Single();
        Assert.Contains(DevelopmentWorkAuthorizationKind.Planning, work.ApplicableActiveAuthorizationKinds);
        Assert.DoesNotContain(DevelopmentWorkAuthorizationKind.Implementation, work.ApplicableActiveAuthorizationKinds);
        Assert.Contains("not authorized", harness.Guided.AuthorizationSummary ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static void ConfirmSubjectIfNeeded(PlanningRegionHarness harness)
    {
        if (!harness.Guided.ShowConfirmSubject)
        {
            return;
        }

        harness.Guided.PendingSubjectConfirmation = "Test subject";
        ((RelayCommand)harness.Guided.ConfirmSubjectCommand).Execute(null);
    }

    private static void WireToBringBackPaResponse(PlanningRegionHarness harness)
    {
        ConfirmSubjectIfNeeded(harness);
        harness.Guided.SelectedPaSessionIntent = AgentSessionIntent.Continue;
        harness.Guided.SelectedEaSessionIntent = AgentSessionIntent.Continue;
        ((RelayCommand)harness.Guided.ConfirmSessionContinuityCommand).Execute(null);
        ((AsyncRelayCommand)harness.Guided.PrimaryActionCommand).Execute(null);
    }

    private static int CountPackageConsumed(
        InMemoryUserApplicationStatePersistence persistence,
        ProjectConcordProjectId projectId) =>
        persistence.RelayOperational
            .ListProvenanceEvents(projectId)
            .Count(e => e.EventType == RelayProvenanceEventType.PackageConsumed);

    private sealed class PlanningRegionHarness
    {
        public InMemoryUserApplicationStatePersistence Persistence { get; }
        public IProjectWorkspaceService Workspace { get; }
        public CommitCountingRelay CountingRelay { get; }
        public WorkflowApplicationServices Workflow { get; }
        public PlanningRegionGuidedExchangeViewModel Guided { get; }
        public ProjectConcordProjectId ProjectId { get; }
        public ProjectRoot Root { get; }

        private PlanningRegionHarness(
            InMemoryUserApplicationStatePersistence persistence,
            IProjectWorkspaceService workspace,
            CommitCountingRelay countingRelay,
            WorkflowApplicationServices workflow,
            PlanningRegionGuidedExchangeViewModel guided,
            ProjectConcordProjectId projectId,
            ProjectRoot root)
        {
            Persistence = persistence;
            Workspace = workspace;
            CountingRelay = countingRelay;
            Workflow = workflow;
            Guided = guided;
            ProjectId = projectId;
            Root = root;
        }

        public static PlanningRegionHarness Create(string? projectFolderName = null)
        {
            var persistence = new InMemoryUserApplicationStatePersistence();
            var workspace = new ProjectWorkspaceService(
                new ProjectRootResolver(),
                new DegenerateAdministratorActor(),
                persistence,
                new LocalProjectRuntime());
            var inner = GovernedRelayP0WorkflowService.Create(persistence);
            var countingRelay = new CommitCountingRelay(inner);
            var workflow = WorkflowApplicationServicesFactory.Create(persistence);
            var folder = projectFolderName ?? "edf-pr-" + Guid.NewGuid().ToString("N");
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), folder));
            var open = workspace.OpenProjectRoot(dir.FullName);
            Assert.True(open.Success);
            var projectId = open.ProjectId!.Value;
            BootstrapPostPlanningDwa(workflow, projectId);
            var guided = new PlanningRegionGuidedExchangeViewModel(
                countingRelay,
                workspace,
                workflow,
                _ => Task.CompletedTask,
                () => { },
                () => new DirectoryInfo(dir.FullName).Name,
                null);
            guided.OnActiveProjectChanged(projectId, hasActiveProject: true);
            guided.ActivatePlanningRegionGuided();
            return new PlanningRegionHarness(
                persistence,
                workspace,
                countingRelay,
                workflow,
                guided,
                projectId,
                open.Root!);
        }

        public GovernedRelayPackage LatestPlanningRegionReviewExport()
        {
            var produced = Persistence.RelayOperational
                .ListProvenanceEvents(ProjectId)
                .Last(e => e.EventType == RelayProvenanceEventType.PackageProduced);
            Assert.NotNull(produced.PackageId);
            return Persistence.RelayOperational.GetPackage(produced.PackageId!.Value)!.Package;
        }

        private static void BootstrapPostPlanningDwa(WorkflowApplicationServices workflow, ProjectConcordProjectId projectId)
        {
            workflow.WorkflowInstances.CreateGewInstance(
                projectId,
                GovernedCorrelationId.New(),
                null,
                WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
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
        }
    }

    private sealed class CommitCountingRelay(IGovernedRelayP0WorkflowService inner) : IGovernedRelayP0WorkflowService
    {
        public int CommitPaHandoverCallCount { get; private set; }

        public RelayP0SessionState GetSessionState(ProjectConcordProjectId projectId) =>
            inner.GetSessionState(projectId);

        public void SetProjectArchitectSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent) =>
            inner.SetProjectArchitectSessionIntent(projectId, intent);

        public void SetEngineeringAgentSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent) =>
            inner.SetEngineeringAgentSessionIntent(projectId, intent);

        public PaReviewExportOperationResult GeneratePaReviewExport(
            ProjectConcordProjectId projectId,
            ProjectRoot projectRoot,
            RelayPaReviewExportOptions options) =>
            inner.GeneratePaReviewExport(projectId, projectRoot, options);

        public PaHandoverImportOperationResult TryValidatePaHandoverImport(
            ProjectConcordProjectId projectId,
            string renderedText,
            GovernedCorrelationId? requiredReviewCorrelationId = null) =>
            inner.TryValidatePaHandoverImport(projectId, renderedText, requiredReviewCorrelationId);

        public PaHandoverImportOperationResult CommitConsumedPaHandoverImport(
            ProjectConcordProjectId projectId,
            GovernedRelayPackage package,
            RelayValidationResult validation)
        {
            CommitPaHandoverCallCount++;
            return inner.CommitConsumedPaHandoverImport(projectId, package, validation);
        }

        public PaHandoverImportOperationResult ImportPaHandover(ProjectConcordProjectId projectId, string renderedText) =>
            inner.ImportPaHandover(projectId, renderedText);

        public EngineeringAgentHandoverPreparationResult PrepareEngineeringAgentHandover(
            GovernedRelayPackage validatedImportPackage,
            RelayValidationResult importValidation) =>
            inner.PrepareEngineeringAgentHandover(validatedImportPackage, importValidation);

        public EngineeringResultImportOperationResult ImportEngineeringResult(
            ProjectConcordProjectId projectId,
            string renderedText) =>
            inner.ImportEngineeringResult(projectId, renderedText);

        public IReadOnlyList<RelayProvenanceEvent> ListRecentProvenance(
            ProjectConcordProjectId projectId,
            int maxEvents = 20) =>
            inner.ListRecentProvenance(projectId, maxEvents);
    }
}
