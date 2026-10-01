namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Neutral failure report. Does not mutate governed package validation state.
/// </summary>
public sealed record EngineeringAgentProviderFailure(
    EngineeringAgentProviderFailureKind Kind,
    string Message,
    bool IsRetryEligible = false);
