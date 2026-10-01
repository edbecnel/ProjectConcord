namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Neutral restart-recovery disposition (A4-T4). Not UI-specific.
/// </summary>
public enum EngineeringAgentTransportRecoveryDisposition
{
    NoActionRequired = 0,
    SafeToResumeBeforeDispatch = 1,
    ProviderReconciliationRequired = 2,
    AmbiguousRequiresOperatorDecision = 3,
    ResultCandidateRequiresConfirmation = 4,
    ProviderUnavailable = 5,
    RecoveryFailed = 6,
}
