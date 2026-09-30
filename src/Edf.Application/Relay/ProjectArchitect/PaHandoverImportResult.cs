namespace Edf.Application.Relay.ProjectArchitect;

using Edf.Domain.Relay;

/// <summary>
/// Parsed manual PA handover import with structural validation outcome.
/// </summary>
public sealed record PaHandoverImportResult(
    GovernedRelayPackage? Package,
    RelayValidationResult Validation);
