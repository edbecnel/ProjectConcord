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
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

public class EngineeringAgentA4T3OrchestrationTests
{
    [Fact]
    public async Task Forward_NoAutomatedProvider_ReturnsProviderUnavailable()
    {
        var projectId = ProjectConcordProjectId.New();
        var (_, _, service, sourceId) = CreateHarness(projectId, registerFake: false);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
    }

    [Fact]
    public async Task Forward_UnknownSourcePackage_ReturnsGovernanceIneligible()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, _) = CreateHarness(projectId);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(
                projectId,
                GovernedPackageId.New(),
                EngineeringAgentMode.Plan,
                EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible, result.Outcome);
        Assert.Equal(0, fake.ForwardCallCount);
        Assert.Equal(0, fake.InitializeAsyncCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_NoProviderSelected_DoesNotInitializeProvider()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("fake.t3"));
        var (_, _, service, sourceId) = CreateHarness(
            projectId,
            null,
            _ => { },
            fake,
            selectPluginByDefault: false);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(0, fake.InitializeAsyncCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_ProviderNotRegistered_DoesNotInitializeProvider()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("fake.t3"));
        var (_, _, service, sourceId) = CreateHarness(
            projectId,
            null,
            prefs =>
            {
                prefs.SetPluginEnabled(projectId, EngineeringAgentProviderPluginId.Parse("missing.plugin"), enabled: true);
                prefs.SetSelectedPlugin(projectId, EngineeringAgentProviderPluginId.Parse("missing.plugin"));
            },
            fake);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(0, fake.InitializeAsyncCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_ProviderDisabled_DoesNotInitializeProvider()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("fake.t3"));
        var (_, _, service, sourceId) = CreateHarness(
            projectId,
            null,
            prefs => prefs.SetPluginEnabled(projectId, fake.PluginId, enabled: false),
            fake);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(0, fake.InitializeAsyncCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_RenderIncompatible_DoesNotInitializeProvider()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.t3"),
            renderProtocolMajor: 99);
        var (_, _, service, sourceId) = CreateHarness(projectId, fake: fake);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(0, fake.InitializeAsyncCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_RoutingUnsupported_DoesNotInitializeProvider()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.t3"),
            supportsDebug: false);
        var (_, _, service, sourceId) = CreateHarness(projectId, fake: fake);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Debug, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(0, fake.InitializeAsyncCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_SuccessfulPreflight_InitializesProvider()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);

        await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(1, fake.InitializeAsyncCallCount);
    }

    [Fact]
    public async Task Forward_InitializationFailure_DoesNotCreateTransportOperationOrForward()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.t3"),
            initializeSucceeds: false);
        var (_, _, service, sourceId) = CreateHarness(projectId, fake: fake);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(1, fake.InitializeAsyncCallCount);
        Assert.Equal(0, fake.ForwardCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_RuntimeReadinessFailure_DoesNotCreateTransportOperationOrForward()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.t3"),
            reportsAuthenticatedWhenInitialized: false);
        var (_, _, service, sourceId) = CreateHarness(projectId, fake: fake);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable, result.Outcome);
        Assert.Equal(1, fake.InitializeAsyncCallCount);
        Assert.Equal(0, fake.ForwardCallCount);
        Assert.Null(result.Operation);
    }

    [Fact]
    public async Task Forward_FullReadiness_CreatesTransportOperation()
    {
        var projectId = ProjectConcordProjectId.New();
        var (_, _, service, sourceId) = CreateHarness(projectId);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.NotNull(result.Operation);
        Assert.Equal(TransportOperationLifecycleState.ForwardAcknowledged, result.Operation!.LifecycleState);
    }

    [Fact]
    public void Selection_Preflight_DoesNotRequireHostInitialization()
    {
        var projectId = ProjectConcordProjectId.New();
        var pluginId = EngineeringAgentProviderPluginId.Parse("preflight.only");
        var plugin = new FakeEngineeringAgentProviderPlugin(pluginId);
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty().Register(plugin);
        var preferences = new EngineeringAgentPluginProjectPreferencesStore();
        preferences.SetPluginEnabled(projectId, pluginId, enabled: true);
        preferences.SetSelectedPlugin(projectId, pluginId);
        var host = new EngineeringAgentPluginHost(catalog);
        var selection = new EngineeringAgentPluginSelectionService(catalog, host, preferences);

        var preflight = selection.EvaluatePreflightForAutomatedTransport(projectId, EngineeringAgentMode.Plan);
        Assert.True(preflight.CanProceedToInitialization);
        Assert.Equal(0, plugin.InitializeAsyncCallCount);

        var readiness = selection.EvaluateRuntimeReadinessForAutomatedTransport(projectId, EngineeringAgentMode.Plan);
        Assert.False(readiness.CanUseForAutomatedTransport);
        Assert.Equal(
            EngineeringAgentPluginSelectionUnavailableReason.PluginNotInitialized,
            readiness.UnavailableReason);
    }

    [Fact]
    public async Task Forward_InitialOperationId_IsDistinctFromGovernedIdentities()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, persistence, service, sourceId) = CreateHarness(projectId);
        TransportOperationId? capturedId = null;
        fake.ForwardHandler = request =>
        {
            capturedId = request.TransportOperationId;
            Assert.False(
                TransportOperationIdentityRules.SharesSameGuidValue(request.TransportOperationId, sourceId));
            Assert.False(
                TransportOperationIdentityRules.SharesSameGuidValue(
                    request.TransportOperationId,
                    request.CorrelationId));
            return AckForward();
        };

        await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.NotNull(capturedId);
    }

    [Fact]
    public async Task Forward_PersistsCreatedThenForwardInProgressBeforeProviderForward()
    {
        var projectId = ProjectConcordProjectId.New();
        var persistence = new InMemoryUserApplicationStatePersistence();
        var saveSequence = new List<TransportOperationLifecycleState>();
        var recording = new RecordingTransportOperationStore(persistence.TransportOperations, saveSequence);
        var wrapped = new DelegatingUserApplicationStatePersistence(persistence, recording);
        var (fake, _, service, sourceId) = CreateHarness(projectId, wrapped);
        fake.ForwardHandler = request =>
        {
            var stored = recording.Get(request.TransportOperationId);
            Assert.NotNull(stored);
            Assert.Equal(TransportOperationLifecycleState.ForwardInProgress, stored!.LifecycleState);
            return AckForward();
        };

        await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(1, fake.ForwardCallCount);
        Assert.Equal(
            [
                TransportOperationLifecycleState.CreatedNotForwarded,
                TransportOperationLifecycleState.ForwardInProgress,
            ],
            saveSequence.Take(2).ToList());
    }

    [Fact]
    public async Task Forward_PreDispatchPersistenceFailure_PreventsProviderForward()
    {
        var projectId = ProjectConcordProjectId.New();
        var inner = new InMemoryUserApplicationStatePersistence();
        var throwing = new ThrowOnSecondSaveTransportStore(inner.TransportOperations);
        var persistence = new DelegatingUserApplicationStatePersistence(inner, throwing);
        var (fake, _, service, sourceId) = CreateHarness(projectId, persistence);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.PersistenceFailed, result.Outcome);
        Assert.Equal(0, fake.ForwardCallCount);
    }

    [Fact]
    public async Task Forward_Success_PersistsSessionHandle()
    {
        var projectId = ProjectConcordProjectId.New();
        var (_, persistence, service, sourceId) = CreateHarness(projectId);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.Dispatched, result.Outcome);
        Assert.Equal("fake-session", result.Operation!.ProviderSessionHint!.Value.Value);
        Assert.Equal(
            TransportOperationLifecycleState.ForwardAcknowledged,
            persistence.TransportOperations.Get(result.Operation.OperationId)!.LifecycleState);
    }

    [Fact]
    public async Task Forward_ImmediateFailure_DoesNotRetry()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, persistence, service, sourceId) = CreateHarness(projectId);
        fake.ForwardHandler = _ => new EngineeringAgentForwardResult(
            IsAcknowledged: false,
            UpdatedSessionHint: null,
            new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.ForwardFailed,
                "forward failed"));

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ForwardFailed, result.Outcome);
        Assert.Equal(1, fake.ForwardCallCount);
        Assert.Equal(
            TransportOperationLifecycleState.ForwardFailed,
            result.Operation!.LifecycleState);
    }

    [Fact]
    public async Task Forward_AmbiguousFailure_PersistsAmbiguous()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);
        fake.ForwardHandler = _ => new EngineeringAgentForwardResult(
            IsAcknowledged: false,
            null,
            new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.AmbiguousOutcome,
                "ambiguous"));

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.Ambiguous, result.Outcome);
        Assert.Equal(TransportOperationLifecycleState.Ambiguous, result.Operation!.LifecycleState);
    }

    [Fact]
    public async Task Forward_SendsComposedExecutionPrompt_WithResponseContract()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);
        fake.ForwardHandler = _ => AckForward();

        _ = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(
                projectId,
                sourceId,
                EngineeringAgentMode.Plan,
                EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.NotNull(fake.LastForwardRequest);
        var body = fake.LastForwardRequest!.RenderedHandoverBody;
        Assert.Contains("engineeringAgentHandoverExport", body, StringComparison.Ordinal);
        Assert.Contains("engineeringResultImport", body, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, body, StringComparison.Ordinal);
        Assert.Contains(
            GovernedRelayEngineeringResultResponseInstruction.SectionHeading,
            body,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task Forward_EnvelopedMidLineRenderMarker_ExtractsAndCompletesImport()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);
        var doc = RenderValidEngineeringResult(projectId);
        var firstNewline = doc.TrimEnd().IndexOf('\n');
        var enveloped =
            $"Preamble.\nMore prose.{doc.TrimEnd()[..firstNewline]}\n{doc.TrimEnd()[(firstNewline + 1)..]}\nTrailing.";
        fake.CandidateHandler = id => new EngineeringAgentResultCandidateResult(
            true,
            new EngineeringAgentTransportResultCandidate(id, enveloped, IsReadyForParse: true),
            null);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(
                projectId,
                sourceId,
                EngineeringAgentMode.Plan,
                EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ImportCompleted, result.Outcome);
    }

    [Fact]
    public async Task Forward_ValidResultCandidate_CompletesImport()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);
        var renderedResult = RenderValidEngineeringResult(projectId);
        fake.CandidateHandler = id => new EngineeringAgentResultCandidateResult(
            true,
            new EngineeringAgentTransportResultCandidate(id, renderedResult, IsReadyForParse: true),
            null);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ImportCompleted, result.Outcome);
        Assert.Equal(TransportOperationLifecycleState.ImportCompleted, result.Operation!.LifecycleState);
        Assert.NotNull(result.Operation.ResultImportPackageId);
    }

    [Fact]
    public async Task Forward_InvalidResultCandidate_ImportRejected()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);
        fake.CandidateHandler = id => new EngineeringAgentResultCandidateResult(
            true,
            new EngineeringAgentTransportResultCandidate(id, "not a governed relay block", IsReadyForParse: true),
            null);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.ImportRejected, result.Outcome);
        Assert.Equal(TransportOperationLifecycleState.ImportRejected, result.Operation!.LifecycleState);
        Assert.Null(result.Operation.ResultImportPackageId);
    }

    [Fact]
    public async Task Cancel_ActiveOperation_PersistsCancelled()
    {
        var projectId = ProjectConcordProjectId.New();
        var (_, _, service, sourceId) = CreateHarness(projectId);
        var forward = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        var cancel = await service.CancelTransportOperationAsync(
            new EngineeringAgentAutomatedCancelRequest(projectId, forward.Operation!.OperationId));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.Cancelled, cancel.Outcome);
        Assert.Equal(TransportOperationLifecycleState.Cancelled, cancel.Operation!.LifecycleState);
    }

    [Fact]
    public async Task Cancel_AmbiguousProviderOutcome_PersistsAmbiguous()
    {
        var projectId = ProjectConcordProjectId.New();
        var (fake, _, service, sourceId) = CreateHarness(projectId);
        var forward = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));
        fake.CancelHandler = _ => new EngineeringAgentCancelResult(
            false,
            new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.AmbiguousOutcome,
                "cancel ambiguous"));

        var cancel = await service.CancelTransportOperationAsync(
            new EngineeringAgentAutomatedCancelRequest(projectId, forward.Operation!.OperationId));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.CancelAmbiguous, cancel.Outcome);
        Assert.Equal(TransportOperationLifecycleState.Ambiguous, cancel.Operation!.LifecycleState);
    }

    [Fact]
    public async Task Forward_PersistenceFailureBeforeForward_PreventsProviderForward()
    {
        var projectId = ProjectConcordProjectId.New();
        var inner = new InMemoryUserApplicationStatePersistence();
        var throwing = new ThrowOnFirstSaveTransportStore(inner.TransportOperations);
        var persistence = new DelegatingUserApplicationStatePersistence(inner, throwing);
        var (fake, _, service, sourceId) = CreateHarness(projectId, persistence);

        var result = await service.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(projectId, sourceId, EngineeringAgentMode.Plan, EngineeringAgentTestWorkspace.DefaultLocator));

        Assert.Equal(EngineeringAgentAutomatedTransportOutcome.PersistenceFailed, result.Outcome);
        Assert.Equal(0, fake.ForwardCallCount);
    }

    [Fact]
    public void ProductionComposition_HasCursorProviderAndExposesAutomatedTransport()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        Assert.Single(services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds);
        Assert.NotNull(services.AutomatedTransport);
    }

    [Fact]
    public void P0Workflow_RemainsOperationalAlongsideAutomatedTransport()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var projectId = ProjectConcordProjectId.New();
        services.RelayWorkflow.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.Continue);
        services.RelayWorkflow.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);
        var state = services.RelayWorkflow.GetSessionState(projectId);
        Assert.Equal(AgentSessionIntent.Continue, state.ProjectArchitectSessionIntent);
    }

    private static (
        FakeEngineeringAgentProviderPlugin Fake,
        IUserApplicationStatePersistence Persistence,
        IEngineeringAgentAutomatedTransportService Service,
        GovernedPackageId SourcePackageId) CreateHarness(
        ProjectConcordProjectId projectId,
        IUserApplicationStatePersistence? persistence = null,
        bool registerFake = true) =>
        CreateHarness(
            projectId,
            persistence,
            null,
            registerFake
                ? new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("fake.t3"))
                : null);

    private static (
        FakeEngineeringAgentProviderPlugin Fake,
        IUserApplicationStatePersistence Persistence,
        IEngineeringAgentAutomatedTransportService Service,
        GovernedPackageId SourcePackageId) CreateHarness(
        ProjectConcordProjectId projectId,
        IUserApplicationStatePersistence? persistence,
        Action<EngineeringAgentPluginProjectPreferencesStore>? configurePreferences,
        FakeEngineeringAgentProviderPlugin? fake,
        bool selectPluginByDefault = true)
    {
        persistence ??= new InMemoryUserApplicationStatePersistence();
        var relay = new GovernedInteractionRelayService(
            persistence.RelayOperational,
            new Tier0RelaySnapshotProvider());
        var workflow = new GovernedRelayP0WorkflowService(
            relay,
            new ProjectArchitectManualAdapter(),
            new EngineeringAgentManualRelayBridge(),
            new Tier0RelaySnapshotProvider());
        var sourceId = SeedValidPaHandover(persistence, projectId, workflow);

        EngineeringAgentPluginCatalog catalog;
        var preferences = new EngineeringAgentPluginProjectPreferencesStore();
        if (fake is not null)
        {
            catalog = EngineeringAgentPluginCatalog.CreateEmpty().Register(fake);
            preferences.SetPluginEnabled(projectId, fake.PluginId, enabled: true);
            if (selectPluginByDefault)
            {
                preferences.SetSelectedPlugin(projectId, fake.PluginId);
            }

            configurePreferences?.Invoke(preferences);
        }
        else
        {
            catalog = EngineeringAgentPluginCatalog.CreateEmpty();
        }

        var host = new EngineeringAgentPluginHost(catalog);
        var selection = new EngineeringAgentPluginSelectionService(catalog, host, preferences);
        var service = new EngineeringAgentTransportOrchestrator(
            persistence,
            workflow,
            catalog,
            host,
            selection);

        return (fake ?? new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("unused")), persistence, service, sourceId);
    }

    private static (
        FakeEngineeringAgentProviderPlugin Fake,
        IUserApplicationStatePersistence Persistence,
        IEngineeringAgentAutomatedTransportService Service,
        GovernedPackageId SourcePackageId) CreateHarness(
        ProjectConcordProjectId projectId,
        FakeEngineeringAgentProviderPlugin fake) =>
        CreateHarness(projectId, null, null, fake);

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

    private static EngineeringAgentForwardResult AckForward() =>
        new(
            IsAcknowledged: true,
            EngineeringAgentProviderSessionHandle.FromOpaque("fake-session"),
            Failure: null);

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

    private sealed class ThrowOnFirstSaveTransportStore(ITransportOperationStore inner) : ITransportOperationStore
    {
        private int _saveCount;

        public void Save(TransportOperation operation)
        {
            _saveCount++;
            if (_saveCount == 1)
            {
                throw new InvalidOperationException("Simulated persistence failure.");
            }

            inner.Save(operation);
        }

        public TransportOperation? Get(TransportOperationId operationId) => inner.Get(operationId);

        public IReadOnlyList<TransportOperation> GetRecoverableOperations(ProjectConcordProjectId projectId) =>
            inner.GetRecoverableOperations(projectId);
    }

    private sealed class ThrowOnSecondSaveTransportStore(ITransportOperationStore inner) : ITransportOperationStore
    {
        private int _saveCount;

        public void Save(TransportOperation operation)
        {
            _saveCount++;
            if (_saveCount == 2)
            {
                throw new InvalidOperationException("Simulated pre-dispatch persistence failure.");
            }

            inner.Save(operation);
        }

        public TransportOperation? Get(TransportOperationId operationId) => inner.Get(operationId);

        public IReadOnlyList<TransportOperation> GetRecoverableOperations(ProjectConcordProjectId projectId) =>
            inner.GetRecoverableOperations(projectId);
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
