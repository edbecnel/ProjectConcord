namespace Edf.Application.Relay.EngineeringAgent;

/// <summary>
/// Static P0 capability declaration for the engineering-agent bridge (PC-PAR-019). No runtime negotiation.
/// </summary>
public sealed record EngineeringAgentRelayBridgeCapabilities(
    bool SupportsManualPaste,
    bool SupportsAutomatedTransport,
    int SupportedRenderVersionMajor);
