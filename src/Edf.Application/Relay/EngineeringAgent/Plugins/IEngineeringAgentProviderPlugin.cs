namespace Edf.Application.Relay.EngineeringAgent.Plugins;

using Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Provider-neutral automated transport contract (ADR-0021 §2). Product-specific mechanics stay inside implementations.
/// </summary>
public interface IEngineeringAgentProviderPlugin
{
    EngineeringAgentProviderPluginId PluginId { get; }

    EngineeringAgentProviderCapabilities DeclareCapabilities();

    EngineeringAgentProviderHealth GetHealth();

    EngineeringAgentForwardResult Forward(
        EngineeringAgentForwardRequest request,
        CancellationToken cancellationToken = default);

    EngineeringAgentResultCandidateResult TryGetResultCandidate(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default);

    EngineeringAgentCancelResult Cancel(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default);
}
