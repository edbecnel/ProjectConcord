namespace Edf.Application.Workflow;

public sealed class WorkflowInstanceStopOperationException : Exception
{
    public WorkflowInstanceStopOperationException(string message)
        : base(message)
    {
    }
}
