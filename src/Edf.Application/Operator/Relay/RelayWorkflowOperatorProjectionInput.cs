namespace Edf.Application.Operator.Relay;

using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Authoritative inputs for relay-workflow operator projections. Snapshot only — not persisted.
/// </summary>
public sealed record RelayWorkflowOperatorProjectionInput(
    ProjectConcordProjectId? ProjectId,
    EngineeringAgentMode RoutingIntent,
    GovernedRelayPackage? ImportedPaHandoverPackage,
    RelayValidationResult? ImportedPaHandoverValidation,
    EngineeringAgentAutomatedTransportResult? LastAutomatedTransportResult);
