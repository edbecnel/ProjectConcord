namespace Edf.Application.Relay.Cursor;

using Edf.Domain.Relay;

/// <summary>
/// Outcome of attempting to prepare a manual Cursor handover artifact.
/// </summary>
public sealed record CursorHandoverPreparationResult(
    bool IsReadyForManualTransfer,
    string? RenderedHandover,
    GovernedRelayPackage? ExportPackage,
    RelayValidationResult Validation);
