namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Restart recovery and idempotency for persisted transport operations (A4-T4). Explicitly invoked only.
/// </summary>
public sealed class EngineeringAgentTransportRecoveryService : IEngineeringAgentTransportRecoveryService
{
    private readonly IUserApplicationStatePersistence _persistence;
    private readonly IGovernedRelayP0WorkflowService _workflow;
    private readonly IEngineeringAgentPluginCatalog _catalog;
    private readonly EngineeringAgentPluginHost _host;
    private readonly IEngineeringAgentPluginProjectPreferencesStore _preferences;
    private readonly TimeProvider _clock;

    public EngineeringAgentTransportRecoveryService(
        IUserApplicationStatePersistence persistence,
        IGovernedRelayP0WorkflowService workflow,
        IEngineeringAgentPluginCatalog catalog,
        EngineeringAgentPluginHost host,
        IEngineeringAgentPluginProjectPreferencesStore preferences,
        TimeProvider? clock = null)
    {
        _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
        _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        _clock = clock ?? TimeProvider.System;
    }

    public IReadOnlyList<TransportOperation> DiscoverRecoverableOperations(ProjectConcordProjectId projectId) =>
        _persistence.TransportOperations.GetRecoverableOperations(projectId);

    public async Task<EngineeringAgentTransportRecoveryAssessment> AssessOperationAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        var loaded = TryLoadOperationForProject(projectId, transportOperationId, out var operation, out var detail);
        if (!loaded)
        {
            throw new InvalidOperationException(detail ?? "Transport operation was not found for the Project.");
        }

        var (registered, enabled) = GetRecordedProviderAvailability(projectId, operation!);
        var disposition = TransportOperationRecoverySemantics.ClassifyDisposition(
            operation!,
            registered,
            enabled);

        string? candidateText = null;
        var requiresConfirmation = disposition
            == EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation;

        if (requiresConfirmation
            && registered
            && enabled
            && await TryEnsureRecordedProviderReadyAsync(operation!, cancellationToken).ConfigureAwait(false))
        {
            candidateText = TryReacquireUntrustedCandidateText(operation!, cancellationToken);
        }

        return BuildAssessment(operation!, disposition, detail: null, candidateText);
    }

    public async Task<EngineeringAgentTransportRecoveryActionResult> ResumeCreatedNotForwardedAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        var loaded = TryLoadOperationForProject(projectId, transportOperationId, out var operation, out var detail);
        if (!loaded || operation is null)
        {
            return Failed(null, EngineeringAgentTransportRecoveryDisposition.RecoveryFailed, detail);
        }

        if (operation.LifecycleState != TransportOperationLifecycleState.CreatedNotForwarded)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                "Only CreatedNotForwarded operations may be explicitly resumed.");
        }

        var forward = await ExecuteForwardForExistingOperationAsync(
            projectId,
            operation,
            cancellationToken).ConfigureAwait(false);
        if (!forward.Succeeded)
        {
            return forward;
        }

        return new EngineeringAgentTransportRecoveryActionResult(
            Succeeded: true,
            forward.Operation,
            Disposition: EngineeringAgentTransportRecoveryDisposition.SafeToResumeBeforeDispatch,
            OrchestrationOutcome: forward.OrchestrationOutcome,
            Detail: forward.Detail,
            ImportValidation: forward.ImportValidation);
    }

    public async Task<EngineeringAgentTransportRecoveryAssessment> ReconcileProviderStateAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        var loaded = TryLoadOperationForProject(projectId, transportOperationId, out var operation, out var detail);
        if (!loaded || operation is null)
        {
            throw new InvalidOperationException(detail ?? "Transport operation was not found for the Project.");
        }

        if (operation.LifecycleState == TransportOperationLifecycleState.CreatedNotForwarded)
        {
            return BuildAssessment(
                operation,
                EngineeringAgentTransportRecoveryDisposition.SafeToResumeBeforeDispatch,
                "Forward did not occur; explicit resume is required before dispatch.");
        }

        if (operation.LifecycleState == TransportOperationLifecycleState.ResultCandidateReceived)
        {
            return await AssessOperationAsync(projectId, transportOperationId, cancellationToken)
                .ConfigureAwait(false);
        }

        if (operation.LifecycleState == TransportOperationLifecycleState.Ambiguous
            || operation.LifecycleState == TransportOperationLifecycleState.TimedOut)
        {
            return BuildAssessment(
                operation,
                EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision,
                "Ambiguous transport outcome requires operator decision before retry or import.");
        }

        if (!await TryEnsureRecordedProviderReadyAsync(operation, cancellationToken).ConfigureAwait(false))
        {
            return BuildAssessment(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable,
                "Recorded provider is unavailable for neutral reconciliation.");
        }

        if (!_catalog.TryGetPlugin(operation.ProviderPluginId, out var plugin) || plugin is null)
        {
            return BuildAssessment(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable,
                "Recorded provider is not registered.");
        }

        var candidateResult = plugin.TryGetResultCandidate(operation.OperationId, cancellationToken);
        if (candidateResult.Failure?.Kind == EngineeringAgentProviderFailureKind.AmbiguousOutcome)
        {
            var ambiguous = PersistLifecycle(
                operation,
                TransportOperationLifecycleState.Ambiguous);
            return BuildAssessment(
                ambiguous,
                EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision,
                candidateResult.Failure.Message);
        }

        if (!candidateResult.HasCandidate || candidateResult.Candidate is null)
        {
            return BuildAssessment(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderReconciliationRequired,
                "Provider did not return a result candidate.");
        }

        var withCandidate = PersistLifecycle(
            operation,
            TransportOperationLifecycleState.ResultCandidateReceived);
        return BuildAssessment(
            withCandidate,
            EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation,
            candidateText: candidateResult.Candidate.UntrustedImportText);
    }

    public async Task<EngineeringAgentTransportRecoveryActionResult> ConfirmRecoveredResultImportAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        var loaded = TryLoadOperationForProject(projectId, transportOperationId, out var operation, out var detail);
        if (!loaded || operation is null)
        {
            return Failed(null, EngineeringAgentTransportRecoveryDisposition.RecoveryFailed, detail);
        }

        if (operation.LifecycleState != TransportOperationLifecycleState.ResultCandidateReceived)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                "Confirmed import is only valid for ResultCandidateReceived operations.");
        }

        if (!await TryEnsureRecordedProviderReadyAsync(operation, cancellationToken).ConfigureAwait(false)
            || !_catalog.TryGetPlugin(operation.ProviderPluginId, out var plugin)
            || plugin is null)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable,
                "Recorded provider is unavailable for candidate reacquisition.");
        }

        var candidateResult = plugin.TryGetResultCandidate(operation.OperationId, cancellationToken);
        if (!candidateResult.HasCandidate || candidateResult.Candidate is null)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                "Untrusted result candidate could not be reacquired for confirmed import.");
        }

        var importOperation = _workflow.ImportEngineeringResult(
            projectId,
            candidateResult.Candidate.UntrustedImportText);

        if (importOperation.Import.Package is null
            || importOperation.Import.Validation.State != RelayValidationState.Valid
            || !importOperation.ProjectIdMatched)
        {
            var rejected = PersistLifecycle(
                operation,
                TransportOperationLifecycleState.ImportRejected);
            return new EngineeringAgentTransportRecoveryActionResult(
                Succeeded: false,
                rejected,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                OrchestrationOutcome: EngineeringAgentAutomatedTransportOutcome.ImportRejected,
                Detail: importOperation.ProjectIdMismatchMessage
                    ?? "Engineering result import boundary rejected the provider candidate.",
                ImportValidation: importOperation.Import.Validation);
        }

        var completed = PersistLifecycle(
            operation with
            {
                ResultImportPackageId = importOperation.Import.Package.PackageId,
            },
            TransportOperationLifecycleState.ImportCompleted);

        return new EngineeringAgentTransportRecoveryActionResult(
            Succeeded: true,
            completed,
            EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation,
            OrchestrationOutcome: EngineeringAgentAutomatedTransportOutcome.ImportCompleted,
            Detail: null,
            ImportValidation: importOperation.Import.Validation);
    }

    private async Task<EngineeringAgentTransportRecoveryActionResult> ExecuteForwardForExistingOperationAsync(
        ProjectConcordProjectId projectId,
        TransportOperation operation,
        CancellationToken cancellationToken)
    {
        var persistedSource = _persistence.RelayOperational.GetPackage(operation.SourcePackageId);
        if (persistedSource is null)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                "Source governed package was not found.");
        }

        var boundaryValidation = ToValidationResult(persistedSource);
        var preparation = _workflow.PrepareEngineeringAgentHandover(
            persistedSource.Package,
            boundaryValidation);
        if (!preparation.IsReadyForManualTransfer || preparation.RenderedHandover is null)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                "Governed handover preparation failed.");
        }

        if (!await TryEnsureRecordedProviderReadyAsync(operation, cancellationToken).ConfigureAwait(false))
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable,
                "Recorded provider is unavailable.");
        }

        if (!_catalog.TryGetPlugin(operation.ProviderPluginId, out var plugin) || plugin is null)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable,
                "Recorded provider is not registered.");
        }

        var routingIntent = ResolveRoutingIntent(persistedSource.Package);
        var compatibility = EngineeringAgentPluginCompatibilityEvaluator.Assess(
            plugin,
            RelayRenderVersion.V1.Major,
            routingIntent);
        if (!compatibility.IsFullyCompatible)
        {
            return Failed(
                operation,
                EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable,
                "Recorded provider is incompatible with the governed handover context.");
        }

        var continuity = _persistence.RelayOperational.GetSessionContinuity(projectId);
        var forwardRequest = new EngineeringAgentForwardRequest(
            operation.OperationId,
            operation.SourcePackageId,
            operation.CorrelationId,
            routingIntent,
            preparation.RenderedHandover,
            continuity,
            operation.ProviderSessionHint);

        var dispatchBase = operation;
        if (operation.LifecycleState == TransportOperationLifecycleState.CreatedNotForwarded)
        {
            if (!TransportOperationPreDispatchPersistence.TryPersistForwardInProgress(
                    _persistence.TransportOperations,
                    operation,
                    _clock,
                    out dispatchBase,
                    out var preDispatchFailure))
            {
                return Failed(
                    operation,
                    EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                    preDispatchFailure?.Message ?? "Failed to persist pre-dispatch transport state.");
            }
        }

        EngineeringAgentForwardResult forwardResult;
        try
        {
            forwardResult = plugin.Forward(forwardRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            var ambiguous = PersistLifecycle(dispatchBase, TransportOperationLifecycleState.Ambiguous);
            return new EngineeringAgentTransportRecoveryActionResult(
                Succeeded: false,
                ambiguous,
                EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision,
                OrchestrationOutcome: EngineeringAgentAutomatedTransportOutcome.Ambiguous,
                Detail: ex.Message,
                ImportValidation: null);
        }

        if (forwardResult.Failure is not null)
        {
            var lifecycle = forwardResult.Failure.Kind == EngineeringAgentProviderFailureKind.AmbiguousOutcome
                ? TransportOperationLifecycleState.Ambiguous
                : TransportOperationLifecycleState.ForwardFailed;
            var failed = PersistLifecycle(dispatchBase, lifecycle);
            var outcome = lifecycle == TransportOperationLifecycleState.Ambiguous
                ? EngineeringAgentAutomatedTransportOutcome.Ambiguous
                : EngineeringAgentAutomatedTransportOutcome.ForwardFailed;
            return new EngineeringAgentTransportRecoveryActionResult(
                Succeeded: false,
                failed,
                lifecycle == TransportOperationLifecycleState.Ambiguous
                    ? EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision
                    : EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                outcome,
                forwardResult.Failure.Message,
                null);
        }

        if (!forwardResult.IsAcknowledged)
        {
            var failed = PersistLifecycle(dispatchBase, TransportOperationLifecycleState.ForwardFailed);
            return new EngineeringAgentTransportRecoveryActionResult(
                Succeeded: false,
                failed,
                EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
                EngineeringAgentAutomatedTransportOutcome.ForwardFailed,
                "Provider did not acknowledge the forward request.",
                null);
        }

        var acknowledged = PersistLifecycle(
            dispatchBase with { ProviderSessionHint = forwardResult.UpdatedSessionHint ?? dispatchBase.ProviderSessionHint },
            TransportOperationLifecycleState.ForwardAcknowledged);

        return new EngineeringAgentTransportRecoveryActionResult(
            Succeeded: true,
            acknowledged,
            EngineeringAgentTransportRecoveryDisposition.ProviderReconciliationRequired,
            EngineeringAgentAutomatedTransportOutcome.Dispatched,
            null,
            null);
    }

    private bool TryLoadOperationForProject(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        out TransportOperation? operation,
        out string? detail)
    {
        operation = _persistence.TransportOperations.Get(transportOperationId);
        if (operation is null)
        {
            detail = "Transport operation was not found.";
            return false;
        }

        var source = _persistence.RelayOperational.GetPackage(operation.SourcePackageId);
        if (source is null || source.Package.ProjectId != projectId)
        {
            detail = "Transport operation does not belong to the active ProjectConcord project.";
            operation = null;
            return false;
        }

        detail = null;
        return true;
    }

    private (bool Registered, bool Enabled) GetRecordedProviderAvailability(
        ProjectConcordProjectId projectId,
        TransportOperation operation)
    {
        var registered = _catalog.TryGetPlugin(operation.ProviderPluginId, out _);
        var prefs = _preferences.GetPreferences(projectId);
        var enabled = prefs.EnabledPluginIds.Contains(operation.ProviderPluginId);
        return (registered, enabled);
    }

    private async Task<bool> TryEnsureRecordedProviderReadyAsync(
        TransportOperation operation,
        CancellationToken cancellationToken)
    {
        if (!_catalog.TryGetPlugin(operation.ProviderPluginId, out var plugin) || plugin is null)
        {
            return false;
        }

        if (!_host.IsInitialized(operation.ProviderPluginId))
        {
            var init = await _host.InitializePluginAsync(operation.ProviderPluginId, cancellationToken)
                .ConfigureAwait(false);
            if (!init.Succeeded)
            {
                return false;
            }
        }

        var health = plugin.GetHealth();
        return health.IsInitialized && health.IsAuthenticated;
    }

    private string? TryReacquireUntrustedCandidateText(
        TransportOperation operation,
        CancellationToken cancellationToken)
    {
        if (!_catalog.TryGetPlugin(operation.ProviderPluginId, out var plugin) || plugin is null)
        {
            return null;
        }

        var candidateResult = plugin.TryGetResultCandidate(operation.OperationId, cancellationToken);
        return candidateResult.HasCandidate ? candidateResult.Candidate?.UntrustedImportText : null;
    }

    private TransportOperation PersistLifecycle(
        TransportOperation operation,
        TransportOperationLifecycleState lifecycleState)
    {
        var updated = operation with
        {
            LifecycleState = lifecycleState,
            UpdatedUtc = _clock.GetUtcNow(),
        };
        _persistence.TransportOperations.Save(updated);
        return updated;
    }

    private static EngineeringAgentTransportRecoveryAssessment BuildAssessment(
        TransportOperation operation,
        EngineeringAgentTransportRecoveryDisposition disposition,
        string? detail = null,
        string? candidateText = null) =>
        new(
            operation,
            disposition,
            detail,
            candidateText,
            RequiresOperatorConfirmationBeforeImport: disposition
                == EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation);

    private static EngineeringAgentTransportRecoveryActionResult Failed(
        TransportOperation? operation = null,
        EngineeringAgentTransportRecoveryDisposition disposition = EngineeringAgentTransportRecoveryDisposition.RecoveryFailed,
        string? detail = null) =>
        new(false, operation, disposition, null, detail, null);

    private static EngineeringAgentMode ResolveRoutingIntent(GovernedRelayPackage package) =>
        package.GovernanceCritical.EngineeringAgentMode ?? EngineeringAgentMode.Plan;

    private static RelayValidationResult ToValidationResult(PersistedGovernedRelayPackage persisted) =>
        persisted.ValidationState switch
        {
            RelayValidationState.Valid => RelayValidationResult.Valid(persisted.ValidationDiagnostics),
            RelayValidationState.Incomplete => RelayValidationResult.Incomplete(persisted.ValidationDiagnostics.ToList()),
            RelayValidationState.RejectedMalformed => RelayValidationResult.RejectedMalformed(
                persisted.ValidationDiagnostics.ToList()),
            _ => RelayValidationResult.Incomplete(persisted.ValidationDiagnostics.ToList()),
        };
}
