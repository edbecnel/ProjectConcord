namespace Edf.Application.Relay.EngineeringAgent.Hosting;

using System.Collections.Concurrent;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Projects;

/// <summary>
/// Project-scoped plugin selection/enablement using optional string preference backing (user_preferences table).
/// </summary>
public sealed class EngineeringAgentPluginProjectPreferencesStore : IEngineeringAgentPluginProjectPreferencesStore
{
    private readonly ConcurrentDictionary<ProjectConcordProjectId, EngineeringAgentPluginProjectPreferences> _cache = new();
    private readonly IEngineeringAgentOperationalPreferenceBacking? _backing;

    public EngineeringAgentPluginProjectPreferencesStore(IEngineeringAgentOperationalPreferenceBacking? backing = null)
    {
        _backing = backing;
    }

    public EngineeringAgentPluginProjectPreferences GetPreferences(ProjectConcordProjectId projectId)
    {
        if (_cache.TryGetValue(projectId, out var cached))
        {
            return cached;
        }

        if (_backing is null)
        {
            return EmptyPreferences;
        }

        var selectedRaw = _backing.Get(SelectedKey(projectId));
        EngineeringAgentProviderPluginId? selected = null;
        if (!string.IsNullOrWhiteSpace(selectedRaw))
        {
            selected = EngineeringAgentProviderPluginId.Parse(selectedRaw);
        }

        var enabledRaw = _backing.Get(EnabledKey(projectId));
        var enabled = ParseEnabledSet(enabledRaw);
        var prefs = new EngineeringAgentPluginProjectPreferences(selected, enabled);
        _cache[projectId] = prefs;
        return prefs;
    }

    public void SetSelectedPlugin(ProjectConcordProjectId projectId, EngineeringAgentProviderPluginId? selectedPluginId)
    {
        var current = GetPreferences(projectId);
        var updated = current with { SelectedPluginId = selectedPluginId };
        Persist(projectId, updated);
    }

    public void SetPluginEnabled(
        ProjectConcordProjectId projectId,
        EngineeringAgentProviderPluginId pluginId,
        bool enabled)
    {
        var current = GetPreferences(projectId);
        var set = new HashSet<EngineeringAgentProviderPluginId>(current.EnabledPluginIds);
        if (enabled)
        {
            set.Add(pluginId);
        }
        else
        {
            set.Remove(pluginId);
        }

        var updated = current with { EnabledPluginIds = set };
        Persist(projectId, updated);
    }

    private void Persist(ProjectConcordProjectId projectId, EngineeringAgentPluginProjectPreferences preferences)
    {
        _cache[projectId] = preferences;
        if (_backing is null)
        {
            return;
        }

        _backing.Set(SelectedKey(projectId), preferences.SelectedPluginId?.Value);
        _backing.Set(EnabledKey(projectId), SerializeEnabledSet(preferences.EnabledPluginIds));
    }

    private static EngineeringAgentPluginProjectPreferences EmptyPreferences =>
        new(null, new HashSet<EngineeringAgentProviderPluginId>());

    private static string SelectedKey(ProjectConcordProjectId projectId) =>
        $"engineering_agent.plugin.selected.{projectId.Value:D}";

    private static string EnabledKey(ProjectConcordProjectId projectId) =>
        $"engineering_agent.plugin.enabled.{projectId.Value:D}";

    private static IReadOnlySet<EngineeringAgentProviderPluginId> ParseEnabledSet(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return new HashSet<EngineeringAgentProviderPluginId>();
        }

        var set = new HashSet<EngineeringAgentProviderPluginId>();
        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            set.Add(EngineeringAgentProviderPluginId.Parse(part));
        }

        return set;
    }

    private static string? SerializeEnabledSet(IReadOnlySet<EngineeringAgentProviderPluginId> enabled)
    {
        if (enabled.Count == 0)
        {
            return null;
        }

        return string.Join(',', enabled.Select(id => id.Value).OrderBy(v => v, StringComparer.Ordinal));
    }
}

public interface IEngineeringAgentOperationalPreferenceBacking
{
    string? Get(string key);

    void Set(string key, string? value);
}
