namespace Edf.Application.Relay.EngineeringAgent.Plugins;

using Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Outcome of a provider transport cancel request (ADR-0022 §8). Not relay STOP.
/// </summary>
public sealed record EngineeringAgentCancelResult(
    bool IsAcknowledged,
    EngineeringAgentProviderFailure? Failure);
