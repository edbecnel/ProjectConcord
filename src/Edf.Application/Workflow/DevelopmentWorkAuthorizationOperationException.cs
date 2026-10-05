namespace Edf.Application.Workflow;

public sealed class DevelopmentWorkAuthorizationOperationException : Exception
{
    public DevelopmentWorkAuthorizationOperationException(string message)
        : base(message)
    {
    }
}
