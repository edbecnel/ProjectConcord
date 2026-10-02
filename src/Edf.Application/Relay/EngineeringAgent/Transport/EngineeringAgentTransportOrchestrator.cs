namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Application.Projects;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Single-operation automated transport orchestration (ADR-0022). Not restart recovery (A4-T4).
/// </summary>
public sealed class EngineeringAgentTransportOrchestrator : IEngineeringAgentTransportOrchestrator
{
    private readonly IUserApplicationStatePersistence _persistence;
    private readonly IGovernedRelayP0WorkflowService _workflow;
    private readonly IEngineeringAgentPluginCatalog _catalog;
    private readonly EngineeringAgentPluginHost _host;
    private readonly EngineeringAgentPluginSelectionService _selection;
    private readonly TimeProvider _clock;

    public EngineeringAgentTransportOrchestrator(
        IUserApplicationStatePersistence persistence,
        IGovernedRelayP0WorkflowService workflow,
        IEngineeringAgentPluginCatalog catalog,
        EngineeringAgentPluginHost host,
        EngineeringAgentPluginSelectionService selection,
        TimeProvider? clock = null)
    {
        _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
        _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        _clock = clock ?? TimeProvider.System;
    }

    public async Task<EngineeringAgentAutomatedTransportResult> ForwardGovernedHandoverAsync(
        EngineeringAgentAutomatedForwardRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.GovernedProjectRoot.NormalizedAbsolutePath))
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible,
                null,
                "Governed Project Root is required for automated transport.",
                null,
                null);
        }

        var persistedSource = _persistence.RelayOperational.GetPackage(request.SourcePackageId);
        if (persistedSource is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible,
                null,
                "Source governed package was not found in operational persistence.",
                null,
                null);
        }

        if (persistedSource.Package.ProjectId != request.ProjectId)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible,
                null,
                "Source package project id does not match the active ProjectConcord project.",
                null,
                null);
        }

        var boundaryValidation = ToValidationResult(persistedSource);
        if (!boundaryValidation.IsEligibleForValidatedEngineeringAgentHandover)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible,
                null,
                "Source package is not eligible for validated Engineering Agent handover.",
                boundaryValidation,
                null);
        }

        var preparation = _workflow.PrepareEngineeringAgentHandover(
            persistedSource.Package,
            boundaryValidation);
        if (!preparation.IsReadyForManualTransfer
            || preparation.RenderedHandover is null
            || preparation.ExportPackage is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible,
                null,
                "Governed handover preparation did not produce a renderable export.",
                preparation.Validation,
                null);
        }

        var preflight = _selection.EvaluatePreflightForAutomatedTransport(
            request.ProjectId,
            request.RoutingIntent);
        if (!preflight.CanProceedToInitialization
            || preflight.SelectedPluginId is null
            || preflight.ResolvedPlugin is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable,
                null,
                preflight.Detail ?? "No automated Engineering Agent provider is available.",
                preparation.Validation,
                null);
        }

        var selectedPluginId = preflight.SelectedPluginId.Value;
        if (!_host.IsInitialized(selectedPluginId))
        {
            var init = await _host.InitializePluginAsync(selectedPluginId, cancellationToken)
                .ConfigureAwait(false);
            if (!init.Succeeded)
            {
                return new EngineeringAgentAutomatedTransportResult(
                    EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable,
                    null,
                    init.Failure?.Message ?? "Provider plugin initialization failed.",
                    preparation.Validation,
                    init.Failure);
            }
        }

        var readiness = _selection.EvaluateRuntimeReadinessForAutomatedTransport(
            request.ProjectId,
            request.RoutingIntent);
        if (!readiness.CanUseForAutomatedTransport
            || readiness.ResolvedPlugin is null
            || readiness.SelectedPluginId is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable,
                null,
                readiness.Detail ?? "Selected Engineering Agent provider is not ready for automated transport.",
                preparation.Validation,
                null);
        }

        var selection = readiness;
        var store = _persistence.TransportOperations;
        var utc = _clock.GetUtcNow();
        var operationId = CreateDistinctOperationId(
            persistedSource.Package.PackageId,
            persistedSource.Package.CorrelationId);

        var created = new TransportOperation(
            operationId,
            persistedSource.Package.PackageId,
            persistedSource.Package.CorrelationId,
            selection.SelectedPluginId.Value,
            Attempt: 1,
            TransportOperationLifecycleState.CreatedNotForwarded,
            ProviderSessionHint: null,
            ResultImportPackageId: null,
            CreatedUtc: utc,
            UpdatedUtc: utc);

        try
        {
            store.Save(created);
        }
        catch (Exception ex)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.PersistenceFailed,
                null,
                ex.Message,
                preparation.Validation,
                null);
        }

        if (!TransportOperationPreDispatchPersistence.TryPersistForwardInProgress(
                store,
                created,
                _clock,
                out var inProgress,
                out var preDispatchFailure))
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.PersistenceFailed,
                null,
                preDispatchFailure?.Message ?? "Failed to persist pre-dispatch transport state.",
                preparation.Validation,
                null);
        }

        var continuity = _persistence.RelayOperational.GetSessionContinuity(request.ProjectId);
        var executionPrompt = GovernedRelayAutomatedExecutionPrompt.Compose(
            preparation.RenderedHandover,
            preparation.ExportPackage);
        var forwardRequest = new EngineeringAgentForwardRequest(
            request.ProjectId,
            operationId,
            persistedSource.Package.PackageId,
            persistedSource.Package.CorrelationId,
            request.RoutingIntent,
            executionPrompt,
            continuity,
            ProviderSessionHint: null,
            request.GovernedProjectRoot);

        EngineeringAgentForwardResult forwardResult;
        try
        {
            forwardResult = selection.ResolvedPlugin.Forward(forwardRequest, cancellationToken);
        }
        catch (Exception ex)
        {
            var failed = inProgress with
            {
                LifecycleState = TransportOperationLifecycleState.Ambiguous,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(failed);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.Ambiguous,
                failed,
                ex.Message,
                preparation.Validation,
                null);
        }

        if (forwardResult.Failure is not null)
        {
            var lifecycle = forwardResult.Failure.Kind == EngineeringAgentProviderFailureKind.AmbiguousOutcome
                ? TransportOperationLifecycleState.Ambiguous
                : TransportOperationLifecycleState.ForwardFailed;
            var failed = inProgress with
            {
                LifecycleState = lifecycle,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(failed);
            var outcome = lifecycle == TransportOperationLifecycleState.Ambiguous
                ? EngineeringAgentAutomatedTransportOutcome.Ambiguous
                : EngineeringAgentAutomatedTransportOutcome.ForwardFailed;
            return new EngineeringAgentAutomatedTransportResult(
                outcome,
                failed,
                forwardResult.Failure.Message,
                preparation.Validation,
                forwardResult.Failure);
        }

        if (!forwardResult.IsAcknowledged)
        {
            var failed = inProgress with
            {
                LifecycleState = TransportOperationLifecycleState.ForwardFailed,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(failed);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ForwardFailed,
                failed,
                "Provider did not acknowledge the forward request.",
                preparation.Validation,
                null);
        }

        var acknowledged = inProgress with
        {
            LifecycleState = TransportOperationLifecycleState.ForwardAcknowledged,
            ProviderSessionHint = forwardResult.UpdatedSessionHint,
            UpdatedUtc = _clock.GetUtcNow(),
        };
        store.Save(acknowledged);

        var candidateResult = selection.ResolvedPlugin.TryGetResultCandidate(operationId, cancellationToken);
        if (candidateResult.Failure?.Kind == EngineeringAgentProviderFailureKind.AmbiguousOutcome)
        {
            var ambiguous = acknowledged with
            {
                LifecycleState = TransportOperationLifecycleState.Ambiguous,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(ambiguous);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.Ambiguous,
                ambiguous,
                candidateResult.Failure.Message,
                preparation.Validation,
                candidateResult.Failure);
        }

        if (!candidateResult.HasCandidate || candidateResult.Candidate is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.Dispatched,
                acknowledged,
                null,
                preparation.Validation,
                null);
        }

        var withCandidate = acknowledged with
        {
            LifecycleState = TransportOperationLifecycleState.ResultCandidateReceived,
            UpdatedUtc = _clock.GetUtcNow(),
        };
        store.Save(withCandidate);

        if (!GovernedRelayAutomatedResultDocumentExtractor.TryExtract(
                candidateResult.Candidate.UntrustedImportText,
                out var extractedImportText,
                out var extractionFailure))
        {
            var rejectedExtraction = withCandidate with
            {
                LifecycleState = TransportOperationLifecycleState.ImportRejected,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(rejectedExtraction);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ImportRejected,
                rejectedExtraction,
                extractionFailure.Diagnostics.FirstOrDefault()?.Message
                    ?? "Automated transport could not extract a governed relay document from the provider response.",
                extractionFailure,
                null);
        }

        var importOperation = _workflow.ImportEngineeringResult(
            request.ProjectId,
            extractedImportText);

        if (importOperation.Import.Package is null
            || importOperation.Import.Validation.State != RelayValidationState.Valid
            || !importOperation.ProjectIdMatched)
        {
            var rejected = withCandidate with
            {
                LifecycleState = TransportOperationLifecycleState.ImportRejected,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(rejected);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ImportRejected,
                rejected,
                importOperation.ProjectIdMismatchMessage
                    ?? "Engineering result import boundary rejected the provider candidate.",
                importOperation.Import.Validation,
                null);
        }

        var completed = withCandidate with
        {
            LifecycleState = TransportOperationLifecycleState.ImportCompleted,
            ResultImportPackageId = importOperation.Import.Package.PackageId,
            UpdatedUtc = _clock.GetUtcNow(),
        };
        store.Save(completed);

        return new EngineeringAgentAutomatedTransportResult(
            EngineeringAgentAutomatedTransportOutcome.ImportCompleted,
            completed,
            null,
            importOperation.Import.Validation,
            null);
    }

    public async Task<EngineeringAgentAutomatedTransportResult> CancelTransportOperationAsync(
        EngineeringAgentAutomatedCancelRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var store = _persistence.TransportOperations;
        var operation = store.Get(request.TransportOperationId);
        if (operation is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.Unavailable,
                null,
                "Transport operation was not found.",
                null,
                null);
        }

        var persistedSource = _persistence.RelayOperational.GetPackage(operation.SourcePackageId);
        if (persistedSource is not null && persistedSource.Package.ProjectId != request.ProjectId)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible,
                operation,
                "Transport operation source package project id does not match the active project.",
                null,
                null);
        }

        if (!_catalog.TryGetPlugin(operation.ProviderPluginId, out var plugin) || plugin is null)
        {
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable,
                operation,
                "Provider plugin is not registered in the catalog.",
                null,
                null);
        }

        // Cancel addresses the operation's stored provider only (no selection/preflight). Initialize host
        // lifecycle for that plugin when required so Cancel can reach provider runtime; session hint remains
        // on the persisted TransportOperation and is not substituted across providers.
        if (!_host.IsInitialized(operation.ProviderPluginId))
        {
            var init = await _host.InitializePluginAsync(operation.ProviderPluginId, cancellationToken)
                .ConfigureAwait(false);
            if (!init.Succeeded)
            {
                return new EngineeringAgentAutomatedTransportResult(
                    EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable,
                    operation,
                    init.Failure?.Message ?? "Provider plugin initialization failed.",
                    null,
                    init.Failure);
            }
        }

        var cancelResult = plugin.Cancel(request.TransportOperationId, cancellationToken);
        if (cancelResult.Failure?.Kind == EngineeringAgentProviderFailureKind.AmbiguousOutcome)
        {
            var ambiguous = operation with
            {
                LifecycleState = TransportOperationLifecycleState.Ambiguous,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(ambiguous);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.CancelAmbiguous,
                ambiguous,
                cancelResult.Failure.Message,
                null,
                cancelResult.Failure);
        }

        if (!cancelResult.IsAcknowledged || cancelResult.Failure is not null)
        {
            var failed = operation with
            {
                LifecycleState = TransportOperationLifecycleState.ForwardFailed,
                UpdatedUtc = _clock.GetUtcNow(),
            };
            store.Save(failed);
            return new EngineeringAgentAutomatedTransportResult(
                EngineeringAgentAutomatedTransportOutcome.ForwardFailed,
                failed,
                cancelResult.Failure?.Message ?? "Provider did not acknowledge cancellation.",
                null,
                cancelResult.Failure);
        }

        var cancelled = operation with
        {
            LifecycleState = TransportOperationLifecycleState.Cancelled,
            UpdatedUtc = _clock.GetUtcNow(),
        };
        store.Save(cancelled);

        return new EngineeringAgentAutomatedTransportResult(
            EngineeringAgentAutomatedTransportOutcome.Cancelled,
            cancelled,
            null,
            null,
            null);
    }

    private static TransportOperationId CreateDistinctOperationId(
        GovernedPackageId sourcePackageId,
        GovernedCorrelationId correlationId)
    {
        for (var attempt = 0; attempt < 8; attempt++)
        {
            var id = TransportOperationId.New();
            if (TransportOperationIdentityRules.SharesSameGuidValue(id, sourcePackageId)
                || TransportOperationIdentityRules.SharesSameGuidValue(id, correlationId))
            {
                continue;
            }

            return id;
        }

        throw new InvalidOperationException("Unable to allocate a TransportOperationId distinct from governed identities.");
    }

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
