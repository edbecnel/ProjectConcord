using Edf.Domain.Workflow;

namespace Edf.Application.Workflow.Eligibility;

public interface IGewV1WorkflowInstanceEligibilityEvaluator
{
    WorkflowInstanceGovernedEligibility Evaluate(
        WorkflowInstance instance,
        IReadOnlyList<WorkflowInstanceId> unresolvedRequiredWorkflowInstanceIds,
        bool instanceStopActive,
        IReadOnlyList<DevelopmentWorkAuthorization> authorizationRecordsForInstance,
        EffectiveConfigurationResolveResult effectiveConfigurationResult);
}
