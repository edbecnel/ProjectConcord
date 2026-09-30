namespace Edf.Application.Relay.Cursor;

using Edf.Domain.Relay;

/// <summary>
/// Parsed manual engineering result import with validation outcome (external evidence — not governance authority).
/// </summary>
public sealed record EngineeringResultImportResult(
    GovernedRelayPackage? Package,
    RelayValidationResult Validation);
