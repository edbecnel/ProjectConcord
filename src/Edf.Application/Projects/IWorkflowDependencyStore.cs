using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects;

public interface IWorkflowDependencyStore
{
    void Insert(WorkflowDependency dependency);

    WorkflowDependency? GetById(WorkflowDependencyId dependencyId);

    IReadOnlyList<WorkflowDependency> ListByProject(ProjectConcordProjectId projectId);

    IReadOnlyList<WorkflowDependency> ListPendingByBlocked(WorkflowInstanceId blockedInstanceId);

    IReadOnlyList<WorkflowDependency> ListPendingForProject(ProjectConcordProjectId projectId);

    bool TryUpdateWithExpectedVersion(WorkflowDependency dependency, long expectedResourceVersion);
}
