using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public interface IWorkflowInstanceStopService
{
    WorkflowInstanceStopSummary RecordGovernedStopSet(
        WorkflowInstanceId workflowInstanceId,
        long? expectedStopResourceVersion,
        GovernedWorkflowMutationAuthority authority);

    WorkflowInstanceStopSummary RecordGovernedStopClear(
        WorkflowInstanceId workflowInstanceId,
        long expectedStopResourceVersion,
        GovernedWorkflowMutationAuthority authority);
}
