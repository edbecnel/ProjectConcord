namespace Edf.Application.Relay.EngineeringAgent.Plugins;

using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

/// <summary>
/// Neutral input for automated forward after governed render and produce (ADR-0022 §2).
/// </summary>
public sealed record EngineeringAgentForwardRequest(
    TransportOperationId TransportOperationId,
    GovernedPackageId SourcePackageId,
    GovernedCorrelationId CorrelationId,
    EngineeringAgentMode RoutingIntent,
    string RenderedHandoverBody,
    RelaySessionContinuity SessionContinuity,
    EngineeringAgentProviderSessionHandle? ProviderSessionHint);
