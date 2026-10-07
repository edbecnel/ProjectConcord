namespace Edf.Application.Operator.PlanningRegion;

using Edf.Application.Operator.WorkState;
using Edf.Domain.Workflow;

public static class PlanningRegionGuidedStepResolver
{
    public static PlanningRegionGuidedStep Resolve(
        GovernedWorkStateOperatorProjection projection,
        PlanningRegionRelayReadModelSnapshot relay,
        PlanningRegionGuidedTransientState transient,
        bool sessionIntentsReadyForReviewExport,
        bool requiresOperatorSubjectConfirmation)
    {
        if (requiresOperatorSubjectConfirmation)
        {
            return PlanningRegionGuidedStep.ConfirmSubject;
        }

        var primaryNextAction = SelectPrimaryNextAction(projection);
        if (!PlanningRegionWorkContinuationFacts.InstanceShouldOfferGovernedExchangeContinuation(
                projection,
                primaryNextAction))
        {
            if (projection.CurrentWork.Count == 1 && projection.CurrentWork[0].StopActive)
            {
                return PlanningRegionGuidedStep.BlockedByStop;
            }

            return PlanningRegionGuidedStep.Inactive;
        }

        if (projection.CurrentWork.Count == 1 && projection.CurrentWork[0].StopActive)
        {
            return PlanningRegionGuidedStep.BlockedByStop;
        }

        if (transient.LastCommittedHandoverPackage is not null
            && transient.LastCommittedHandoverValidation is not null
            && transient.LastCommittedHandoverValidation.IsEligibleForValidatedEngineeringAgentHandover)
        {
            return PlanningRegionGuidedStep.SendToEngineeringAgent;
        }

        if (relay.LatestConsumedPaHandover is not null
            && relay.LatestConsumedPaHandover.Validation.IsEligibleForValidatedEngineeringAgentHandover
            && relay.LatestPaReviewExport is not null)
        {
            return PlanningRegionGuidedStep.SendToEngineeringAgent;
        }

        if (!string.IsNullOrWhiteSpace(transient.PaResponseDraft)
            || transient.LastValidationAttemptFailed
            || transient.AwaitingPaResponseAcknowledged
            || transient.ReviewTransferred)
        {
            return PlanningRegionGuidedStep.BringBackPaResponse;
        }

        if (relay.LatestPaReviewExport is not null && relay.LatestConsumedPaHandover is null)
        {
            return PlanningRegionGuidedStep.BringBackPaResponse;
        }

        if (!sessionIntentsReadyForReviewExport)
        {
            return PlanningRegionGuidedStep.ConfirmSessionContinuity;
        }

        return PlanningRegionGuidedStep.SendToProjectArchitect;
    }

    private static WorkflowOperatorNextActionItem? SelectPrimaryNextAction(
        GovernedWorkStateOperatorProjection projection)
    {
        foreach (var action in projection.WorkflowNextActions)
        {
            return action;
        }

        return null;
    }
}
