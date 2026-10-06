namespace Edf.Application.Workflow.PlanningEntry;

using Edf.Domain.Workflow;

public enum IntakePlanningEntryTransitionOutcome
{
    Succeeded = 0,
    AlreadyApplied = 1,
    NotEligible = 2,
}

public sealed record IntakePlanningEntryEligibility(
    bool IsEligible,
    string ReasonCode,
    WorkflowInstanceId? ApplicableInstanceId,
    long? ApplicableInstanceResourceVersion);

public sealed record IntakePlanningEntryTransitionResult(
    IntakePlanningEntryTransitionOutcome Outcome,
    string ReasonCode,
    WorkflowInstance? Instance,
    string? OperatorMessage);
