namespace Edf.Application.Relay.EngineeringAgent.Plugins;

using Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Outcome of provider forward attempt. Does not imply governance acceptance.
/// </summary>
public sealed record EngineeringAgentForwardResult(
    bool IsAcknowledged,
    EngineeringAgentProviderSessionHandle? UpdatedSessionHint,
    EngineeringAgentProviderFailure? Failure);
