namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Domain.Relay;

/// <summary>
/// Outcome of an explicit recovery action (resume, reconcile, confirmed import) (A4-T4).
/// </summary>
public sealed record EngineeringAgentTransportRecoveryActionResult(
    bool Succeeded,
    TransportOperation? Operation,
    EngineeringAgentTransportRecoveryDisposition Disposition,
    EngineeringAgentAutomatedTransportOutcome? OrchestrationOutcome,
    string? Detail,
    RelayValidationResult? ImportValidation);
