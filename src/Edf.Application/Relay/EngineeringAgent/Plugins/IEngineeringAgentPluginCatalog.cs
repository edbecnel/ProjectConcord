namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Explicit plugin catalog (ADR-0023 §3). Implementation in A4-T1.
/// </summary>
public interface IEngineeringAgentPluginCatalog
{
    IReadOnlyList<EngineeringAgentProviderPluginId> RegisteredPluginIds { get; }

    bool TryGetPlugin(EngineeringAgentProviderPluginId pluginId, out IEngineeringAgentProviderPlugin? plugin);
}
