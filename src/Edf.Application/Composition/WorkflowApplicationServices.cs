using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Operator.PlanningRegion;
using Edf.Application.Operator.WorkContinuity;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Application.Workflow.PlanningAuthorization;
using Edf.Application.Workflow.PlanningEntry;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Composition;

public sealed record WorkflowApplicationServices(
    IWorkflowInstanceService WorkflowInstances,
    IWorkflowRelationshipService WorkflowRelationships,
    IDevelopmentWorkAuthorizationService DevelopmentWorkAuthorizations,
    IWorkflowInstanceStopService WorkflowInstanceStops,
    IGewV1EffectiveConfigurationResolver EffectiveConfigurationResolver,
    IGovernedWorkStateRecoveryService WorkStateRecovery,
    GovernedWorkStateOperatorProjectionService WorkStateOperatorProjection,
    IGewV1IntakePlanningEntryTransitionService IntakePlanningEntryTransitions,
    IPlanningEntryRelayReadModel PlanningEntryRelayReadModel,
    IGewV1PlanningDevelopmentWorkAuthorizationGrantService PlanningAuthorizationGrants,
    IPlanningAuthorizationRelayReadModel PlanningAuthorizationRelayReadModel,
    IPlanningRegionRelayReadModel PlanningRegionRelayReadModel,
    OperatorWorkFocusService OperatorWorkFocus);

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
        var eligibilityEvaluator = new GewV1WorkflowInstanceEligibilityEvaluator(registry);
        var projection = new GovernedWorkStateOperatorProjectionService(
            recovery,
            effectiveConfigurationResolver,
            eligibilityEvaluator,
            registry,
            git);
        var intakePlanningEntryTransitions = new GewV1IntakePlanningEntryTransitionService(
            recovery,
            effectiveConfigurationResolver,
            eligibilityEvaluator,
            registry,
            workflowService);
        var planningEntryRelayReadModel = new PlanningEntryRelayReadModel(persistence.RelayOperational);
        var planningAuthorizationGrants = new GewV1PlanningDevelopmentWorkAuthorizationGrantService(
            recovery,
            effectiveConfigurationResolver,
            eligibilityEvaluator,
            registry,
            developmentWorkAuthorizationService);
        var planningAuthorizationRelayReadModel = new PlanningAuthorizationRelayReadModel(persistence.RelayOperational);
        var planningRegionRelayReadModel = new PlanningRegionRelayReadModel(persistence.RelayOperational);
        var operatorWorkFocus = new OperatorWorkFocusService(persistence.OperatorWorkFocus);
        return new WorkflowApplicationServices(
            workflowService,
            relationshipService,
            developmentWorkAuthorizationService,
            workflowInstanceStopService,
            effectiveConfigurationResolver,
            recovery,
            projection,
            intakePlanningEntryTransitions,
            planningEntryRelayReadModel,
            planningAuthorizationGrants,
            planningAuthorizationRelayReadModel,
            planningRegionRelayReadModel,
            operatorWorkFocus);
    }
}
