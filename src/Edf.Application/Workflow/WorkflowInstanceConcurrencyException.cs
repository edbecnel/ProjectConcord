using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class WorkflowInstanceConcurrencyException : Exception
{
    public WorkflowInstanceConcurrencyException(WorkflowInstanceId instanceId, long expectedResourceVersion)
        : base(
            $"Workflow instance '{instanceId}' update rejected: expected resource version {expectedResourceVersion} is stale or the instance does not exist.")
    {
        InstanceId = instanceId;
        ExpectedResourceVersion = expectedResourceVersion;
    }

    public WorkflowInstanceId InstanceId { get; }

    public long ExpectedResourceVersion { get; }
}
