using Edf.Application.Composition;
using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Tests.Relay;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Application.Workflow.PlanningAuthorization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Desktop.Tests;

public class PaHandoverGuidedValidateConsumptionGateTests
{
    [Fact]
    public void PlanningAuthorization_IncompleteParsedPackage_DoesNotConsume_OffersCorrection()
    {
        var harness = PlanningAuthorizationHarness.Create();
        WireToValidateResponseStep(harness.Guided);
        var review = harness.LatestReviewExportPackage();
        harness.Guided.PaResponseDraft = PaHandoverGuidedExchangeTestArtifacts.RenderIncompleteMissingEngineeringAgentMode(
            harness.ProjectId,
            review.CorrelationId);

        ExecuteValidate(harness.Guided);

        Assert.Equal(0, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(0, CountPackageConsumed(harness.Persistence, harness.ProjectId));
        Assert.True(harness.Guided.ShowCopyCorrectionRequest);
        Assert.Equal(PlanningAuthorizationGuidedStep.ValidateResponse, harness.Guided.CurrentStep);
    }

    [Fact]
    public void PlanningEntry_IncompleteParsedPackage_DoesNotConsume_OffersCorrection()
    {
        var harness = PlanningEntryHarness.Create();
        WireToValidateResponseStep(harness.Guided);
        var review = harness.LatestReviewExportPackage();
        harness.Guided.PaResponseDraft = PaHandoverGuidedExchangeTestArtifacts.RenderIncompleteMissingEngineeringAgentMode(
            harness.ProjectId,
            review.CorrelationId);

        ExecuteValidate(harness.Guided);

        Assert.Equal(0, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(0, CountPackageConsumed(harness.Persistence, harness.ProjectId));
        Assert.True(harness.Guided.ShowCopyCorrectionRequest);
        Assert.Equal(PlanningEntryGuidedStep.ValidateResponse, harness.Guided.CurrentStep);
    }

    [Fact]
    public void PlanningAuthorization_ValidQualifyingResponse_ConsumesOnce_DoesNotRecordPlanningDwa()
    {
        var harness = PlanningAuthorizationHarness.Create();
        WireToValidateResponseStep(harness.Guided);
        var review = harness.LatestReviewExportPackage();
        harness.Guided.PaResponseDraft =
            PaHandoverGuidedExchangeTestArtifacts.RenderValidPlanningDevelopmentWorkAuthorizationHandover(
                harness.ProjectId,
                review.CorrelationId);

        var dwaBefore = harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId).Count;
        ExecuteValidate(harness.Guided);

        Assert.Equal(1, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(1, CountPackageConsumed(harness.Persistence, harness.ProjectId));
        Assert.False(harness.Guided.ShowCopyCorrectionRequest);
        Assert.Equal(dwaBefore, harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId).Count);
        Assert.Equal(PlanningAuthorizationGuidedStep.ReviewDecision, harness.Guided.CurrentStep);
    }

    [Fact]
    public void PlanningAuthorization_ValidNonQualifyingResponse_ConsumesWithoutCorrection_DoesNotRecordPlanningDwa()
    {
        var harness = PlanningAuthorizationHarness.Create();
        WireToValidateResponseStep(harness.Guided);
        var review = harness.LatestReviewExportPackage();
        harness.Guided.PaResponseDraft =
            PaHandoverGuidedExchangeTestArtifacts.RenderValidPlanningDevelopmentWorkAuthorizationHandover(
                harness.ProjectId,
                review.CorrelationId,
                payload => payload with
                {
                    AuthorizationDisposition = payload.AuthorizationDisposition! with { PlanningAuthorized = false },
                });

        ExecuteValidate(harness.Guided);

        Assert.Equal(1, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(1, CountPackageConsumed(harness.Persistence, harness.ProjectId));
        Assert.False(harness.Guided.ShowCopyCorrectionRequest);
        Assert.Empty(harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId));
        Assert.Equal(PlanningAuthorizationGuidedStep.ValidateResponse, harness.Guided.CurrentStep);
    }

    [Fact]
    public void PlanningAuthorization_DuplicateValidCommit_ThroughWorkflow_IsIdempotent()
    {
        var harness = PlanningAuthorizationHarness.Create();
        WireToValidateResponseStep(harness.Guided);
        var review = harness.LatestReviewExportPackage();
        var paste = PaHandoverGuidedExchangeTestArtifacts.RenderValidPlanningDevelopmentWorkAuthorizationHandover(
            harness.ProjectId,
            review.CorrelationId);
        var validated = harness.CountingRelay.TryValidatePaHandoverImport(
            harness.ProjectId,
            paste,
            review.CorrelationId);
        Assert.True(Edf.Application.Operator.PaHandoverExchangeCorrectionSupport.IsAuthorizedForGuidedDurableConsumption(validated));

        harness.CountingRelay.CommitConsumedPaHandoverImport(
            harness.ProjectId,
            validated.Import.Package!,
            validated.Import.Validation);
        harness.CountingRelay.CommitConsumedPaHandoverImport(
            harness.ProjectId,
            validated.Import.Package!,
            validated.Import.Validation);

        Assert.Equal(2, harness.CountingRelay.CommitPaHandoverCallCount);
        Assert.Equal(1, CountPackageConsumed(harness.Persistence, harness.ProjectId));
    }

    [Fact]
    public void RecoveryPathFailures_AreNotAuthorizedForGuidedConsumption()
    {
        var workflow = PaHandoverCorrectionRecoveryPathSupport.CreateWorkflowWithTrustedPlanningAuthorizationReview();
        var canonical = File.ReadAllText(FixturePath("pa-handover-planning-entry-valid.relay.txt"));
        var ambiguous = canonical + Environment.NewLine + Environment.NewLine + canonical;
        var invalidJson =
            "ProjectConcord-Relay-Render: 1" + Environment.NewLine
            + "```projectconcord-relay-v1" + Environment.NewLine
            + "{ not-json" + Environment.NewLine
            + "```";

        AssertRecoveryFailureDoesNotAuthorizeConsumption(workflow, ambiguous);
        AssertRecoveryFailureDoesNotAuthorizeConsumption(workflow, invalidJson);

        var mismatchPackage = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(
            workflow.ProjectId) with
        {
            CorrelationId = workflow.TrustedReviewExport.CorrelationId,
        };
        var mismatchRendered = new GovernedRelayV1Renderer().Render(mismatchPackage);
        mismatchRendered = System.Text.RegularExpressions.Regex.Replace(
            mismatchRendered,
            @"Planning-Authorized:\s*\S+",
            "Planning-Authorized: false",
            System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        AssertRecoveryFailureDoesNotAuthorizeConsumption(workflow, mismatchRendered);

        var incomplete = PaHandoverGuidedExchangeTestArtifacts.RenderIncompleteMissingEngineeringAgentMode(
            workflow.ProjectId,
            workflow.TrustedReviewExport.CorrelationId);
        AssertRecoveryFailureDoesNotAuthorizeConsumption(workflow, incomplete);
    }

    private static void AssertRecoveryFailureDoesNotAuthorizeConsumption(
        PaHandoverCorrectionRecoveryPathSupport.WorkflowWithReview workflow,
        string pasted)
    {
        var before = CountPackageConsumed(workflow.Persistence, workflow.ProjectId);
        var result = workflow.Service.TryValidatePaHandoverImport(
            workflow.ProjectId,
            pasted,
            workflow.TrustedReviewExport.CorrelationId);
        var after = CountPackageConsumed(workflow.Persistence, workflow.ProjectId);

        Assert.Equal(before, after);
        Assert.False(Edf.Application.Operator.PaHandoverExchangeCorrectionSupport.IsAuthorizedForGuidedDurableConsumption(result));
    }

    private static void WireToValidateResponseStep(PlanningAuthorizationGuidedExchangeViewModel guided)
    {
        guided.SelectedPaSessionIntent = AgentSessionIntent.Continue;
        guided.SelectedEaSessionIntent = AgentSessionIntent.Continue;
        ((RelayCommand)guided.ConfirmSessionContinuityCommand).Execute(null);
        ((AsyncRelayCommand)guided.PrepareReviewCommand).Execute(null);
        ((AsyncRelayCommand)guided.CopyReviewCommand).Execute(null);
        ((RelayCommand)guided.AcknowledgeHavePaResponseCommand).Execute(null);
    }

    private static void WireToValidateResponseStep(PlanningEntryGuidedExchangeViewModel guided)
    {
        guided.SelectedPaSessionIntent = AgentSessionIntent.Continue;
        guided.SelectedEaSessionIntent = AgentSessionIntent.Continue;
        ((RelayCommand)guided.ConfirmSessionContinuityCommand).Execute(null);
        ((AsyncRelayCommand)guided.PrepareReviewCommand).Execute(null);
        ((AsyncRelayCommand)guided.CopyReviewCommand).Execute(null);
        ((RelayCommand)guided.AcknowledgeHavePaResponseCommand).Execute(null);
    }

    private static void ExecuteValidate(PlanningAuthorizationGuidedExchangeViewModel guided) =>
        ((AsyncRelayCommand)guided.ValidatePaResponseCommand).Execute(null);

    private static void ExecuteValidate(PlanningEntryGuidedExchangeViewModel guided) =>
        ((AsyncRelayCommand)guided.ValidatePaResponseCommand).Execute(null);

    private static int CountPackageConsumed(
        InMemoryUserApplicationStatePersistence persistence,
        ProjectConcordProjectId projectId) =>
        persistence.RelayOperational
            .ListProvenanceEvents(projectId)
            .Count(e => e.EventType == RelayProvenanceEventType.PackageConsumed);

    private static string FixturePath(string fileName) =>
        Path.Combine(RepoRoot.Find(), "docs", "Verification", "Fixtures", "MVR-0005", fileName);

    private sealed class PlanningAuthorizationHarness
    {
        public InMemoryUserApplicationStatePersistence Persistence { get; }
        public IProjectWorkspaceService Workspace { get; }
        public CommitCountingRelay CountingRelay { get; }
        public WorkflowApplicationServices Workflow { get; }
        public PlanningAuthorizationGuidedExchangeViewModel Guided { get; }
        public ProjectConcordProjectId ProjectId { get; }
        public ProjectRoot Root { get; }

        private PlanningAuthorizationHarness(
            InMemoryUserApplicationStatePersistence persistence,
            IProjectWorkspaceService workspace,
            CommitCountingRelay countingRelay,
            WorkflowApplicationServices workflow,
            PlanningAuthorizationGuidedExchangeViewModel guided,
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

        public static PlanningAuthorizationHarness Create()
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
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-gate-pa-" + Guid.NewGuid().ToString("N")));
            var open = workspace.OpenProjectRoot(dir.FullName);
            Assert.True(open.Success);
            var projectId = open.ProjectId!.Value;
            CreatePlanningGovernedInstance(workflow, projectId);
            var guided = new PlanningAuthorizationGuidedExchangeViewModel(
                countingRelay,
                workspace,
                workflow,
                _ => Task.CompletedTask,
                () => { },
                null);
            guided.OnActiveProjectChanged(projectId, hasActiveProject: true);
            guided.ActivatePlanningAuthorizationGuided();
            return new PlanningAuthorizationHarness(
                persistence,
                workspace,
                countingRelay,
                workflow,
                guided,
                projectId,
                open.Root!);
        }

        public GovernedRelayPackage LatestReviewExportPackage()
        {
            var produced = Persistence.RelayOperational
                .ListProvenanceEvents(ProjectId)
                .Last(e => e.EventType == RelayProvenanceEventType.PackageProduced);
            Assert.NotNull(produced.PackageId);
            return Persistence.RelayOperational.GetPackage(produced.PackageId!.Value)!.Package;
        }
    }

    private sealed class PlanningEntryHarness
    {
        public InMemoryUserApplicationStatePersistence Persistence { get; }
        public CommitCountingRelay CountingRelay { get; }
        public PlanningEntryGuidedExchangeViewModel Guided { get; }
        public ProjectConcordProjectId ProjectId { get; }

        private PlanningEntryHarness(
            InMemoryUserApplicationStatePersistence persistence,
            CommitCountingRelay countingRelay,
            PlanningEntryGuidedExchangeViewModel guided,
            ProjectConcordProjectId projectId)
        {
            Persistence = persistence;
            CountingRelay = countingRelay;
            Guided = guided;
            ProjectId = projectId;
        }

        public static PlanningEntryHarness Create()
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
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-gate-pe-" + Guid.NewGuid().ToString("N")));
            var open = workspace.OpenProjectRoot(dir.FullName);
            Assert.True(open.Success);
            var projectId = open.ProjectId!.Value;
            workflow.WorkflowInstances.CreateGewInstance(
                projectId,
                GovernedCorrelationId.New(),
                open.Root!.AbsolutePath,
                WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
            var guided = new PlanningEntryGuidedExchangeViewModel(
                countingRelay,
                workspace,
                workflow,
                _ => Task.CompletedTask,
                () => { },
                null);
            guided.OnActiveProjectChanged(projectId, hasActiveProject: true);
            guided.ActivatePlanningEntryGuided();
            return new PlanningEntryHarness(persistence, countingRelay, guided, projectId);
        }

        public GovernedRelayPackage LatestReviewExportPackage()
        {
            var produced = Persistence.RelayOperational
                .ListProvenanceEvents(ProjectId)
                .Last(e => e.EventType == RelayProvenanceEventType.PackageProduced);
            Assert.NotNull(produced.PackageId);
            return Persistence.RelayOperational.GetPackage(produced.PackageId!.Value)!.Package;
        }
    }

    private static void CreatePlanningGovernedInstance(WorkflowApplicationServices services, ProjectConcordProjectId projectId)
    {
        var instance = services.WorkflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        services.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
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

    private static class RepoRoot
    {
        public static string Find()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "ProjectConcord.sln")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            throw new InvalidOperationException("Repository root not found.");
        }
    }
}
