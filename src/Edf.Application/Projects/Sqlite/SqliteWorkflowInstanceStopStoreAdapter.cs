using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteWorkflowInstanceStopStoreAdapter : IWorkflowInstanceStopStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteWorkflowInstanceStopStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public void InsertSummary(WorkflowInstanceStopSummary summary) =>
        _store.InsertWorkflowInstanceStopSummary(summary);

    public void InsertEvent(WorkflowInstanceStopEvent stopEvent) =>
        _store.InsertWorkflowInstanceStopEvent(stopEvent);

    public WorkflowInstanceStopSummary? GetSummary(WorkflowInstanceId workflowInstanceId) =>
        _store.GetWorkflowInstanceStopSummary(workflowInstanceId);

    public IReadOnlyList<WorkflowInstanceStopSummary> ListSummariesByProject(ProjectConcordProjectId projectId) =>
        _store.ListWorkflowInstanceStopSummaries(projectId);

    public IReadOnlyList<WorkflowInstanceStopEvent> ListEventsByWorkflowInstance(WorkflowInstanceId workflowInstanceId) =>
        _store.ListWorkflowInstanceStopEvents(workflowInstanceId);

    public bool TryUpdateSummaryWithExpectedVersion(WorkflowInstanceStopSummary summary, long expectedResourceVersion) =>
        _store.TryUpdateWorkflowInstanceStopSummaryWithExpectedVersion(summary, expectedResourceVersion);
}
