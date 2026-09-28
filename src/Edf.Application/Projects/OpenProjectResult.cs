using Edf.Domain.Projects;

namespace Edf.Application.Projects;

public sealed class OpenProjectResult
{
    private OpenProjectResult(bool success, ProjectRoot? root, ProjectConcordProjectId? projectId, string? errorMessage)
    {
        Success = success;
        Root = root;
        ProjectId = projectId;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; }

    public ProjectRoot? Root { get; }

    public ProjectConcordProjectId? ProjectId { get; }

    public string? ErrorMessage { get; }

    public static OpenProjectResult Succeeded(ProjectRoot root, ProjectConcordProjectId projectId) =>
        new(true, root, projectId, null);

    public static OpenProjectResult Failed(string errorMessage) =>
        new(false, null, null, errorMessage ?? "Unknown error.");
}
