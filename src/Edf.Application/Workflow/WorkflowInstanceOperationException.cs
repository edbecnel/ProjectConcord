namespace Edf.Application.Workflow;

public sealed class WorkflowInstanceOperationException : Exception
{
    public WorkflowInstanceOperationException(string message)
        : base(message)
    {
    }

    public WorkflowInstanceOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
