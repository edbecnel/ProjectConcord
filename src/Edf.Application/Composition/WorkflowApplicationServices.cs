using Edf.Application.Operator.WorkState;
using Edf.Application.Projects;
using Edf.Application.Workflow;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Composition;

public sealed record WorkflowApplicationServices(
    IWorkflowInstanceService WorkflowInstances,
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
        var recovery = new GovernedWorkStateRecoveryService(persistence.WorkflowInstances, registry);
        var projection = new GovernedWorkStateOperatorProjectionService(recovery, git);
        return new WorkflowApplicationServices(workflowService, recovery, projection);
    }
}
