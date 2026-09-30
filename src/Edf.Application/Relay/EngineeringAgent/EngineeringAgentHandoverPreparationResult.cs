namespace Edf.Application.Relay.EngineeringAgent;

using Edf.Domain.Relay;

/// <summary>
/// Outcome of attempting to prepare a manual engineering-agent handover artifact.
/// </summary>
public sealed record EngineeringAgentHandoverPreparationResult(
    bool IsReadyForManualTransfer,
    string? RenderedHandover,
    GovernedRelayPackage? ExportPackage,
    RelayValidationResult Validation);
