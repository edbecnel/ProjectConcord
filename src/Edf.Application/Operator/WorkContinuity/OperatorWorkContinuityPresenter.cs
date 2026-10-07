namespace Edf.Application.Operator.WorkContinuity;

using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Operator.PlanningRegion;
using Edf.Application.Operator.WorkState;
using Edf.Domain.Workflow;

public static class OperatorWorkContinuityPresenter
{
    public static string ComposeWhatJustHappened(
        GovernedWorkStateOperatorProjection projection,
        PlanningRegionRelayReadModelSnapshot relay)
    {
        if (projection.CurrentWork.Count == 1 && projection.CurrentWork[0].StopActive)
        {
            return "STOP is active on this workflow instance. Normal guided planning progression is blocked until STOP is cleared under governed rules.";
        }

        if (relay.LatestConsumedPaHandover is not null)
        {
            return "A Project Architect response was validated and recorded in relay provenance for this project.";
        }

        if (relay.LatestPaReviewExport is not null)
        {
            return "A governed review package was prepared for Project Architect exchange. Transfer it to your Project Architect conversation if you have not already.";
        }

        if (projection.CurrentWork.Count == 1
            && projection.CurrentWork[0].ApplicableActiveAuthorizationKinds.Contains(
                DevelopmentWorkAuthorizationKind.Planning)
            && !PlanningAuthorizationWorkStateFacts.InstanceNeedsPlanningDevelopmentWorkAuthorization(projection))
        {
            return "Planning development work authorization was recorded. Planning work may continue under that governed scope.";
        }

        return "Continue from the current governed planning position using the guided steps below.";
    }

    public static string ComposeRecentContinuityForPersistence(
        GovernedWorkStateOperatorProjection projection,
        PlanningRegionRelayReadModelSnapshot relay)
    {
        var narrative = ComposeWhatJustHappened(projection, relay);
        return narrative.Length > 512 ? narrative[..512] : narrative;
    }
}
