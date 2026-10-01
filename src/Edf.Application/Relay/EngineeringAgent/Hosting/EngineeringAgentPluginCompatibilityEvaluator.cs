namespace Edf.Application.Relay.EngineeringAgent.Hosting;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Relay;

public sealed record EngineeringAgentPluginHostContractCompatibility(
    bool IsCompatible,
    string? Reason);

public sealed record EngineeringAgentPluginRenderProtocolCompatibility(
    bool IsCompatible,
    string? Reason);

public sealed record EngineeringAgentPluginRoutingCompatibility(
    bool IsCompatible,
    string? Reason);

public sealed record EngineeringAgentPluginCompatibilityAssessment(
    EngineeringAgentPluginHostContractCompatibility HostContract,
    EngineeringAgentPluginRenderProtocolCompatibility RenderProtocol,
    EngineeringAgentPluginRoutingCompatibility RoutingIntent)
{
    public bool IsFullyCompatible =>
        HostContract.IsCompatible && RenderProtocol.IsCompatible && RoutingIntent.IsCompatible;
}

public static class EngineeringAgentPluginCompatibilityEvaluator
{
    public static EngineeringAgentPluginCompatibilityAssessment Assess(
        IEngineeringAgentProviderPlugin plugin,
        int requiredRenderProtocolMajor,
        EngineeringAgentMode routingIntent)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        var capabilities = plugin.DeclareCapabilities();

        var host = AssessHostContract(capabilities);
        var render = AssessRenderProtocol(capabilities, requiredRenderProtocolMajor);
        var routing = AssessRoutingIntent(capabilities, routingIntent);

        return new EngineeringAgentPluginCompatibilityAssessment(host, render, routing);
    }

    private static EngineeringAgentPluginHostContractCompatibility AssessHostContract(
        EngineeringAgentProviderCapabilities capabilities)
    {
        if (!capabilities.SupportsAutomatedTransport)
        {
            return new EngineeringAgentPluginHostContractCompatibility(
                false,
                "Plugin does not declare automated transport support.");
        }

        if (!capabilities.IsAvailableForSelection)
        {
            return new EngineeringAgentPluginHostContractCompatibility(
                false,
                "Plugin is not available for selection.");
        }

        return new EngineeringAgentPluginHostContractCompatibility(true, null);
    }

    private static EngineeringAgentPluginRenderProtocolCompatibility AssessRenderProtocol(
        EngineeringAgentProviderCapabilities capabilities,
        int requiredRenderProtocolMajor)
    {
        if (!capabilities.IsCompatibleWithRenderProtocol(requiredRenderProtocolMajor))
        {
            return new EngineeringAgentPluginRenderProtocolCompatibility(
                false,
                $"Plugin render protocol major {capabilities.SupportedRenderProtocolMajor} is incompatible with required major {requiredRenderProtocolMajor}.");
        }

        return new EngineeringAgentPluginRenderProtocolCompatibility(true, null);
    }

    private static EngineeringAgentPluginRoutingCompatibility AssessRoutingIntent(
        EngineeringAgentProviderCapabilities capabilities,
        EngineeringAgentMode routingIntent)
    {
        if (!capabilities.RoutingIntentSupport.SupportsRoutingIntent(routingIntent))
        {
            return new EngineeringAgentPluginRoutingCompatibility(
                false,
                $"Plugin does not declare semantically adequate support for routing intent {routingIntent}.");
        }

        return new EngineeringAgentPluginRoutingCompatibility(true, null);
    }
}
