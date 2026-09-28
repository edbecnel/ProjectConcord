using Edf.Domain.Projects;

namespace Edf.Application.Projects;

public sealed class OpenProjectResult
{
    private OpenProjectResult(bool success, ProjectRoot? root, string? errorMessage)
    {
        Success = success;
        Root = root;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; }

    public ProjectRoot? Root { get; }

    public string? ErrorMessage { get; }

    public static OpenProjectResult Succeeded(ProjectRoot root) => new(true, root, null);

    public static OpenProjectResult Failed(string errorMessage) =>
        new(false, null, errorMessage ?? "Unknown error.");
}
