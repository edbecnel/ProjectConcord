namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Neutral automated-transport outcome classification (A4-T3). Not UI-specific.
/// </summary>
public enum EngineeringAgentAutomatedTransportOutcome
{
    Unavailable = 0,
    GovernanceIneligible = 1,
    ProviderUnavailable = 2,
    PersistenceFailed = 3,
    ForwardFailed = 4,
    Dispatched = 5,
    ResultCandidateReceived = 6,
    ImportCompleted = 7,
    ImportRejected = 8,
    Ambiguous = 9,
    Cancelled = 10,
    CancelAmbiguous = 11,
}
