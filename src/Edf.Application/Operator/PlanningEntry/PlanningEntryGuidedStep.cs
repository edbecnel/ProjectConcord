namespace Edf.Application.Operator.PlanningEntry;

public enum PlanningEntryGuidedStep
{
    Inactive = 0,
    ConfirmSessionContinuity = 1,
    PrepareReview = 2,
    SendReview = 3,
    ValidateResponse = 4,
    ReviewDecision = 5,
    Complete = 6,
}
