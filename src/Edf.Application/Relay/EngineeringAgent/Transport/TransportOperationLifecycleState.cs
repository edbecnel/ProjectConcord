namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Semantic transport-operation lifecycle distinctions (ADR-0022 §5). Operational state — not package validation state.
/// </summary>
public enum TransportOperationLifecycleState
{
    CreatedNotForwarded = 0,
    ForwardInProgress = 1,
    ForwardAcknowledged = 2,
    ForwardFailed = 3,
    AwaitingResult = 4,
    ResultCandidateReceived = 5,
    ImportCompleted = 6,
    ImportRejected = 7,
    Cancelled = 8,
    TimedOut = 9,
    Ambiguous = 10,
    Closed = 11,
}
