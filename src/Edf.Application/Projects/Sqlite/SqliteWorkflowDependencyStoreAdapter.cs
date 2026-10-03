using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteWorkflowDependencyStoreAdapter : IWorkflowDependencyStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteWorkflowDependencyStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public void Insert(WorkflowDependency dependency) => _store.InsertWorkflowDependency(dependency);

    public WorkflowDependency? GetById(WorkflowDependencyId dependencyId) =>
        _store.GetWorkflowDependency(dependencyId);

    public IReadOnlyList<WorkflowDependency> ListByProject(ProjectConcordProjectId projectId) =>
        _store.ListWorkflowDependencies(projectId);

    public IReadOnlyList<WorkflowDependency> ListPendingByBlocked(WorkflowInstanceId blockedInstanceId) =>
        _store.ListPendingWorkflowDependenciesByBlocked(blockedInstanceId);

    public IReadOnlyList<WorkflowDependency> ListPendingForProject(ProjectConcordProjectId projectId) =>
        _store.ListPendingWorkflowDependencies(projectId);

    public bool TryUpdateWithExpectedVersion(WorkflowDependency dependency, long expectedResourceVersion) =>
        _store.TryUpdateWorkflowDependencyWithExpectedVersion(dependency, expectedResourceVersion);
}
