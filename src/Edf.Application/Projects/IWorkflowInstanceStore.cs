using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects;

public interface IWorkflowInstanceStore
{
    void Insert(WorkflowInstance instance);

    WorkflowInstance? GetById(WorkflowInstanceId instanceId);

    IReadOnlyList<WorkflowInstance> ListByProject(
        ProjectConcordProjectId projectId,
        WorkflowInstanceLifecycle? lifecycleFilter = null);

    /// <summary>
    /// Persists the instance when <paramref name="expectedResourceVersion"/> matches the stored row.
    /// </summary>
    /// <returns>True when the update succeeded; false when the row was missing or version mismatched.</returns>
    bool TryUpdateWithExpectedVersion(WorkflowInstance instance, long expectedResourceVersion);
}
