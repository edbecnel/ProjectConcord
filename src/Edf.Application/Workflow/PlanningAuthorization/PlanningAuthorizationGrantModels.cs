namespace Edf.Application.Workflow.PlanningAuthorization;

using Edf.Domain.Workflow;

public enum PlanningAuthorizationGrantOutcome
{
    Succeeded = 0,
    NotEligible = 1,
    AlreadyApplied = 2,
}

public sealed record PlanningAuthorizationGrantEligibility(
    bool IsEligible,
    string ReasonCode,
    WorkflowInstanceId? WorkflowInstanceId);

public sealed record PlanningAuthorizationGrantResult(
    PlanningAuthorizationGrantOutcome Outcome,
    string ReasonCode,
    DevelopmentWorkAuthorization? Authorization,
    string OperatorMessage);
