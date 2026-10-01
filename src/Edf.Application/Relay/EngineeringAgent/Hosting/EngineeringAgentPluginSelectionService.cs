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

/// <summary>
/// Stage A — static provider preflight before host initialization (A4-T3 bounded T1 refinement).
/// </summary>
public sealed record EngineeringAgentPluginPreflightEvaluation(
    bool CanProceedToInitialization,
    EngineeringAgentProviderPluginId? SelectedPluginId,
    IEngineeringAgentProviderPlugin? ResolvedPlugin,
    EngineeringAgentPluginSelectionUnavailableReason UnavailableReason,
    EngineeringAgentPluginCompatibilityAssessment? Compatibility,
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

    public EngineeringAgentPluginPreflightEvaluation EvaluatePreflightForAutomatedTransport(
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
            return PreflightUnavailable(
                EngineeringAgentPluginSelectionUnavailableReason.NoProviderSelected,
                selectedId: null,
                detail: "No Engineering Agent provider plugin is selected for this Project.");
        }

        if (!_catalog.TryGetPlugin(selectedId, out var plugin) || plugin is null)
        {
            return PreflightUnavailable(
                EngineeringAgentPluginSelectionUnavailableReason.ProviderNotRegistered,
                selectedId,
                detail: "Selected plugin is not registered in the catalog.");
        }

        if (!prefs.EnabledPluginIds.Contains(selectedId))
        {
            return PreflightUnavailable(
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
            return PreflightUnavailable(
                EngineeringAgentPluginSelectionUnavailableReason.HostContractIncompatible,
                selectedId,
                plugin,
                compatibility,
                compatibility.HostContract.Reason);
        }

        if (!compatibility.RenderProtocol.IsCompatible)
        {
            return PreflightUnavailable(
                EngineeringAgentPluginSelectionUnavailableReason.RenderProtocolIncompatible,
                selectedId,
                plugin,
                compatibility,
                compatibility.RenderProtocol.Reason);
        }

        if (!compatibility.RoutingIntent.IsCompatible)
        {
            return PreflightUnavailable(
                EngineeringAgentPluginSelectionUnavailableReason.RoutingIntentUnsupported,
                selectedId,
                plugin,
                compatibility,
                compatibility.RoutingIntent.Reason);
        }

        return new EngineeringAgentPluginPreflightEvaluation(
            CanProceedToInitialization: true,
            SelectedPluginId: selectedId,
            ResolvedPlugin: plugin,
            UnavailableReason: EngineeringAgentPluginSelectionUnavailableReason.None,
            Compatibility: compatibility,
            Detail: null);
    }

    public EngineeringAgentPluginSelectionEvaluation EvaluateRuntimeReadinessForAutomatedTransport(
        ProjectConcordProjectId projectId,
        EngineeringAgentMode routingIntent,
        int requiredRenderProtocolMajor = 0)
    {
        var preflight = EvaluatePreflightForAutomatedTransport(
            projectId,
            routingIntent,
            requiredRenderProtocolMajor);
        if (!preflight.CanProceedToInitialization
            || preflight.SelectedPluginId is not { } selectedId
            || preflight.ResolvedPlugin is null)
        {
            return FromPreflight(preflight);
        }

        var plugin = preflight.ResolvedPlugin;
        var compatibility = preflight.Compatibility!;

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

    public EngineeringAgentPluginSelectionEvaluation EvaluateForAutomatedTransport(
        ProjectConcordProjectId projectId,
        EngineeringAgentMode routingIntent,
        int requiredRenderProtocolMajor = 0) =>
        EvaluateRuntimeReadinessForAutomatedTransport(projectId, routingIntent, requiredRenderProtocolMajor);

    private static EngineeringAgentPluginSelectionEvaluation FromPreflight(
        EngineeringAgentPluginPreflightEvaluation preflight) =>
        new(
            CanUseForAutomatedTransport: false,
            SelectedPluginId: preflight.SelectedPluginId,
            ResolvedPlugin: preflight.ResolvedPlugin,
            UnavailableReason: preflight.UnavailableReason,
            Compatibility: preflight.Compatibility,
            Health: preflight.ResolvedPlugin?.GetHealth(),
            Detail: preflight.Detail);

    private static EngineeringAgentPluginPreflightEvaluation PreflightUnavailable(
        EngineeringAgentPluginSelectionUnavailableReason reason,
        EngineeringAgentProviderPluginId? selectedId,
        IEngineeringAgentProviderPlugin? plugin = null,
        EngineeringAgentPluginCompatibilityAssessment? compatibility = null,
        string? detail = null) =>
        new(
            CanProceedToInitialization: false,
            SelectedPluginId: selectedId,
            ResolvedPlugin: plugin,
            UnavailableReason: reason,
            Compatibility: compatibility,
            Detail: detail);

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
