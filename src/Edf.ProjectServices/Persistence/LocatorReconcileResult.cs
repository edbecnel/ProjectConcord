using Edf.Domain.Projects;

namespace Edf.ProjectServices.Persistence;

public readonly record struct LocatorReconcileResult(bool Success, ManagedProject? Project, string? ErrorMessage)
{
    public static LocatorReconcileResult Succeeded(ManagedProject project) => new(true, project, null);

    public static LocatorReconcileResult Failed(string errorMessage) => new(false, null, errorMessage);
}
