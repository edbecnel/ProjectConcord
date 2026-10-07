using Edf.Application.Operator.WorkContinuity;
using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteOperatorWorkFocusStoreAdapter : IOperatorWorkFocusStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteOperatorWorkFocusStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public OperatorActiveWorkFocus? GetActiveForProject(ProjectConcordProjectId projectId) =>
        _store.GetOperatorWorkFocus(projectId);

    public void Upsert(OperatorActiveWorkFocus record) => _store.UpsertOperatorWorkFocus(record);

    public bool TryUpdateWithExpectedVersion(OperatorActiveWorkFocus record, long expectedResourceVersion) =>
        _store.TryUpdateOperatorWorkFocusWithExpectedVersion(record, expectedResourceVersion);
}
