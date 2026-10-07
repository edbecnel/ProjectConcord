namespace Edf.Application.Operator.PlanningAuthorization;

using Edf.Application.Workflow.PlanningAuthorization;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;

public static class PlanningAuthorizationGuidedStepResolver
{
    public static PlanningAuthorizationGuidedStep Resolve(
        string? topologyPlaceId,
        bool instanceNeedsPlanningDwa,
        PlanningAuthorizationRelayReadModelSnapshot relay,
        PlanningAuthorizationGuidedTransientState transient,
        IGewV1PlanningDevelopmentWorkAuthorizationGrantService grantService,
        ProjectConcordProjectId projectId,
        bool sessionIntentsReadyForReviewExport)
    {
        if (topologyPlaceId != GewV1TopologyPlaces.PlanningGoverned)
        {
            return PlanningAuthorizationGuidedStep.Inactive;
        }

        if (!instanceNeedsPlanningDwa)
        {
            return PlanningAuthorizationGuidedStep.Complete;
        }

        var consumed = relay.LatestConsumedPaHandover;
        if (consumed is not null
            && grantService.EvaluateGrantEligibility(projectId, consumed.Package, consumed.Validation).IsEligible)
        {
            if (transient.LastValidationAttemptFailed && !string.IsNullOrWhiteSpace(transient.PaResponseDraft))
            {
                return PlanningAuthorizationGuidedStep.ValidateResponse;
            }

            return PlanningAuthorizationGuidedStep.ReviewDecision;
        }

        if (!string.IsNullOrWhiteSpace(transient.PaResponseDraft)
            || transient.LastValidationAttemptFailed)
        {
            return PlanningAuthorizationGuidedStep.ValidateResponse;
        }

        if (transient.AwaitingPaResponseAcknowledged)
        {
            return PlanningAuthorizationGuidedStep.ValidateResponse;
        }

        if (relay.LatestPaReviewExport is not null || !string.IsNullOrWhiteSpace(transient.CachedRenderedReview))
        {
            return PlanningAuthorizationGuidedStep.SendReview;
        }

        if (!sessionIntentsReadyForReviewExport)
        {
            return PlanningAuthorizationGuidedStep.ConfirmSessionContinuity;
        }

        return PlanningAuthorizationGuidedStep.PrepareReview;
    }

    public static int StepNumber(PlanningAuthorizationGuidedStep step) =>
        step switch
        {
            PlanningAuthorizationGuidedStep.ConfirmSessionContinuity => 1,
            PlanningAuthorizationGuidedStep.PrepareReview => 1,
            PlanningAuthorizationGuidedStep.SendReview => 2,
            PlanningAuthorizationGuidedStep.ValidateResponse => 3,
            PlanningAuthorizationGuidedStep.ReviewDecision => 4,
            _ => 0,
        };

    public static int StepCount(PlanningAuthorizationGuidedStep step) =>
        step == PlanningAuthorizationGuidedStep.Complete ? 4 : 4;
}
