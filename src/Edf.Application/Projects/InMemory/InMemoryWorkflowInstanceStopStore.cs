using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryWorkflowInstanceStopStore : IWorkflowInstanceStopStore
{
    private readonly Dictionary<WorkflowInstanceId, WorkflowInstanceStopSummary> _summaries = new();
    private readonly List<WorkflowInstanceStopEvent> _events = new();

    public void InsertSummary(WorkflowInstanceStopSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        if (_summaries.ContainsKey(summary.WorkflowInstanceId))
        {
            throw new InvalidOperationException(
                $"Stop summary for workflow instance '{summary.WorkflowInstanceId}' already exists.");
        }

        _summaries[summary.WorkflowInstanceId] = summary;
    }

    public void InsertEvent(WorkflowInstanceStopEvent stopEvent)
    {
        ArgumentNullException.ThrowIfNull(stopEvent);
        _events.Add(stopEvent);
    }

    public WorkflowInstanceStopSummary? GetSummary(WorkflowInstanceId workflowInstanceId) =>
        _summaries.TryGetValue(workflowInstanceId, out var summary) ? summary : null;

    public IReadOnlyList<WorkflowInstanceStopSummary> ListSummariesByProject(ProjectConcordProjectId projectId) =>
        _summaries.Values.Where(s => s.ProjectId == projectId).ToList();

    public IReadOnlyList<WorkflowInstanceStopEvent> ListEventsByWorkflowInstance(WorkflowInstanceId workflowInstanceId) =>
        _events.Where(e => e.WorkflowInstanceId == workflowInstanceId).OrderBy(e => e.OccurredUtc).ToList();

    public bool TryUpdateSummaryWithExpectedVersion(WorkflowInstanceStopSummary summary, long expectedResourceVersion)
    {
        if (!_summaries.TryGetValue(summary.WorkflowInstanceId, out var existing))
        {
            return false;
        }

        if (existing.ResourceVersion != expectedResourceVersion)
        {
            return false;
        }

        _summaries[summary.WorkflowInstanceId] = summary;
        return true;
    }
}
