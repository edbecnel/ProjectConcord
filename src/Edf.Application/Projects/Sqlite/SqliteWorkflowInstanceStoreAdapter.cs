using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteWorkflowInstanceStoreAdapter : IWorkflowInstanceStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteWorkflowInstanceStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public void Insert(WorkflowInstance instance) => _store.InsertWorkflowInstance(instance);

    public WorkflowInstance? GetById(WorkflowInstanceId instanceId) => _store.GetWorkflowInstance(instanceId);

    public IReadOnlyList<WorkflowInstance> ListByProject(
        ProjectConcordProjectId projectId,
        WorkflowInstanceLifecycle? lifecycleFilter = null) =>
        _store.ListWorkflowInstances(projectId, lifecycleFilter);

    public bool TryUpdateWithExpectedVersion(WorkflowInstance instance, long expectedResourceVersion) =>
        _store.TryUpdateWorkflowInstanceWithExpectedVersion(instance, expectedResourceVersion);
}
