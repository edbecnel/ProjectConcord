namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Collections.Concurrent;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;
using static Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models.CursorEngineeringAgentModelFamilies;
using Edf.Domain.Projects;

/// <summary>
/// Per-project durable Cursor model preferences (ACP provider-internal).
/// </summary>
public sealed class CursorEngineeringAgentModelPreferencesStore : ICursorEngineeringAgentModelPreferencesStore
{
    private readonly ConcurrentDictionary<ProjectConcordProjectId, CursorEngineeringAgentModelSelection> _cache = new();
    private readonly IEngineeringAgentOperationalPreferenceBacking? _backing;

    public CursorEngineeringAgentModelPreferencesStore(IEngineeringAgentOperationalPreferenceBacking? backing = null)
    {
        _backing = backing;
    }

    public CursorEngineeringAgentModelSelection GetSelection(ProjectConcordProjectId projectId)
    {
        if (_cache.TryGetValue(projectId, out var cached))
        {
            return cached;
        }

        if (_backing is null)
        {
            return CursorEngineeringAgentModelSelection.DefaultComposer25NonFast;
        }

        var family = _backing.Get(FamilyKey(projectId));
        var fastRaw = _backing.Get(FastKey(projectId));
        if (string.IsNullOrWhiteSpace(family) && string.IsNullOrWhiteSpace(fastRaw))
        {
            return CursorEngineeringAgentModelSelection.DefaultComposer25NonFast;
        }

        var selection = new CursorEngineeringAgentModelSelection(
            string.IsNullOrWhiteSpace(family)
                ? Composer25
                : family.Trim(),
            ParseFastEnabled(fastRaw));
        _cache[projectId] = selection;
        return selection;
    }

    public void SetSelection(ProjectConcordProjectId projectId, CursorEngineeringAgentModelSelection selection)
    {
        ArgumentNullException.ThrowIfNull(selection);
        _cache[projectId] = selection;
        if (_backing is null)
        {
            return;
        }

        _backing.Set(FamilyKey(projectId), selection.FamilySlug);
        _backing.Set(FastKey(projectId), selection.FastEnabled ? "true" : "false");
    }

    internal static bool ParseFastEnabled(string? raw) =>
        string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase);

    private static string FamilyKey(ProjectConcordProjectId projectId) =>
        $"engineering_agent.cursor.model.family.{projectId.Value:D}";

    private static string FastKey(ProjectConcordProjectId projectId) =>
        $"engineering_agent.cursor.model.fast.{projectId.Value:D}";
}

public interface ICursorEngineeringAgentModelPreferencesStore
{
    CursorEngineeringAgentModelSelection GetSelection(ProjectConcordProjectId projectId);

    void SetSelection(ProjectConcordProjectId projectId, CursorEngineeringAgentModelSelection selection);
}
