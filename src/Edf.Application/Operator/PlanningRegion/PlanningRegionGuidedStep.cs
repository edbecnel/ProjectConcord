namespace Edf.Application.Operator.PlanningRegion;

public enum PlanningRegionGuidedStep
{
    Inactive = 0,
    ConfirmSubject = 1,
    BlockedByStop = 2,
    ConfirmSessionContinuity = 3,
    SendToProjectArchitect = 4,
    BringBackPaResponse = 5,
    SendToEngineeringAgent = 6,
    Complete = 7,
}
