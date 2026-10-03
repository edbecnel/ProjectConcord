using Edf.Application.Operator.WorkState;
using Edf.Application.Projects;
using Edf.Application.Workflow;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Composition;

public sealed record WorkflowApplicationServices(
    IWorkflowInstanceService WorkflowInstances,
    IWorkflowRelationshipService WorkflowRelationships,
    IGovernedWorkStateRecoveryService WorkStateRecovery,
    GovernedWorkStateOperatorProjectionService WorkStateOperatorProjection);

public static class WorkflowApplicationServicesFactory
{
    public static WorkflowApplicationServices Create(IUserApplicationStatePersistence persistence)
    {
        ArgumentNullException.ThrowIfNull(persistence);
        var registry = new GewV1PrescribedWorkflowRegistry();
        var git = new GitHeadCommitResolver();
        var workflowService = new WorkflowInstanceService(persistence.WorkflowInstances, registry, git);
        var relationshipService = new WorkflowRelationshipService(
            persistence.WorkflowInstances,
            persistence.WorkflowOrigins,
            persistence.WorkflowDependencies);
        var recovery = new GovernedWorkStateRecoveryService(
            persistence.WorkflowInstances,
            persistence.WorkflowOrigins,
            persistence.WorkflowDependencies,
            registry);
        var projection = new GovernedWorkStateOperatorProjectionService(recovery, git);
        return new WorkflowApplicationServices(workflowService, relationshipService, recovery, projection);
    }
}
