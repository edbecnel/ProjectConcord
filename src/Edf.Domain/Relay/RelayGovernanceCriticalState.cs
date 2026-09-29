namespace Edf.Domain.Relay;

/// <summary>
/// Neutral governance-critical fields (PC-PAR-020) without provider-specific rendered labels.
/// </summary>
public sealed record RelayGovernanceCriticalState(
    EngineeringAgentMode? EngineeringAgentMode,
    EngineeringAgentMode? PriorEngineeringAgentMode,
    EngineeringAgentModeTransition? ModeTransition,
    RelaySessionContinuity SessionContinuity,
    RelayStopMetadata Stop,
    bool AuthorizationDispositionPresent,
    bool WorkContextPresent,
    RelayGovernanceDirectiveFlags DirectiveFlags,
    EdfGovernanceCorrelation? EdfCorrelation);
