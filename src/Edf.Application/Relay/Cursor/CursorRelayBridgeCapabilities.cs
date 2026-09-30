namespace Edf.Application.Relay.Cursor;

/// <summary>
/// Static P0 capability declaration for the Cursor bridge (PC-PAR-019). No runtime negotiation.
/// </summary>
public sealed record CursorRelayBridgeCapabilities(
    bool SupportsManualPaste,
    bool SupportsAutomatedTransport,
    int SupportedRenderVersionMajor);
