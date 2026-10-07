using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public static class PlanningRegionWorkContinuationFacts
{
    public static bool InstanceShouldOfferGovernedExchangeContinuation(
        GovernedWorkStateOperatorProjection projection,
        WorkflowOperatorNextActionItem? primaryNextAction)
    {
        if (projection.CurrentWork.Count != 1)
        {
            return false;
        }

        var work = projection.CurrentWork[0];
        if (work.TopologyPlaceId.Value != GewV1TopologyPlaces.PlanningGoverned)
        {
            return false;
        }

        if (PlanningAuthorizationWorkStateFacts.InstanceNeedsPlanningDevelopmentWorkAuthorization(projection))
        {
            return false;
        }

        if (work.StopActive)
        {
            return false;
        }

        if (!work.GovernedEligibility.EligibleUnderEvaluatedConstraints)
        {
            return false;
        }

        if (work.ApplicableActiveAuthorizationKinds.Contains(DevelopmentWorkAuthorizationKind.Implementation))
        {
            return false;
        }

        if (!work.ApplicableActiveAuthorizationKinds.Contains(DevelopmentWorkAuthorizationKind.Planning))
        {
            return false;
        }

        if (primaryNextAction is null)
        {
            return false;
        }

        if (primaryNextAction.WorkflowInstanceId != work.InstanceId)
        {
            return false;
        }

        return primaryNextAction.SuggestedStepCode
               == WorkflowEligibilityReasonCodes.NextActionPlanningRegionWork;
    }
}
