using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects;

public interface IWorkflowInstanceStopStore
{
    void InsertSummary(WorkflowInstanceStopSummary summary);

    void InsertEvent(WorkflowInstanceStopEvent stopEvent);

    WorkflowInstanceStopSummary? GetSummary(WorkflowInstanceId workflowInstanceId);

    IReadOnlyList<WorkflowInstanceStopSummary> ListSummariesByProject(ProjectConcordProjectId projectId);

    IReadOnlyList<WorkflowInstanceStopEvent> ListEventsByWorkflowInstance(WorkflowInstanceId workflowInstanceId);

    bool TryUpdateSummaryWithExpectedVersion(WorkflowInstanceStopSummary summary, long expectedResourceVersion);
}
