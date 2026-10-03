using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteWorkflowOriginStoreAdapter : IWorkflowOriginStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteWorkflowOriginStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public void Insert(WorkflowOrigin origin) => _store.InsertWorkflowOrigin(origin);

    public IReadOnlyList<WorkflowOrigin> ListByProject(ProjectConcordProjectId projectId) =>
        _store.ListWorkflowOrigins(projectId);
}
