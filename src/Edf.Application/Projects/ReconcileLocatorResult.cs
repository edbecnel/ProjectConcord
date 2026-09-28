using Edf.Domain.Projects;

namespace Edf.Application.Projects;

public sealed class ReconcileLocatorResult
{
    private ReconcileLocatorResult(bool success, ManagedProject? project, string? errorMessage)
    {
        Success = success;
        Project = project;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; }

    public ManagedProject? Project { get; }

    public string? ErrorMessage { get; }

    public static ReconcileLocatorResult Succeeded(ManagedProject project) => new(true, project, null);

    public static ReconcileLocatorResult Failed(string errorMessage) =>
        new(false, null, errorMessage ?? "Reconciliation failed.");
}
