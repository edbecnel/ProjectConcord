namespace Edf.Application.Relay.EngineeringAgent.Hosting;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public enum EngineeringAgentPluginSelectionUnavailableReason
{
    None = 0,
    NoProviderSelected = 1,
    ProviderNotRegistered = 2,
    ProviderDisabled = 3,
    HostContractIncompatible = 4,
    RenderProtocolIncompatible = 5,
    RoutingIntentUnsupported = 6,
    PluginUnhealthy = 7,
    PluginNotInitialized = 8,
}

public sealed record EngineeringAgentPluginSelectionEvaluation(
    bool CanUseForAutomatedTransport,
    EngineeringAgentProviderPluginId? SelectedPluginId,
    IEngineeringAgentProviderPlugin? ResolvedPlugin,
    EngineeringAgentPluginSelectionUnavailableReason UnavailableReason,
    EngineeringAgentPluginCompatibilityAssessment? Compatibility,
    EngineeringAgentProviderHealth? Health,
    string? Detail);

public sealed class EngineeringAgentPluginSelectionService
{
    private readonly IEngineeringAgentPluginCatalog _catalog;
    private readonly EngineeringAgentPluginHost _host;
    private readonly IEngineeringAgentPluginProjectPreferencesStore _preferences;

    public EngineeringAgentPluginSelectionService(
        IEngineeringAgentPluginCatalog catalog,
        EngineeringAgentPluginHost host,
        IEngineeringAgentPluginProjectPreferencesStore preferences)
    {
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
    }

    public EngineeringAgentPluginSelectionEvaluation EvaluateForAutomatedTransport(
        ProjectConcordProjectId projectId,
        EngineeringAgentMode routingIntent,
        int requiredRenderProtocolMajor = 0)
    {
        if (requiredRenderProtocolMajor == 0)
        {
            requiredRenderProtocolMajor = RelayRenderVersion.V1.Major;
        }

        var prefs = _preferences.GetPreferences(projectId);
        if (prefs.SelectedPluginId is not { } selectedId)
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.NoProviderSelected,
                selectedId: null,
                detail: "No Engineering Agent provider plugin is selected for this Project.");
        }

        if (!_catalog.TryGetPlugin(selectedId, out var plugin) || plugin is null)
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.ProviderNotRegistered,
                selectedId,
                detail: "Selected plugin is not registered in the catalog.");
        }

        if (!prefs.EnabledPluginIds.Contains(selectedId))
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.ProviderDisabled,
                selectedId,
                plugin,
                detail: "Selected plugin is disabled for this Project.");
        }

        var compatibility = EngineeringAgentPluginCompatibilityEvaluator.Assess(
            plugin,
            requiredRenderProtocolMajor,
            routingIntent);

        if (!compatibility.HostContract.IsCompatible)
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.HostContractIncompatible,
                selectedId,
                plugin,
                compatibility,
                compatibility.HostContract.Reason);
        }

        if (!compatibility.RenderProtocol.IsCompatible)
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.RenderProtocolIncompatible,
                selectedId,
                plugin,
                compatibility,
                compatibility.RenderProtocol.Reason);
        }

        if (!compatibility.RoutingIntent.IsCompatible)
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.RoutingIntentUnsupported,
                selectedId,
                plugin,
                compatibility,
                compatibility.RoutingIntent.Reason);
        }

        if (!_host.IsInitialized(selectedId))
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.PluginNotInitialized,
                selectedId,
                plugin,
                compatibility,
                detail: "Selected plugin has not been initialized by the plugin host.");
        }

        var health = plugin.GetHealth();
        if (!health.IsInitialized || !health.IsAuthenticated)
        {
            return Unavailable(
                EngineeringAgentPluginSelectionUnavailableReason.PluginUnhealthy,
                selectedId,
                plugin,
                compatibility,
                health.StatusMessage ?? "Plugin is not healthy or authenticated.");
        }

        return new EngineeringAgentPluginSelectionEvaluation(
            CanUseForAutomatedTransport: true,
            SelectedPluginId: selectedId,
            ResolvedPlugin: plugin,
            UnavailableReason: EngineeringAgentPluginSelectionUnavailableReason.None,
            Compatibility: compatibility,
            Health: health,
            Detail: null);
    }

    private static EngineeringAgentPluginSelectionEvaluation Unavailable(
        EngineeringAgentPluginSelectionUnavailableReason reason,
        EngineeringAgentProviderPluginId? selectedId,
        IEngineeringAgentProviderPlugin? plugin = null,
        EngineeringAgentPluginCompatibilityAssessment? compatibility = null,
        string? detail = null) =>
        new(
            CanUseForAutomatedTransport: false,
            SelectedPluginId: selectedId,
            ResolvedPlugin: plugin,
            UnavailableReason: reason,
            Compatibility: compatibility,
            Health: plugin?.GetHealth(),
            Detail: detail);
}
