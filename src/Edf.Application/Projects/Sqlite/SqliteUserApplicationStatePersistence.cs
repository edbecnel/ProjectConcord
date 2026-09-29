using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

public sealed class SqliteUserApplicationStatePersistence : IUserApplicationStatePersistence, IDisposable
{
    private readonly SqliteUserApplicationStateStore _store;
    private readonly SqliteProjectRegistryAdapter _registry;
    private readonly SqliteUserPreferencesStoreAdapter _preferences;
    private readonly SqliteRelayOperationalStoreAdapter _relay;

    public SqliteUserApplicationStatePersistence(SqliteUserApplicationStateStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _registry = new SqliteProjectRegistryAdapter(store);
        _preferences = new SqliteUserPreferencesStoreAdapter(store);
        _relay = new SqliteRelayOperationalStoreAdapter(store);
    }

    public IProjectRegistry ProjectRegistry => _registry;

    public IUserPreferencesStore UserPreferences => _preferences;

    public IRelayOperationalStore RelayOperational => _relay;

    public void ExecuteInTransaction(Action work) => _store.ExecuteInTransaction(work);

    public void Dispose() => _store.Dispose();

    private sealed class SqliteProjectRegistryAdapter : IProjectRegistry
    {
        private readonly SqliteUserApplicationStateStore _store;

        public SqliteProjectRegistryAdapter(SqliteUserApplicationStateStore store) => _store = store;

        public int MaxRecentProjects => _store.MaxRecentProjects;

        public ManagedProject RegisterNewProjectAtLocator(
            ProjectLocator locator,
            string displayName,
            DateTimeOffset openedUtc) =>
            _store.RegisterNewProjectAtLocator(locator, displayName, openedUtc);

        public ManagedProject? ResolveByRegisteredLocator(ProjectLocator locator) =>
            _store.ResolveByRegisteredLocator(locator);

        public ManagedProject? GetById(ProjectConcordProjectId projectId) => _store.GetById(projectId);

        public IReadOnlyList<RecentProjectEntry> ListRecent(ProjectConcordProjectId? lastActiveProjectId) =>
            _store.ListRecent(lastActiveProjectId);

        public void RecordSuccessfulOpen(ProjectConcordProjectId projectId, DateTimeOffset openedUtc) =>
            _store.RecordSuccessfulOpen(projectId, openedUtc);

        public void RemoveFromRecent(ProjectConcordProjectId projectId) => _store.RemoveFromRecent(projectId);

        public ReconcileLocatorResult ReconcileProjectLocator(
            ProjectConcordProjectId projectId,
            ProjectLocator newLocator,
            string displayName,
            DateTimeOffset reconciledUtc)
        {
            var result = _store.ReconcileProjectLocator(projectId, newLocator, displayName, reconciledUtc);
            return result.Success && result.Project is not null
                ? ReconcileLocatorResult.Succeeded(result.Project)
                : ReconcileLocatorResult.Failed(result.ErrorMessage ?? "Reconciliation failed.");
        }
    }

    private sealed class SqliteUserPreferencesStoreAdapter : IUserPreferencesStore
    {
        private readonly SqliteUserApplicationStateStore _store;

        public SqliteUserPreferencesStoreAdapter(SqliteUserApplicationStateStore store) => _store = store;

        public ProjectConcordProjectId? GetLastActiveProjectId() => _store.GetLastActiveProjectId();

        public void SetLastActiveProjectId(ProjectConcordProjectId? projectId) =>
            _store.SetLastActiveProjectId(projectId);
    }
}
