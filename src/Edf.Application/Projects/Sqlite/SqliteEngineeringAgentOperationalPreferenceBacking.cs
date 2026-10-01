namespace Edf.Application.Projects.Sqlite;

using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.ProjectServices.Persistence;

internal sealed class SqliteEngineeringAgentOperationalPreferenceBacking : IEngineeringAgentOperationalPreferenceBacking
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteEngineeringAgentOperationalPreferenceBacking(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public string? Get(string key) => _store.GetUserPreference(key);

    public void Set(string key, string? value) => _store.SetUserPreference(key, value);
}
