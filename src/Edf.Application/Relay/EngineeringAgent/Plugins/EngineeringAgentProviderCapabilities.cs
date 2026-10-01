namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Provider-neutral capability declaration (ADR-0021 §5, ADR-0023 §6). Distinct from relay bridge capabilities.
/// </summary>
public sealed record EngineeringAgentProviderCapabilities(
    bool SupportsAutomatedTransport,
    int SupportedRenderProtocolMajor,
    EngineeringAgentRoutingIntentSupport RoutingIntentSupport,
    bool IsAvailableForSelection)
{
    public bool IsCompatibleWithRenderProtocol(int requiredMajor) =>
        SupportedRenderProtocolMajor == requiredMajor;
}
