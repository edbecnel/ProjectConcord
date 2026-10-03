using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryWorkflowOriginStore : IWorkflowOriginStore
{
    private readonly List<WorkflowOrigin> _origins = new();

    public void Insert(WorkflowOrigin origin)
    {
        ArgumentNullException.ThrowIfNull(origin);
        if (_origins.Any(o =>
                o.SourceWorkflowInstanceId == origin.SourceWorkflowInstanceId
                && o.DerivedWorkflowInstanceId == origin.DerivedWorkflowInstanceId))
        {
            throw new InvalidOperationException("Workflow origin for this source/derived pair already exists.");
        }

        _origins.Add(origin);
    }

    public IReadOnlyList<WorkflowOrigin> ListByProject(ProjectConcordProjectId projectId) =>
        _origins.Where(o => o.ProjectId == projectId).OrderBy(o => o.CreatedUtc).ToList();
}
