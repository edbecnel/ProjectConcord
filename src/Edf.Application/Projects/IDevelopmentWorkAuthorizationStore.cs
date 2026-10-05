using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects;

public interface IDevelopmentWorkAuthorizationStore
{
    void Insert(DevelopmentWorkAuthorization authorization);

    DevelopmentWorkAuthorization? GetById(DevelopmentWorkAuthorizationId authorizationId);

    IReadOnlyList<DevelopmentWorkAuthorization> ListByProject(ProjectConcordProjectId projectId);

    IReadOnlyList<DevelopmentWorkAuthorization> ListByWorkflowInstance(WorkflowInstanceId workflowInstanceId);

    DevelopmentWorkAuthorization? TryGetActiveByContext(
        WorkflowInstanceId workflowInstanceId,
        DevelopmentWorkAuthorizationKind authorizationKind,
        string normalizedTrancheKey);

    bool TryUpdateWithExpectedVersion(DevelopmentWorkAuthorization authorization, long expectedResourceVersion);
}
