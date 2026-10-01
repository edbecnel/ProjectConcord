namespace Edf.Application.Relay.EngineeringAgent.Hosting;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Projects;

/// <summary>
/// Per-Project operational preferences for Engineering Agent plugin enablement and selection (ADR-0023).
/// Not canonical EDF or relay governance state.
/// </summary>
public interface IEngineeringAgentPluginProjectPreferencesStore
{
    EngineeringAgentPluginProjectPreferences GetPreferences(ProjectConcordProjectId projectId);

    void SetSelectedPlugin(ProjectConcordProjectId projectId, EngineeringAgentProviderPluginId? selectedPluginId);

    void SetPluginEnabled(
        ProjectConcordProjectId projectId,
        EngineeringAgentProviderPluginId pluginId,
        bool enabled);
}

public sealed record EngineeringAgentPluginProjectPreferences(
    EngineeringAgentProviderPluginId? SelectedPluginId,
    IReadOnlySet<EngineeringAgentProviderPluginId> EnabledPluginIds);
