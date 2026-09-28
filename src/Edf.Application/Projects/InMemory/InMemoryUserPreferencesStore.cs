using Edf.Domain.Projects;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryUserPreferencesStore : IUserPreferencesStore
{
    private ProjectConcordProjectId? _lastActiveProjectId;

    public ProjectConcordProjectId? GetLastActiveProjectId() => _lastActiveProjectId;

    public void SetLastActiveProjectId(ProjectConcordProjectId? projectId) => _lastActiveProjectId = projectId;
}
