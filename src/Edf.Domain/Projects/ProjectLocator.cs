namespace Edf.Domain.Projects;

/// <summary>
/// Registered Project Root filesystem locator (observation, not canonical identity).
/// </summary>
public sealed record ProjectLocator(string NormalizedAbsolutePath)
{
    public static ProjectLocator FromPath(string absolutePath)
    {
        if (string.IsNullOrWhiteSpace(absolutePath))
        {
            throw new ArgumentException("Locator path is required.", nameof(absolutePath));
        }

        return new ProjectLocator(Path.GetFullPath(absolutePath));
    }
}
