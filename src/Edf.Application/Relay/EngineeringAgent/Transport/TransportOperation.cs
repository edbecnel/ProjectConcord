namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Relay;

/// <summary>
/// In-memory transport operation model (ADR-0022 §4–5). Persistence deferred to A4-T2.
/// </summary>
public sealed record TransportOperation(
    TransportOperationId OperationId,
    GovernedPackageId SourcePackageId,
    GovernedCorrelationId CorrelationId,
    EngineeringAgentProviderPluginId ProviderPluginId,
    int Attempt,
    TransportOperationLifecycleState LifecycleState,
    EngineeringAgentProviderSessionHandle? ProviderSessionHint,
    GovernedPackageId? ResultImportPackageId,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc);
