using Edf.Domain.Projects;
using Edf.Identity.Actors;

namespace Edf.Application.Projects;

public interface IProjectWorkspaceService : ICurrentProjectSession
{
    ICurrentProjectActor CurrentActor { get; }

    OpenProjectResult OpenProjectRoot(string absolutePath);

    OpenProjectResult OpenProjectById(ProjectConcordProjectId projectId);

    ReconcileLocatorResult ReconcileProjectLocator(ProjectConcordProjectId projectId, string newAbsolutePath);

    void CloseProject();

    IReadOnlyList<RecentProjectEntry> ListRecentProjects();

    void RemoveFromRecent(ProjectConcordProjectId projectId);
}
