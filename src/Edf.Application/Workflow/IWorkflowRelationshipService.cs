using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public interface IWorkflowRelationshipService
{
    WorkflowOrigin RecordGovernedWorkflowOrigin(
        WorkflowInstanceId sourceInstanceId,
        WorkflowInstanceId derivedInstanceId,
        WorkflowOriginKind originKind,
        GovernedWorkflowMutationAuthority authority);

    WorkflowDependency RecordGovernedBlockingDependency(
        WorkflowInstanceId blockedInstanceId,
        WorkflowInstanceId requiredInstanceId,
        GovernedWorkflowMutationAuthority authority);

    WorkflowDependency RecordGovernedDependencySatisfaction(
        WorkflowDependencyId dependencyId,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority);

    WorkflowDependency RecordGovernedDependencyRelease(
        WorkflowDependencyId dependencyId,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority);
}
