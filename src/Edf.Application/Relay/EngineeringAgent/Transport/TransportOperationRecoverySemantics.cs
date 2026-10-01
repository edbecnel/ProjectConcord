namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Restart-recovery semantics for persisted transport operations (ADR-0022 §7, §10; A4-T4).
/// </summary>
public static class TransportOperationRecoverySemantics
{
    public static bool IsTerminalLifecycleState(TransportOperationLifecycleState state) =>
        state is TransportOperationLifecycleState.ImportCompleted
            or TransportOperationLifecycleState.ImportRejected
            or TransportOperationLifecycleState.Cancelled
            or TransportOperationLifecycleState.Closed
            or TransportOperationLifecycleState.ForwardFailed;

    public static bool RequiresRecoveryConsideration(TransportOperationLifecycleState state) =>
        state is TransportOperationLifecycleState.CreatedNotForwarded
            or TransportOperationLifecycleState.ForwardInProgress
            or TransportOperationLifecycleState.ForwardAcknowledged
            or TransportOperationLifecycleState.AwaitingResult
            or TransportOperationLifecycleState.ResultCandidateReceived
            or TransportOperationLifecycleState.Ambiguous
            or TransportOperationLifecycleState.TimedOut;

    public static EngineeringAgentTransportRecoveryDisposition ClassifyDisposition(
        TransportOperation operation,
        bool providerRegistered,
        bool providerEnabledForProject)
    {
        if (IsTerminalLifecycleState(operation.LifecycleState))
        {
            return EngineeringAgentTransportRecoveryDisposition.NoActionRequired;
        }

        if (!providerRegistered)
        {
            return EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable;
        }

        if (!providerEnabledForProject)
        {
            return EngineeringAgentTransportRecoveryDisposition.ProviderUnavailable;
        }

        return operation.LifecycleState switch
        {
            TransportOperationLifecycleState.CreatedNotForwarded =>
                EngineeringAgentTransportRecoveryDisposition.SafeToResumeBeforeDispatch,
            TransportOperationLifecycleState.ResultCandidateReceived =>
                EngineeringAgentTransportRecoveryDisposition.ResultCandidateRequiresConfirmation,
            TransportOperationLifecycleState.Ambiguous =>
                EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision,
            TransportOperationLifecycleState.TimedOut =>
                EngineeringAgentTransportRecoveryDisposition.AmbiguousRequiresOperatorDecision,
            TransportOperationLifecycleState.ForwardInProgress
                or TransportOperationLifecycleState.ForwardAcknowledged
                or TransportOperationLifecycleState.AwaitingResult =>
                EngineeringAgentTransportRecoveryDisposition.ProviderReconciliationRequired,
            _ => EngineeringAgentTransportRecoveryDisposition.NoActionRequired,
        };
    }
}
