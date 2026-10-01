namespace Edf.Application.Operator.Relay;

using Edf.Application.Operator;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Maps Engineering Agent automated transport/hosting signals to Attention (ADR-0022 §14).
/// </summary>
public sealed class EngineeringAgentTransportOperatorAttentionContributor : IOperatorAttentionContributor
{
    private readonly EngineeringAgentPluginSelectionService _selection;
    private readonly ITransportOperationStore _transportStore;

    public EngineeringAgentTransportOperatorAttentionContributor(
        EngineeringAgentPluginSelectionService selection,
        ITransportOperationStore transportStore)
    {
        _selection = selection ?? throw new ArgumentNullException(nameof(selection));
        _transportStore = transportStore ?? throw new ArgumentNullException(nameof(transportStore));
    }

    public IReadOnlyList<OperatorAttentionItem> Contribute(RelayWorkflowOperatorProjectionInput input)
    {
        if (input.ProjectId is not { } projectId)
        {
            return [];
        }

        var items = new List<OperatorAttentionItem>();

        var preflight = _selection.EvaluatePreflightForAutomatedTransport(projectId, input.RoutingIntent);
        if (!preflight.CanProceedToInitialization)
        {
            items.Add(MapPluginUnavailable(preflight.UnavailableReason, preflight.Detail));
        }
        else if (preflight.Compatibility is not null
                 && (!preflight.Compatibility.HostContract.IsCompatible
                     || !preflight.Compatibility.RenderProtocol.IsCompatible
                     || !preflight.Compatibility.RoutingIntent.IsCompatible))
        {
            items.Add(new OperatorAttentionItem(
                OperatorAttentionCodes.EngineeringAgentPluginIncompatible,
                OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                "Selected Engineering Agent provider plugin is incompatible with this Project or routing intent.",
                IsBlocking: false,
                OperatorAttentionUrgency.Recommended,
                preflight.Compatibility.HostContract.Reason
                    ?? preflight.Compatibility.RenderProtocol.Reason
                    ?? preflight.Compatibility.RoutingIntent.Reason));
        }
        else
        {
            var readiness = _selection.EvaluateRuntimeReadinessForAutomatedTransport(projectId, input.RoutingIntent);
            if (!readiness.CanUseForAutomatedTransport)
            {
                items.Add(MapRuntimeUnavailable(readiness));
            }
        }

        if (input.LastAutomatedTransportResult is { } last)
        {
            items.AddRange(MapTransportResult(last));
        }

        foreach (var operation in _transportStore.GetRecoverableOperations(projectId))
        {
            if (operation.LifecycleState == TransportOperationLifecycleState.Ambiguous
                || operation.LifecycleState == TransportOperationLifecycleState.ForwardInProgress
                || operation.LifecycleState == TransportOperationLifecycleState.ResultCandidateReceived)
            {
                items.Add(new OperatorAttentionItem(
                    OperatorAttentionCodes.EngineeringAgentRecoverableTransport,
                    OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                    $"Recoverable automated transport operation ({operation.LifecycleState}) requires explicit operator follow-up.",
                    IsBlocking: false,
                    OperatorAttentionUrgency.Recommended,
                    $"TransportOperationId: {operation.OperationId}"));
            }
        }

        return DeduplicateByCode(items);
    }

    private static OperatorAttentionItem MapPluginUnavailable(
        EngineeringAgentPluginSelectionUnavailableReason reason,
        string? detail) =>
        new(
            reason is EngineeringAgentPluginSelectionUnavailableReason.HostContractIncompatible
                or EngineeringAgentPluginSelectionUnavailableReason.RenderProtocolIncompatible
                or EngineeringAgentPluginSelectionUnavailableReason.RoutingIntentUnsupported
                ? OperatorAttentionCodes.EngineeringAgentPluginIncompatible
                : OperatorAttentionCodes.EngineeringAgentPluginUnavailable,
            OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
            reason switch
            {
                EngineeringAgentPluginSelectionUnavailableReason.NoProviderSelected =>
                    "No Engineering Agent provider plugin is selected for automated transport.",
                EngineeringAgentPluginSelectionUnavailableReason.ProviderNotRegistered =>
                    "Selected Engineering Agent provider plugin is not registered.",
                EngineeringAgentPluginSelectionUnavailableReason.ProviderDisabled =>
                    "Selected Engineering Agent provider plugin is disabled for this Project.",
                EngineeringAgentPluginSelectionUnavailableReason.HostContractIncompatible
                or EngineeringAgentPluginSelectionUnavailableReason.RenderProtocolIncompatible
                or EngineeringAgentPluginSelectionUnavailableReason.RoutingIntentUnsupported =>
                    "Selected Engineering Agent provider plugin is incompatible.",
                _ => "Engineering Agent automated transport provider is unavailable.",
            },
            IsBlocking: false,
            OperatorAttentionUrgency.Recommended,
            detail);

    private static OperatorAttentionItem MapRuntimeUnavailable(EngineeringAgentPluginSelectionEvaluation readiness)
    {
        if (readiness.UnavailableReason == EngineeringAgentPluginSelectionUnavailableReason.PluginUnhealthy
            && readiness.Health is { IsAuthenticated: false })
        {
            return new OperatorAttentionItem(
                OperatorAttentionCodes.EngineeringAgentAuthenticationFailure,
                OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                "Engineering Agent provider authentication is unavailable.",
                IsBlocking: false,
                OperatorAttentionUrgency.Recommended,
                readiness.Detail);
        }

        if (readiness.UnavailableReason == EngineeringAgentPluginSelectionUnavailableReason.PluginNotInitialized)
        {
            return new OperatorAttentionItem(
                OperatorAttentionCodes.EngineeringAgentInitializationFailure,
                OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                "Engineering Agent provider has not completed initialization for automated transport.",
                IsBlocking: false,
                OperatorAttentionUrgency.Recommended,
                readiness.Detail);
        }

        return MapPluginUnavailable(readiness.UnavailableReason, readiness.Detail);
    }

    private static IEnumerable<OperatorAttentionItem> MapTransportResult(EngineeringAgentAutomatedTransportResult result)
    {
        if (result.ProviderFailure is { } failure)
        {
            yield return MapProviderFailure(failure, result.Detail);
            yield break;
        }

        switch (result.Outcome)
        {
            case EngineeringAgentAutomatedTransportOutcome.ProviderUnavailable:
                yield return new OperatorAttentionItem(
                    result.Detail?.Contains("initialization", StringComparison.OrdinalIgnoreCase) == true
                        ? OperatorAttentionCodes.EngineeringAgentInitializationFailure
                        : OperatorAttentionCodes.EngineeringAgentPluginUnavailable,
                    OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                    result.Detail ?? "Engineering Agent automated transport provider is unavailable.",
                    false,
                    OperatorAttentionUrgency.Recommended,
                    result.Detail);
                break;
            case EngineeringAgentAutomatedTransportOutcome.PersistenceFailed:
                yield return new OperatorAttentionItem(
                    OperatorAttentionCodes.EngineeringAgentTransportPersistence,
                    OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                    "Automated transport persistence failed.",
                    false,
                    OperatorAttentionUrgency.Recommended,
                    result.Detail);
                break;
            case EngineeringAgentAutomatedTransportOutcome.ForwardFailed:
                yield return new OperatorAttentionItem(
                    OperatorAttentionCodes.EngineeringAgentForwardFailure,
                    OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                    "Automated forward to the Engineering Agent provider failed.",
                    false,
                    OperatorAttentionUrgency.Recommended,
                    result.Detail);
                break;
            case EngineeringAgentAutomatedTransportOutcome.Ambiguous:
            case EngineeringAgentAutomatedTransportOutcome.CancelAmbiguous:
                yield return new OperatorAttentionItem(
                    OperatorAttentionCodes.EngineeringAgentAmbiguousTransport,
                    OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                    "Automated transport state is ambiguous and requires operator review.",
                    false,
                    OperatorAttentionUrgency.Recommended,
                    result.Detail);
                break;
            case EngineeringAgentAutomatedTransportOutcome.ImportRejected:
                yield return new OperatorAttentionItem(
                    OperatorAttentionCodes.EngineeringAgentResultRetrievalFailure,
                    OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
                    "Automated result candidate could not be imported under governed relay rules.",
                    false,
                    OperatorAttentionUrgency.Recommended,
                    result.Detail);
                break;
        }
    }

    private static OperatorAttentionItem MapProviderFailure(EngineeringAgentProviderFailure failure, string? detail)
    {
        var code = failure.Kind switch
        {
            EngineeringAgentProviderFailureKind.Incompatible
                or EngineeringAgentProviderFailureKind.RoutingIntentUnsupported
                => OperatorAttentionCodes.EngineeringAgentPluginIncompatible,
            EngineeringAgentProviderFailureKind.AuthenticationUnavailable
                => OperatorAttentionCodes.EngineeringAgentAuthenticationFailure,
            EngineeringAgentProviderFailureKind.InitializationFailed
                => OperatorAttentionCodes.EngineeringAgentInitializationFailure,
            EngineeringAgentProviderFailureKind.ForwardFailed
                => OperatorAttentionCodes.EngineeringAgentForwardFailure,
            EngineeringAgentProviderFailureKind.ResultRetrievalFailed
                => OperatorAttentionCodes.EngineeringAgentResultRetrievalFailure,
            EngineeringAgentProviderFailureKind.AmbiguousOutcome
                => OperatorAttentionCodes.EngineeringAgentAmbiguousTransport,
            _ => OperatorAttentionCodes.EngineeringAgentPluginUnavailable,
        };

        return new OperatorAttentionItem(
            code,
            OperatorAttentionSourceDomain.EngineeringAgentAutomatedTransport,
            failure.Message,
            false,
            OperatorAttentionUrgency.Recommended,
            detail);
    }

    private static List<OperatorAttentionItem> DeduplicateByCode(List<OperatorAttentionItem> items)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var result = new List<OperatorAttentionItem>();
        foreach (var item in items)
        {
            if (seen.Add(item.Code))
            {
                result.Add(item);
            }
        }

        return result;
    }
}
