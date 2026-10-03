using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryWorkflowInstanceStore : IWorkflowInstanceStore
{
    private readonly Dictionary<WorkflowInstanceId, WorkflowInstance> _instances = new();

    public void Insert(WorkflowInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        if (_instances.ContainsKey(instance.InstanceId))
        {
            throw new InvalidOperationException($"Workflow instance '{instance.InstanceId}' already exists.");
        }

        _instances[instance.InstanceId] = instance;
    }

    public WorkflowInstance? GetById(WorkflowInstanceId instanceId) =>
        _instances.TryGetValue(instanceId, out var instance) ? instance : null;

    public IReadOnlyList<WorkflowInstance> ListByProject(
        ProjectConcordProjectId projectId,
        WorkflowInstanceLifecycle? lifecycleFilter = null)
    {
        return _instances.Values
            .Where(i => i.ProjectId == projectId && (lifecycleFilter is null || i.Lifecycle == lifecycleFilter))
            .OrderBy(i => i.CreatedUtc)
            .ToList();
    }

    public bool TryUpdateWithExpectedVersion(WorkflowInstance instance, long expectedResourceVersion)
    {
        if (!_instances.TryGetValue(instance.InstanceId, out var existing))
        {
            return false;
        }

        if (existing.ResourceVersion != expectedResourceVersion)
        {
            return false;
        }

        _instances[instance.InstanceId] = instance;
        return true;
    }
}
