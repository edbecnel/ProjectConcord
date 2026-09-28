using Edf.Application.Projects;
using Edf.Domain.Projects;
using Edf.Identity.Actors;

namespace Edf.Desktop.Tests;

internal sealed class FakeProjectWorkspaceService : IProjectWorkspaceService
{
    private readonly List<RecentProjectEntry> _recent = new();
    private ProjectRoot? _currentRoot;
    private ProjectConcordProjectId? _currentProjectId = null;

    public int OpenProjectRootCallCount { get; private set; }

    public int OpenProjectByIdCallCount { get; private set; }

    public int ListRecentProjectsCallCount { get; private set; }

    public ICurrentProjectActor CurrentActor { get; } = new DegenerateAdministratorActor("test");

    public ProjectConcordProjectId? CurrentProjectId => _currentProjectId;

    public ProjectRoot? CurrentRoot => _currentRoot;

    public void SetRecentProjects(IEnumerable<RecentProjectEntry> entries)
    {
        _recent.Clear();
        _recent.AddRange(entries);
    }

    public OpenProjectResult OpenProjectRoot(string absolutePath)
    {
        OpenProjectRootCallCount++;
        return OpenProjectResult.Failed("Not configured.");
    }

    public OpenProjectResult OpenProjectById(ProjectConcordProjectId projectId)
    {
        OpenProjectByIdCallCount++;
        return OpenProjectResult.Failed("Not configured.");
    }

    public ReconcileLocatorResult ReconcileProjectLocator(
        ProjectConcordProjectId projectId,
        string newAbsolutePath) =>
        ReconcileLocatorResult.Failed("Not configured.");

    public void CloseProject() => _currentRoot = null;

    public IReadOnlyList<RecentProjectEntry> ListRecentProjects()
    {
        ListRecentProjectsCallCount++;
        return _recent.ToList();
    }

    public void RemoveFromRecent(ProjectConcordProjectId projectId) =>
        _recent.RemoveAll(e => e.ProjectId.Value == projectId.Value);
}
