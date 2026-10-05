using Edf.Application.Operator.WorkState;
using Edf.Application.Projects;
using Edf.Application.Workflow;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Composition;

public sealed record WorkflowApplicationServices(
    IWorkflowInstanceService WorkflowInstances,
    IWorkflowRelationshipService WorkflowRelationships,
    IDevelopmentWorkAuthorizationService DevelopmentWorkAuthorizations,
    IWorkflowInstanceStopService WorkflowInstanceStops,
    IGewV1EffectiveConfigurationResolver EffectiveConfigurationResolver,
    IGovernedWorkStateRecoveryService WorkStateRecovery,
    GovernedWorkStateOperatorProjectionService WorkStateOperatorProjection);

public static class WorkflowApplicationServicesFactory
{
    public static WorkflowApplicationServices Create(IUserApplicationStatePersistence persistence)
    {
        ArgumentNullException.ThrowIfNull(persistence);
        var registry = new GewV1PrescribedWorkflowRegistry();
        var git = new GitHeadCommitResolver();
        var effectiveConfigurationResolver = new GewV1EffectiveConfigurationResolver(registry);
        var workflowService = new WorkflowInstanceService(
            persistence.WorkflowInstances,
            registry,
            git);
        var relationshipService = new WorkflowRelationshipService(
            persistence.WorkflowInstances,
            persistence.WorkflowOrigins,
            persistence.WorkflowDependencies);
        var developmentWorkAuthorizationService = new DevelopmentWorkAuthorizationService(
            persistence.WorkflowInstances,
            persistence.DevelopmentWorkAuthorizations,
            effectiveConfigurationResolver,
            persistence);
        var workflowInstanceStopService = new WorkflowInstanceStopService(
            persistence.WorkflowInstances,
            persistence.WorkflowInstanceStops);
        var recovery = new GovernedWorkStateRecoveryService(
            persistence.WorkflowInstances,
            persistence.WorkflowOrigins,
            persistence.WorkflowDependencies,
            persistence.DevelopmentWorkAuthorizations,
            persistence.WorkflowInstanceStops,
            registry);
        var projection = new GovernedWorkStateOperatorProjectionService(
            recovery,
            effectiveConfigurationResolver,
            registry,
            git);
        return new WorkflowApplicationServices(
            workflowService,
            relationshipService,
            developmentWorkAuthorizationService,
            workflowInstanceStopService,
            effectiveConfigurationResolver,
            recovery,
            projection);
    }
}
