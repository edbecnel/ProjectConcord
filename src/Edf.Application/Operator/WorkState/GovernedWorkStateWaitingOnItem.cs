using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public sealed record GovernedWorkStateWaitingOnItem(
    WorkflowInstanceId WorkflowInstanceId,
    string ReasonCode,
    IReadOnlyList<WorkflowInstanceId> RelatedWorkflowInstanceIds);
