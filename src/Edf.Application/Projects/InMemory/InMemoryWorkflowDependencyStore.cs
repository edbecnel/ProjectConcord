using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryWorkflowDependencyStore : IWorkflowDependencyStore
{
    private readonly Dictionary<WorkflowDependencyId, WorkflowDependency> _dependencies = new();

    public void Insert(WorkflowDependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        if (_dependencies.ContainsKey(dependency.DependencyId))
        {
            throw new InvalidOperationException($"Workflow dependency '{dependency.DependencyId}' already exists.");
        }

        if (dependency.Status == WorkflowDependencyStatus.Pending
            && _dependencies.Values.Any(d =>
                d.Status == WorkflowDependencyStatus.Pending
                && d.BlockedWorkflowInstanceId == dependency.BlockedWorkflowInstanceId
                && d.RequiredWorkflowInstanceId == dependency.RequiredWorkflowInstanceId))
        {
            throw new InvalidOperationException("A pending dependency already exists for this blocked/required pair.");
        }

        _dependencies[dependency.DependencyId] = dependency;
    }

    public WorkflowDependency? GetById(WorkflowDependencyId dependencyId) =>
        _dependencies.TryGetValue(dependencyId, out var dependency) ? dependency : null;

    public IReadOnlyList<WorkflowDependency> ListByProject(ProjectConcordProjectId projectId) =>
        _dependencies.Values.Where(d => d.ProjectId == projectId).OrderBy(d => d.CreatedUtc).ToList();

    public IReadOnlyList<WorkflowDependency> ListPendingByBlocked(WorkflowInstanceId blockedInstanceId) =>
        _dependencies.Values
            .Where(d => d.Status == WorkflowDependencyStatus.Pending && d.BlockedWorkflowInstanceId == blockedInstanceId)
            .ToList();

    public IReadOnlyList<WorkflowDependency> ListPendingForProject(ProjectConcordProjectId projectId) =>
        _dependencies.Values
            .Where(d => d.ProjectId == projectId && d.Status == WorkflowDependencyStatus.Pending)
            .ToList();

    public bool TryUpdateWithExpectedVersion(WorkflowDependency dependency, long expectedResourceVersion)
    {
        if (!_dependencies.TryGetValue(dependency.DependencyId, out var existing))
        {
            return false;
        }

        if (existing.ResourceVersion != expectedResourceVersion)
        {
            return false;
        }

        _dependencies[dependency.DependencyId] = dependency;
        return true;
    }
}
