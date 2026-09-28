namespace Edf.Domain.Projects;

/// <summary>
/// Absolute filesystem path selected as the ProjectConcord project root (M1 skeleton — not EDF discovery).
/// </summary>
public sealed record ProjectRoot(string AbsolutePath)
{
    public static ProjectRoot Create(string absolutePath)
    {
        if (string.IsNullOrWhiteSpace(absolutePath))
        {
            throw new ArgumentException("Project root path is required.", nameof(absolutePath));
        }

        var fullPath = Path.GetFullPath(absolutePath);

        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"Project root directory was not found: {fullPath}");
        }

        return new ProjectRoot(fullPath);
    }
}
