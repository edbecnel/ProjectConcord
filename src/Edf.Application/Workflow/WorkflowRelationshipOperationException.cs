namespace Edf.Application.Workflow;

public sealed class WorkflowRelationshipOperationException : Exception
{
    public WorkflowRelationshipOperationException(string message)
        : base(message)
    {
    }
}
