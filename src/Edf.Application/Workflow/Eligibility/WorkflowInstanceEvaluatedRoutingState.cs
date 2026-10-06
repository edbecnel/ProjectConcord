namespace Edf.Application.Workflow.Eligibility;

public enum WorkflowInstanceEvaluatedRoutingState
{
    NonActive = 0,
    IntakeAwaitingGovernedPlanningEntry = 1,
    PlanningGovernedRegion = 2,
    ImplementationGovernedRegion = 3,
    PostSubmissionAwaitingGovernance = 4,
}
