using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public sealed record GovernedWorkStateOperatorProjection(
    IReadOnlyList<GovernedWorkStateCurrentWorkItem> CurrentWork,
    IReadOnlyList<WorkflowInstanceId> CandidateFrontierInstanceIds,
    ProjectionAvailability FullyGovernedFrontierAvailability,
    string? FullyGovernedFrontierUnavailableReason,
    ProjectionAvailability WaitingOnAvailability,
    string? WaitingOnUnavailableReason,
    IReadOnlyList<GovernedWorkStateWaitingOnItem> WaitingOnItems,
    ProjectionAvailability NextActionAvailability,
    string? NextActionUnavailableReason,
    IReadOnlyList<WorkflowOperatorNextActionItem> WorkflowNextActions);
