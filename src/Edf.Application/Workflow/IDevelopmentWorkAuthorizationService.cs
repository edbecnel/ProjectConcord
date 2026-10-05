using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public interface IDevelopmentWorkAuthorizationService
{
    DevelopmentWorkAuthorization RecordGovernedGrant(
        WorkflowInstanceId workflowInstanceId,
        DevelopmentWorkAuthorizationKind authorizationKind,
        string? authorizedTrancheId,
        IReadOnlyList<string>? authorizedScopeMarkers,
        string? authorityReference,
        GovernedWorkflowMutationAuthority authority);

    DevelopmentWorkAuthorization RecordGovernedSupersede(
        DevelopmentWorkAuthorizationId authorizationId,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority);
}
