using Edf.Application.Composition;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Engine.Projects;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

public class EngineeringAgentA4T4RecoveryTests
{
    [Fact]
    public void DiscoverRecoverableOperations_ExcludesTerminalStates()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        SaveOperation(
            harness,
            sourceId,
            TransportOperationLifecycleState.ImportCompleted);
        SaveOperation(
            harness,
            sourceId,
            TransportOperationLifecycleState.CreatedNotForwarded);

        var recoverable = harness.Recovery.DiscoverRecoverableOperations(projectId);

        Assert.Single(recoverable);
        Assert.Equal(TransportOperationLifecycleState.CreatedNotForwarded, recoverable[0].LifecycleState);
    }

    [Fact]
    public async Task AssessOperation_ResultCandidateReceived_RequiresConfirmationWithoutImport()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(
            harness,
            sourceId,
            TransportOperationLifecycleState.ResultCandidateReceived);
        harness.Fake.CandidateHandler = _ => new EngineeringAgentResultCandidateResult(
            true,
            new EngineeringAgentTransportResultCandidate(
                operation.OperationId,
                "untrusted-body",
                IsReadyForParse: true),
            null);

        var assessment = await harness.Recovery.AssessOperationAsync(projectId, operation.OperationId);

        Assert.Equal(
            EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation,
            assessment.Disposition);
        Assert.True(assessment.RequiresOperatorConfirmationBeforeImport);
        Assert.Equal("untrusted-body", assessment.UntrustedResultCandidateText);
        Assert.Equal(0, harness.Workflow.ImportCallCount);
    }

    [Fact]
    public async Task ReconcileProviderState_DoesNotImportAutomatically()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(
            harness,
            sourceId,
            TransportOperationLifecycleState.ForwardAcknowledged);
        var rendered = RenderValidEngineeringResult(projectId);
        harness.Fake.CandidateHandler = id => new EngineeringAgentResultCandidateResult(
            true,
            new EngineeringAgentTransportResultCandidate(id, rendered, IsReadyForParse: true),
            null);

        var assessment = await harness.Recovery.ReconcileProviderStateAsync(projectId, operation.OperationId);

        Assert.Equal(
            EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation,
            assessment.Disposition);
        Assert.Equal(0, harness.Workflow.ImportCallCount);
        Assert.Equal(
            TransportOperationLifecycleState.ResultCandidateReceived,
            harness.Persistence.TransportOperations.Get(operation.OperationId)!.LifecycleState);
    }

    [Fact]
    public async Task AssessOperation_Ambiguous_DoesNotRedispatch()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.Ambiguous);

        var assessment = await harness.Recovery.AssessOperationAsync(projectId, operation.OperationId);

        Assert.Equal(
            EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision,
            assessment.Disposition);
        Assert.Equal(0, harness.Fake.ForwardCallCount);
    }

    [Fact]
    public async Task AssessOperation_ForwardInProgress_NotSafeToResume()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.ForwardInProgress);

        var assessment = await harness.Recovery.AssessOperationAsync(projectId, operation.OperationId);

        Assert.Equal(
            EngineeringAgentTransportRecoveryDisposition.ProviderReconciliationRequired,
            assessment.Disposition);
        Assert.Equal(0, harness.Fake.ForwardCallCount);
    }

    [Fact]
    public async Task AssessOperation_ResultCandidateReceived_DoesNotRedispatch()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.ResultCandidateReceived);

        var assessment = await harness.Recovery.AssessOperationAsync(projectId, operation.OperationId);

        Assert.Equal(
            EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation,
            assessment.Disposition);
        Assert.Equal(0, harness.Fake.ForwardCallCount);
    }

    [Fact]
    public async Task ResumeCreatedNotForwarded_PersistsForwardInProgressBeforeForward()
    {
        var projectId = ProjectConcordProjectId.New();
        var persistence = new InMemoryUserApplicationStatePersistence();
        var saveSequence = new List<TransportOperationLifecycleState>();
        var recording = new RecordingTransportOperationStore(persistence.TransportOperations, saveSequence);
        var wrapped = new DelegatingUserApplicationStatePersistence(persistence, recording);
        var (harness, sourceId) = CreateHarness(projectId, wrapped);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.CreatedNotForwarded);
        harness.Fake.ForwardHandler = request =>
        {
            Assert.Equal(TransportOperationLifecycleState.ForwardInProgress, recording.Get(request.TransportOperationId)!.LifecycleState);
            return new EngineeringAgentForwardResult(
                true,
                EngineeringAgentProviderSessionHandle.FromOpaque("session"),
                null);
        };

        var result = await harness.Recovery.ResumeCreatedNotForwardedAsync(projectId, operation.OperationId);

        Assert.True(result.Succeeded);
        Assert.Equal(operation.OperationId, result.Operation!.OperationId);
        Assert.Equal(1, result.Operation.Attempt);
        Assert.Contains(TransportOperationLifecycleState.ForwardInProgress, saveSequence);
    }

    [Fact]
    public async Task AssessOperation_ForwardAcknowledged_DoesNotRedispatch()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.ForwardAcknowledged);

        var assessment = await harness.Recovery.AssessOperationAsync(projectId, operation.OperationId);

        Assert.Equal(
            EngineeringAgentTransportRecoveryDisposition.ProviderReconciliationRequired,
            assessment.Disposition);
        Assert.Equal(0, harness.Fake.ForwardCallCount);
    }

    [Fact]
    public async Task ResumeCreatedNotForwarded_UsesSameTransportOperationId()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.CreatedNotForwarded);

        var result = await harness.Recovery.ResumeCreatedNotForwardedAsync(projectId, operation.OperationId);

        Assert.True(result.Succeeded);
        Assert.Equal(operation.OperationId, result.Operation!.OperationId);
        Assert.Equal(1, harness.Fake.ForwardCallCount);
        Assert.Equal(
            TransportOperationLifecycleState.ForwardAcknowledged,
            result.Operation.LifecycleState);
    }

    [Fact]
    public async Task DiscoverRecoverableOperations_DoesNotForwardAutomatically()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        SaveOperation(harness, sourceId, TransportOperationLifecycleState.CreatedNotForwarded);

        _ = harness.Recovery.DiscoverRecoverableOperations(projectId);

        Assert.Equal(0, harness.Fake.ForwardCallCount);
        Assert.Equal(0, harness.Fake.InitializeAsyncCallCount);
    }

    [Fact]
    public async Task AssessOperation_MissingProvider_DoesNotRunSelection()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId, registerFake: false);
        var operation = SaveOperation(
            harness,
            sourceId,
            TransportOperationLifecycleState.ForwardAcknowledged,
            EngineeringAgentProviderPluginId.Parse("missing.provider"));

        var assessment = await harness.Recovery.AssessOperationAsync(projectId, operation.OperationId);

        Assert.Equal(EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable, assessment.Disposition);
        Assert.Equal(0, harness.Fake.InitializeAsyncCallCount);
    }

    [Fact]
    public async Task ConfirmRecoveredResultImport_CompletesAfterExplicitConfirmation()
    {
        var projectId = ProjectConcordProjectId.New();
        var (harness, sourceId) = CreateHarness(projectId);
        var operation = SaveOperation(harness, sourceId, TransportOperationLifecycleState.ResultCandidateReceived);
        var rendered = RenderValidEngineeringResult(projectId);
        harness.Fake.CandidateHandler = id => new EngineeringAgentResultCandidateResult(
            true,
            new EngineeringAgentTransportResultCandidate(id, rendered, IsReadyForParse: true),
            null);

        var result = await harness.Recovery.ConfirmRecoveredResultImportAsync(projectId, operation.OperationId);

        Assert.True(result.Succeeded);
        Assert.Equal(1, harness.Workflow.ImportCallCount);
        Assert.Equal(
            TransportOperationLifecycleState.ImportCompleted,
            result.Operation!.LifecycleState);
        Assert.NotNull(result.Operation.ResultImportPackageId);
    }

    [Fact]
    public void ProductionComposition_ExposesRecoveryWithoutStartupSideEffects()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        Assert.NotNull(services.TransportRecovery);
        Assert.Single(services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds);
    }

    private static TransportOperation SaveOperation(
        RecoveryHarness harness,
        GovernedPackageId sourcePackageId,
        TransportOperationLifecycleState lifecycle,
        EngineeringAgentProviderPluginId? providerPluginId = null)
    {
        var source = harness.Persistence.RelayOperational.GetPackage(sourcePackageId)!;
        var operation = new TransportOperation(
            TransportOperationId.New(),
            sourcePackageId,
            source.Package.CorrelationId,
            providerPluginId ?? harness.Fake.PluginId,
            Attempt: 1,
            lifecycle,
            ProviderSessionHint: lifecycle == TransportOperationLifecycleState.ForwardAcknowledged
                ? EngineeringAgentProviderSessionHandle.FromOpaque("session")
                : null,
            ResultImportPackageId: null,
            CreatedUtc: DateTimeOffset.UtcNow,
            UpdatedUtc: DateTimeOffset.UtcNow);
        harness.Persistence.TransportOperations.Save(operation);
        return operation;
    }

    private static (RecoveryHarness Harness, GovernedPackageId SourcePackageId) CreateHarness(
        ProjectConcordProjectId projectId,
        bool registerFake = true) =>
        CreateHarness(projectId, new InMemoryUserApplicationStatePersistence(), registerFake);

    private static (RecoveryHarness Harness, GovernedPackageId SourcePackageId) CreateHarness(
        ProjectConcordProjectId projectId,
        IUserApplicationStatePersistence persistence,
        bool registerFake = true)
    {
        if (persistence.ProjectRegistry is InMemoryProjectRegistry inMemoryRegistry
            && inMemoryRegistry.GetById(projectId) is null)
        {
            var openedUtc = DateTimeOffset.UtcNow;
            inMemoryRegistry.RegisterProjectForTests(
                new ManagedProject(
                    projectId,
                    "t4-recovery-test",
                    EngineeringAgentTestWorkspace.DefaultLocator,
                    openedUtc,
                    openedUtc));
        }

        var workflow = new CountingGovernedRelayP0WorkflowService(
            GovernedRelayP0WorkflowService.Create(persistence));
        var sourceId = SeedValidPaHandover(persistence, projectId, workflow.Inner);

        FakeEngineeringAgentProviderPlugin? fake = null;
        EngineeringAgentPluginCatalog catalog;
        var preferences = new EngineeringAgentPluginProjectPreferencesStore();
        if (registerFake)
        {
            fake = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("fake.t4"));
            catalog = EngineeringAgentPluginCatalog.CreateEmpty().Register(fake);
            preferences.SetPluginEnabled(projectId, fake.PluginId, enabled: true);
            preferences.SetSelectedPlugin(projectId, fake.PluginId);
        }
        else
        {
            fake = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("unused"));
            catalog = EngineeringAgentPluginCatalog.CreateEmpty();
        }

        var host = new EngineeringAgentPluginHost(catalog);
        var recovery = new EngineeringAgentTransportRecoveryService(
            persistence,
            workflow,
            catalog,
            host,
            preferences);

        return (new RecoveryHarness(fake!, persistence, workflow, recovery), sourceId);
    }

    private sealed class RecordingTransportOperationStore(
        ITransportOperationStore inner,
        List<TransportOperationLifecycleState> saves) : ITransportOperationStore
    {
        public void Save(TransportOperation operation)
        {
            saves.Add(operation.LifecycleState);
            inner.Save(operation);
        }

        public TransportOperation? Get(TransportOperationId operationId) => inner.Get(operationId);

        public IReadOnlyList<TransportOperation> GetRecoverableOperations(ProjectConcordProjectId projectId) =>
            inner.GetRecoverableOperations(projectId);
    }

    private static GovernedPackageId SeedValidPaHandover(
        IUserApplicationStatePersistence persistence,
        ProjectConcordProjectId projectId,
        IGovernedRelayP0WorkflowService workflow)
    {
        var pa = new ProjectArchitectManualAdapter();
        var handover = RelaySerializationFixtures.ValidImplementationHandover() with { ProjectId = projectId };
        var rendered = pa.RenderPaReviewPackage(handover);
        var imported = pa.TryParsePaHandoverImport(rendered);
        Assert.True(imported.Validation.IsEligibleForValidatedEngineeringAgentHandover);
        persistence.RelayOperational.SavePackage(
            new PersistedGovernedRelayPackage(
                imported.Package!,
                imported.Validation.State,
                imported.Validation.Diagnostics));
        workflow.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.Continue);
        workflow.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);
        return imported.Package!.PackageId;
    }

    private static string RenderValidEngineeringResult(ProjectConcordProjectId projectId)
    {
        var package = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            authorizationDispositionPresent: false,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty) with
        {
            ProjectId = projectId,
            Kind = GovernedPackageKind.EngineeringResultImport,
            GovernanceCritical = new RelayGovernanceCriticalState(
                EngineeringAgentMode.Plan,
                null,
                null,
                RelayTestFixtures.SessionWithBothIntents(),
                RelayStopMetadata.None,
                false,
                false,
                new RelayGovernanceDirectiveFlags(false, false),
                null),
        };
        return new GovernedRelayV1Renderer().Render(package);
    }

    private sealed record RecoveryHarness(
        FakeEngineeringAgentProviderPlugin Fake,
        IUserApplicationStatePersistence Persistence,
        CountingGovernedRelayP0WorkflowService Workflow,
        IEngineeringAgentTransportRecoveryService Recovery);

    private sealed class CountingGovernedRelayP0WorkflowService : IGovernedRelayP0WorkflowService
    {
        public CountingGovernedRelayP0WorkflowService(IGovernedRelayP0WorkflowService inner) => Inner = inner;

        public IGovernedRelayP0WorkflowService Inner { get; }

        public int ImportCallCount { get; private set; }

        public EngineeringResultImportOperationResult ImportEngineeringResult(
            ProjectConcordProjectId projectId,
            string renderedText)
        {
            ImportCallCount++;
            return Inner.ImportEngineeringResult(projectId, renderedText);
        }

        public RelayP0SessionState GetSessionState(ProjectConcordProjectId projectId) =>
            Inner.GetSessionState(projectId);

        public void SetProjectArchitectSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent) =>
            Inner.SetProjectArchitectSessionIntent(projectId, intent);

        public void SetEngineeringAgentSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent) =>
            Inner.SetEngineeringAgentSessionIntent(projectId, intent);

        public PaReviewExportOperationResult GeneratePaReviewExport(
            ProjectConcordProjectId projectId,
            ProjectRoot projectRoot,
            RelayPaReviewExportOptions options) =>
            Inner.GeneratePaReviewExport(projectId, projectRoot, options);

        public PaHandoverImportOperationResult ImportPaHandover(
            ProjectConcordProjectId projectId,
            string renderedText) =>
            Inner.ImportPaHandover(projectId, renderedText);

        public EngineeringAgentHandoverPreparationResult PrepareEngineeringAgentHandover(
            GovernedRelayPackage validatedImportPackage,
            RelayValidationResult importValidation) =>
            Inner.PrepareEngineeringAgentHandover(validatedImportPackage, importValidation);

        public IReadOnlyList<RelayProvenanceEvent> ListRecentProvenance(
            ProjectConcordProjectId projectId,
            int maxEvents = 20) =>
            Inner.ListRecentProvenance(projectId, maxEvents);
    }

    private sealed class DelegatingUserApplicationStatePersistence(
        IUserApplicationStatePersistence inner,
        ITransportOperationStore transport) : IUserApplicationStatePersistence
    {
        public IProjectRegistry ProjectRegistry => inner.ProjectRegistry;

        public IUserPreferencesStore UserPreferences => inner.UserPreferences;

        public IRelayOperationalStore RelayOperational => inner.RelayOperational;

        public ITransportOperationStore TransportOperations => transport;

        public void ExecuteInTransaction(Action work) => inner.ExecuteInTransaction(work);
    }
}
