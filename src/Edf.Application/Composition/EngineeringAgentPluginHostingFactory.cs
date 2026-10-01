namespace Edf.Application.Composition;

using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Projects.Sqlite;
using Edf.Application.Relay.EngineeringAgent.Hosting;

public static class EngineeringAgentPluginHostingFactory
{
    public static EngineeringAgentPluginHostingServices Create(IUserApplicationStatePersistence persistence)
    {
        IEngineeringAgentOperationalPreferenceBacking? backing = persistence switch
        {
            SqliteUserApplicationStatePersistence sqlite => sqlite.CreateEngineeringAgentPreferenceBacking(),
            _ => null,
        };

        var catalog = EngineeringAgentPluginCatalog.CreateEmpty();
        var preferences = new EngineeringAgentPluginProjectPreferencesStore(backing);
        var host = new EngineeringAgentPluginHost(catalog);
        var selection = new EngineeringAgentPluginSelectionService(catalog, host, preferences);
        return new EngineeringAgentPluginHostingServices(catalog, host, preferences, selection);
    }
}
