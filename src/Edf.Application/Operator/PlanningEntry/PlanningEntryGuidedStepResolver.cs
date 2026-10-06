namespace Edf.Application.Operator.PlanningEntry;

using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

public static class PlanningEntryGuidedStepResolver
{
    public static PlanningEntryGuidedStep Resolve(
        string? topologyPlaceId,
        PlanningEntryRelayReadModelSnapshot relay,
        PlanningEntryGuidedTransientState transient,
        IGewV1IntakePlanningEntryTransitionService planningEntryTransitions,
        ProjectConcordProjectId projectId,
        bool sessionIntentsReadyForReviewExport)
    {
        if (topologyPlaceId == GewV1TopologyPlaces.PlanningGoverned)
        {
            return PlanningEntryGuidedStep.Complete;
        }

        if (topologyPlaceId != GewV1TopologyPlaces.Intake)
        {
            return PlanningEntryGuidedStep.Inactive;
        }

        var consumed = relay.LatestConsumedPaHandover;
        if (consumed is not null
            && planningEntryTransitions.EvaluateEligibility(projectId, consumed.Package, consumed.Validation).IsEligible)
        {
            if (transient.LastValidationAttemptFailed && !string.IsNullOrWhiteSpace(transient.PaResponseDraft))
            {
                return PlanningEntryGuidedStep.ValidateResponse;
            }

            return PlanningEntryGuidedStep.ReviewDecision;
        }

        if (!string.IsNullOrWhiteSpace(transient.PaResponseDraft)
            || transient.LastValidationAttemptFailed)
        {
            return PlanningEntryGuidedStep.ValidateResponse;
        }

        if (transient.AwaitingPaResponseAcknowledged)
        {
            return PlanningEntryGuidedStep.ValidateResponse;
        }

        if (relay.LatestPaReviewExport is not null || !string.IsNullOrWhiteSpace(transient.CachedRenderedReview))
        {
            return PlanningEntryGuidedStep.SendReview;
        }

        if (!sessionIntentsReadyForReviewExport)
        {
            return PlanningEntryGuidedStep.ConfirmSessionContinuity;
        }

        return PlanningEntryGuidedStep.PrepareReview;
    }

    public static int StepNumber(PlanningEntryGuidedStep step) =>
        step switch
        {
            PlanningEntryGuidedStep.ConfirmSessionContinuity => 1,
            PlanningEntryGuidedStep.PrepareReview => 1,
            PlanningEntryGuidedStep.SendReview => 2,
            PlanningEntryGuidedStep.ValidateResponse => 3,
            PlanningEntryGuidedStep.ReviewDecision => 4,
            PlanningEntryGuidedStep.Complete => 4,
            _ => 0,
        };

    public static int StepCount(PlanningEntryGuidedStep step) =>
        step == PlanningEntryGuidedStep.Complete ? 4 : 4;
}
