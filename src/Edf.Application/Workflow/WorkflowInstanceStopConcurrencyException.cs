using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class WorkflowInstanceStopConcurrencyException : Exception
{
    public WorkflowInstanceStopConcurrencyException(WorkflowInstanceId workflowInstanceId, long expectedResourceVersion)
        : base(
            $"Workflow instance stop state for '{workflowInstanceId}' concurrency conflict (expected resource version {expectedResourceVersion}).")
    {
        WorkflowInstanceId = workflowInstanceId;
        ExpectedResourceVersion = expectedResourceVersion;
    }

    public WorkflowInstanceId WorkflowInstanceId { get; }

    public long ExpectedResourceVersion { get; }
}
