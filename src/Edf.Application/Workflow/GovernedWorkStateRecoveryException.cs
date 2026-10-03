namespace Edf.Application.Workflow;

public sealed class GovernedWorkStateRecoveryException : Exception
{
    public GovernedWorkStateRecoveryException(string message)
        : base(message)
    {
    }
}
