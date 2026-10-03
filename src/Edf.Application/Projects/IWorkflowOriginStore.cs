using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects;

public interface IWorkflowOriginStore
{
    void Insert(WorkflowOrigin origin);

    IReadOnlyList<WorkflowOrigin> ListByProject(ProjectConcordProjectId projectId);
}
