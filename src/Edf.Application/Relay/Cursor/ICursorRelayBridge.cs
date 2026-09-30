namespace Edf.Application.Relay.Cursor;

using Edf.Domain.Relay;

/// <summary>
/// Engineering-agent / Cursor transport boundary (PC-PAR-022). P0 manual copy/paste only — no automated IDE transport.
/// </summary>
public interface ICursorRelayBridge
{
    CursorRelayBridgeCapabilities DeclareCapabilities();

    /// <summary>
    /// Produces Cursor-directed Markdown from a governed package only when boundary validation is
    /// <see cref="RelayValidationState.Valid"/> per <see cref="RelayValidatedHandoverEligibility"/>.
    /// </summary>
    CursorHandoverPreparationResult TryRenderValidatedHandover(
        GovernedRelayPackage package,
        RelayValidationResult boundaryValidation);

    /// <summary>
    /// Parses optional thin engineering result / evidence pasted back from Cursor (untrusted input).
    /// </summary>
    EngineeringResultImportResult TryParseEngineeringResult(string renderedText);
}
