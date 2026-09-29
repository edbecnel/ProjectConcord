namespace Edf.Domain.Relay;

/// <summary>
/// Governed relay package envelope plus explicit validation disposition for operational persistence.
/// </summary>
public sealed record PersistedGovernedRelayPackage(
    GovernedRelayPackage Package,
    RelayValidationState ValidationState,
    IReadOnlyList<RelayValidationDiagnostic> ValidationDiagnostics,
    string? RenderedBodyHash = null);
