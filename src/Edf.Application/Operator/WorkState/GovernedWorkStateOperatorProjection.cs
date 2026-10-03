using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public sealed record GovernedWorkStateOperatorProjection(
    IReadOnlyList<GovernedWorkStateCurrentWorkItem> CurrentWork,
    ProjectionAvailability WaitingOnAvailability,
    string? WaitingOnUnavailableReason,
    ProjectionAvailability NextActionAvailability,
    string? NextActionUnavailableReason);
