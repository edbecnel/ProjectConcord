using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;
using Edf.Domain.Projects;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class CursorEngineeringAgentModelPreferencesStoreTests
{
    [Fact]
    public void NoStoredPreference_DefaultsComposer25NonFast()
    {
        var store = new CursorEngineeringAgentModelPreferencesStore();
        var selection = store.GetSelection(ProjectConcordProjectId.New());

        Assert.Equal(CursorEngineeringAgentModelFamilies.Composer25, selection.FamilySlug);
        Assert.False(selection.FastEnabled);
    }

    [Fact]
    public void ExplicitPreference_OverridesDefault_AndPersistsAcrossStoreRecreation()
    {
        var backing = new InMemoryEngineeringAgentPreferenceBacking();
        var projectId = ProjectConcordProjectId.New();
        var store = new CursorEngineeringAgentModelPreferencesStore(backing);
        store.SetSelection(
            projectId,
            new CursorEngineeringAgentModelSelection(CursorEngineeringAgentModelFamilies.Composer25, FastEnabled: true));

        var recreated = new CursorEngineeringAgentModelPreferencesStore(backing);
        var selection = recreated.GetSelection(projectId);

        Assert.True(selection.FastEnabled);
    }

    private sealed class InMemoryEngineeringAgentPreferenceBacking : IEngineeringAgentOperationalPreferenceBacking
    {
        private readonly Dictionary<string, string> _values = new(StringComparer.Ordinal);

        public string? Get(string key) => _values.TryGetValue(key, out var value) ? value : null;

        public void Set(string key, string value) => _values[key] = value;
    }
}
