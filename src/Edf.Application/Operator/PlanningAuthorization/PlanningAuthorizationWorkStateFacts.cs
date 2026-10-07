namespace Edf.Application.Operator.PlanningAuthorization;

using Edf.Application.Operator.WorkState;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Workflow;

public static class PlanningAuthorizationWorkStateFacts
{
    public static bool InstanceNeedsPlanningDevelopmentWorkAuthorization(
        GovernedWorkStateOperatorProjection projection)
    {
        if (projection.CurrentWork.Count != 1)
        {
            return false;
        }

        var work = projection.CurrentWork[0];
        return work.TopologyPlaceId.Value == GewV1TopologyPlaces.PlanningGoverned
               && work.GovernedEligibility.EvaluatedConstraintViolationCodes.Contains(
                   WorkflowEligibilityReasonCodes.BlockedAuthorization);
    }
}
