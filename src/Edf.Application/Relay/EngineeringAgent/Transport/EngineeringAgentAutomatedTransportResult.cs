namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Neutral orchestration result for one automated transport operation (A4-T3).
/// </summary>
public sealed record EngineeringAgentAutomatedTransportResult(
    EngineeringAgentAutomatedTransportOutcome Outcome,
    TransportOperation? Operation,
    string? Detail,
    RelayValidationResult? GovernanceValidation,
    EngineeringAgentProviderFailure? ProviderFailure);
