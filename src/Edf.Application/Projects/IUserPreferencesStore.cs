using Edf.Domain.Projects;

namespace Edf.Application.Projects;

public interface IUserPreferencesStore
{
    ProjectConcordProjectId? GetLastActiveProjectId();

    void SetLastActiveProjectId(ProjectConcordProjectId? projectId);
}
