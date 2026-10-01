namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Relay;

/// <summary>
/// Transport operation operational model (ADR-0022 §4–5). Persisted via <see cref="ITransportOperationStore"/> (A4-T2).
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
