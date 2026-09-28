using Edf.Domain.Projects;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Projects;

public sealed class ProjectWorkspaceService : IProjectWorkspaceService
{
    private readonly ProjectRootResolver _rootResolver;
    private readonly IProjectRegistry _projectRegistry;
    private readonly IUserPreferencesStore _userPreferences;
    private readonly ILocalProjectRuntime _localRuntime;

    public ProjectWorkspaceService(
        ProjectRootResolver rootResolver,
        ICurrentProjectActor currentActor,
        IProjectRegistry projectRegistry,
        IUserPreferencesStore userPreferences,
        ILocalProjectRuntime localRuntime)
    {
        _rootResolver = rootResolver ?? throw new ArgumentNullException(nameof(rootResolver));
        CurrentActor = currentActor ?? throw new ArgumentNullException(nameof(currentActor));
        _projectRegistry = projectRegistry ?? throw new ArgumentNullException(nameof(projectRegistry));
        _userPreferences = userPreferences ?? throw new ArgumentNullException(nameof(userPreferences));
        _localRuntime = localRuntime ?? throw new ArgumentNullException(nameof(localRuntime));
    }

    public ICurrentProjectActor CurrentActor { get; }

    public ProjectConcordProjectId? CurrentProjectId => _localRuntime.CurrentProjectId;

    public ProjectRoot? CurrentRoot => _localRuntime.CurrentProjectRoot;

    public OpenProjectResult OpenProjectRoot(string absolutePath)
    {
        var previousProjectId = CurrentProjectId;
        var previousRoot = CurrentRoot;

        try
        {
            var root = _rootResolver.Resolve(absolutePath);
            var locator = ProjectLocator.FromPath(root.AbsolutePath);
            var openedUtc = DateTimeOffset.UtcNow;
            var displayName = new DirectoryInfo(locator.NormalizedAbsolutePath).Name;

            var existing = _projectRegistry.ResolveByRegisteredLocator(locator);
            var project = existing
                ?? _projectRegistry.RegisterNewProjectAtLocator(locator, displayName, openedUtc);

            _projectRegistry.RecordSuccessfulOpen(project.ProjectId, openedUtc);
            ActivateSession(project.ProjectId, root);
            _userPreferences.SetLastActiveProjectId(project.ProjectId);
            return OpenProjectResult.Succeeded(root, project.ProjectId);
        }
        catch (Exception ex)
        {
            RestoreSession(previousProjectId, previousRoot);
            return OpenProjectResult.Failed(ex.Message);
        }
    }

    public OpenProjectResult OpenProjectById(ProjectConcordProjectId projectId)
    {
        var previousProjectId = CurrentProjectId;
        var previousRoot = CurrentRoot;

        var project = _projectRegistry.GetById(projectId);
        if (project is null)
        {
            return OpenProjectResult.Failed("Project was not found.");
        }

        if (LocatorAvailabilityEvaluator.Evaluate(project.RegisteredLocator) != LocatorAvailability.Available)
        {
            return OpenProjectResult.Failed("Registered project locator is missing on disk.");
        }

        try
        {
            var root = _rootResolver.Resolve(project.RegisteredLocator.NormalizedAbsolutePath);
            var openedUtc = DateTimeOffset.UtcNow;
            _projectRegistry.RecordSuccessfulOpen(projectId, openedUtc);
            ActivateSession(projectId, root);
            _userPreferences.SetLastActiveProjectId(projectId);
            return OpenProjectResult.Succeeded(root, projectId);
        }
        catch (Exception ex)
        {
            RestoreSession(previousProjectId, previousRoot);
            return OpenProjectResult.Failed(ex.Message);
        }
    }

    public ReconcileLocatorResult ReconcileProjectLocator(ProjectConcordProjectId projectId, string newAbsolutePath)
    {
        try
        {
            var root = _rootResolver.Resolve(newAbsolutePath);
            var locator = ProjectLocator.FromPath(root.AbsolutePath);
            var displayName = new DirectoryInfo(locator.NormalizedAbsolutePath).Name;
            var reconciledUtc = DateTimeOffset.UtcNow;

            var result = _projectRegistry.ReconcileProjectLocator(projectId, locator, displayName, reconciledUtc);
            if (!result.Success || result.Project is null)
            {
                return result;
            }

            if (CurrentProjectId is { } currentId && currentId.Value == projectId.Value)
            {
                ActivateSession(projectId, root);
            }

            if (_userPreferences.GetLastActiveProjectId() is { } lastActive
                && lastActive.Value == projectId.Value)
            {
                _userPreferences.SetLastActiveProjectId(projectId);
            }

            return result;
        }
        catch (Exception ex)
        {
            return ReconcileLocatorResult.Failed(ex.Message);
        }
    }

    public void CloseProject()
    {
        _localRuntime.ClearSession();
    }

    public IReadOnlyList<RecentProjectEntry> ListRecentProjects() =>
        _projectRegistry.ListRecent(_userPreferences.GetLastActiveProjectId());

    public void RemoveFromRecent(ProjectConcordProjectId projectId) =>
        _projectRegistry.RemoveFromRecent(projectId);

    private void ActivateSession(ProjectConcordProjectId projectId, ProjectRoot root) =>
        _localRuntime.SetSession(projectId, root);

    private void RestoreSession(ProjectConcordProjectId? projectId, ProjectRoot? root)
    {
        if (projectId is { } id && root is not null)
        {
            _localRuntime.SetSession(id, root);
            return;
        }

        _localRuntime.ClearSession();
    }
}
