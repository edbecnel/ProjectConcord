using Edf.Application.Operator;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public sealed record WorkflowOperatorNextActionItem(
    WorkflowInstanceId WorkflowInstanceId,
    string SuggestedStepCode,
    GovernedExecutionPermission PermissionToExecute,
    OperatorNextActionClass ActionClass,
    string Message);
