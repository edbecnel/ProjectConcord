using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

internal sealed class FakeEngineeringAgentProviderPlugin : IEngineeringAgentProviderPlugin
{
    public FakeEngineeringAgentProviderPlugin(
        EngineeringAgentProviderPluginId pluginId,
        EngineeringAgentProviderCapabilities capabilities)
    {
        PluginId = pluginId;
        _capabilities = capabilities;
    }

    private readonly EngineeringAgentProviderCapabilities _capabilities;

    public EngineeringAgentProviderPluginId PluginId { get; }

    public EngineeringAgentProviderCapabilities DeclareCapabilities() => _capabilities;

    public EngineeringAgentProviderHealth GetHealth() =>
        new(IsInitialized: true, IsAuthenticated: true, StatusMessage: null);

    public EngineeringAgentForwardResult Forward(
        EngineeringAgentForwardRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        return new EngineeringAgentForwardResult(
            IsAcknowledged: true,
            UpdatedSessionHint: EngineeringAgentProviderSessionHandle.FromOpaque("fake-session"),
            Failure: null);
    }

    public EngineeringAgentResultCandidateResult TryGetResultCandidate(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        return new EngineeringAgentResultCandidateResult(
            HasCandidate: false,
            Candidate: null,
            Failure: null);
    }

    public EngineeringAgentCancelResult Cancel(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        return new EngineeringAgentCancelResult(IsAcknowledged: true, Failure: null);
    }
}
