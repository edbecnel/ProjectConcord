namespace Edf.Application.Relay.EngineeringAgent.Hosting;

using Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Provider-neutral plugin hosting lifecycle (ADR-0023 §2). Not transport lifecycle.
/// </summary>
public sealed class EngineeringAgentPluginHost : IEngineeringAgentPluginHost
{
    private readonly IEngineeringAgentPluginCatalog _catalog;
    private readonly HashSet<EngineeringAgentProviderPluginId> _initialized = new();
    private readonly object _sync = new();

    public EngineeringAgentPluginHost(IEngineeringAgentPluginCatalog catalog)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    public async Task<EngineeringAgentProviderInitializeResult> InitializePluginAsync(
        EngineeringAgentProviderPluginId pluginId,
        CancellationToken cancellationToken = default)
    {
        if (!_catalog.TryGetPlugin(pluginId, out var plugin) || plugin is null)
        {
            return new EngineeringAgentProviderInitializeResult(
                EngineeringAgentProviderHealth.Unavailable("Plugin is not registered in the catalog."),
                null);
        }

        lock (_sync)
        {
            if (_initialized.Contains(pluginId))
            {
                return new EngineeringAgentProviderInitializeResult(plugin.GetHealth(), Failure: null);
            }
        }

        var result = await plugin.InitializeAsync(cancellationToken).ConfigureAwait(false);
        if (!result.Succeeded)
        {
            return result;
        }

        lock (_sync)
        {
            _initialized.Add(pluginId);
        }

        return result;
    }

    public async Task<EngineeringAgentProviderShutdownResult> ShutdownPluginAsync(
        EngineeringAgentProviderPluginId pluginId,
        CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            if (!_initialized.Contains(pluginId))
            {
                return new EngineeringAgentProviderShutdownResult(IsAcknowledged: true, Failure: null);
            }
        }

        if (!_catalog.TryGetPlugin(pluginId, out var plugin) || plugin is null)
        {
            lock (_sync)
            {
                _initialized.Remove(pluginId);
            }

            return new EngineeringAgentProviderShutdownResult(
                IsAcknowledged: false,
                new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.Unavailable,
                    "Plugin is not registered in the catalog."));
        }

        var result = await plugin.ShutdownAsync(cancellationToken).ConfigureAwait(false);

        lock (_sync)
        {
            _initialized.Remove(pluginId);
        }

        return result;
    }

    public bool IsInitialized(EngineeringAgentProviderPluginId pluginId)
    {
        lock (_sync)
        {
            return _initialized.Contains(pluginId);
        }
    }
}
