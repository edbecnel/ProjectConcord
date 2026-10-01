namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Outcome of provider plugin initialization (ADR-0023 hosting). Distinct from transport forward outcomes.
/// </summary>
public sealed record EngineeringAgentProviderInitializeResult(
    EngineeringAgentProviderHealth Health,
    EngineeringAgentProviderFailure? Failure)
{
    public bool Succeeded => Failure is null && Health.IsInitialized;
}
