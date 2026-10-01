namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Provider-neutral hosting boundary for initialization and invocation (ADR-0023 §2). Implementation in A4-T1.
/// </summary>
public interface IEngineeringAgentPluginHost
{
    EngineeringAgentProviderHealth InitializePlugin(EngineeringAgentProviderPluginId pluginId);

    void ShutdownPlugin(EngineeringAgentProviderPluginId pluginId);
}
