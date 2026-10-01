namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Outcome of provider plugin shutdown (ADR-0023 hosting).
/// </summary>
public sealed record EngineeringAgentProviderShutdownResult(
    bool IsAcknowledged,
    EngineeringAgentProviderFailure? Failure);
