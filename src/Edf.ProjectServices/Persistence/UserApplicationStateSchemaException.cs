namespace Edf.ProjectServices.Persistence;

public sealed class UserApplicationStateSchemaException : Exception
{
    public UserApplicationStateSchemaException(string message)
        : base(message)
    {
    }
}
