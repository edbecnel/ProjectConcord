namespace Edf.Application.Relay.EngineeringAgent.Hosting;

using Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Explicit static plugin catalog (ADR-0023 §3). No discovery or dynamic loading.
/// </summary>
public sealed class EngineeringAgentPluginCatalog : IEngineeringAgentPluginCatalog
{
    private readonly Dictionary<EngineeringAgentProviderPluginId, IEngineeringAgentProviderPlugin> _plugins;

    private EngineeringAgentPluginCatalog(Dictionary<EngineeringAgentProviderPluginId, IEngineeringAgentProviderPlugin> plugins)
    {
        _plugins = plugins;
    }

    public static EngineeringAgentPluginCatalog CreateEmpty() =>
        new(new Dictionary<EngineeringAgentProviderPluginId, IEngineeringAgentProviderPlugin>());

    public static EngineeringAgentPluginCatalog FromRegistrations(IEnumerable<IEngineeringAgentProviderPlugin> plugins)
    {
        ArgumentNullException.ThrowIfNull(plugins);
        var builder = CreateEmpty();
        foreach (var plugin in plugins)
        {
            builder = builder.Register(plugin);
        }

        return builder;
    }

    public EngineeringAgentPluginCatalog Register(IEngineeringAgentProviderPlugin plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        var copy = new Dictionary<EngineeringAgentProviderPluginId, IEngineeringAgentProviderPlugin>(_plugins);
        if (copy.ContainsKey(plugin.PluginId))
        {
            throw new EngineeringAgentPluginCatalogDuplicateIdentityException(plugin.PluginId);
        }

        copy[plugin.PluginId] = plugin;
        return new EngineeringAgentPluginCatalog(copy);
    }

    public IReadOnlyList<EngineeringAgentProviderPluginId> RegisteredPluginIds =>
        _plugins.Keys.OrderBy(id => id.Value, StringComparer.Ordinal).ToList();

    public bool TryGetPlugin(EngineeringAgentProviderPluginId pluginId, out IEngineeringAgentProviderPlugin? plugin) =>
        _plugins.TryGetValue(pluginId, out plugin);
}

public sealed class EngineeringAgentPluginCatalogDuplicateIdentityException : Exception
{
    public EngineeringAgentPluginCatalogDuplicateIdentityException(EngineeringAgentProviderPluginId pluginId)
        : base($"Duplicate Engineering Agent provider plugin identity: '{pluginId.Value}'.")
    {
        PluginId = pluginId;
    }

    public EngineeringAgentProviderPluginId PluginId { get; }
}
