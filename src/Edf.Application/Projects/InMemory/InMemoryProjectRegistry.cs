using Edf.Domain.Projects;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryProjectRegistry : IProjectRegistry
{
    public const int DefaultMaxRecentProjects = 10;

    private readonly Dictionary<Guid, ManagedProject> _projectsById = new();
    private readonly Dictionary<string, ProjectConcordProjectId> _idByLocatorPath = new(StringComparer.Ordinal);
    private readonly List<ProjectConcordProjectId> _recentOrder = new();
    private readonly object _sync = new();

    public InMemoryProjectRegistry(int maxRecentProjects = DefaultMaxRecentProjects)
    {
        if (maxRecentProjects < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRecentProjects));
        }

        MaxRecentProjects = maxRecentProjects;
    }

    public int MaxRecentProjects { get; }

    public ManagedProject RegisterNewProjectAtLocator(ProjectLocator locator, string displayName, DateTimeOffset openedUtc)
    {
        lock (_sync)
        {
            if (_idByLocatorPath.ContainsKey(locator.NormalizedAbsolutePath))
            {
                throw new InvalidOperationException("Locator is already registered to a project.");
            }

            var projectId = ProjectConcordProjectId.New();
            var project = new ManagedProject(
                projectId,
                displayName,
                locator,
                openedUtc,
                openedUtc);

            _projectsById[projectId.Value] = project;
            _idByLocatorPath[locator.NormalizedAbsolutePath] = projectId;
            TouchRecent(projectId);
            return project;
        }
    }

    public ManagedProject? ResolveByRegisteredLocator(ProjectLocator locator)
    {
        lock (_sync)
        {
            if (!_idByLocatorPath.TryGetValue(locator.NormalizedAbsolutePath, out var projectId))
            {
                return null;
            }

            return _projectsById.GetValueOrDefault(projectId.Value);
        }
    }

    public ManagedProject? GetById(ProjectConcordProjectId projectId)
    {
        lock (_sync)
        {
            return _projectsById.GetValueOrDefault(projectId.Value);
        }
    }

    public IReadOnlyList<RecentProjectEntry> ListRecent(ProjectConcordProjectId? lastActiveProjectId)
    {
        lock (_sync)
        {
            var entries = new List<RecentProjectEntry>(_recentOrder.Count);
            foreach (var projectId in _recentOrder)
            {
                if (!_projectsById.TryGetValue(projectId.Value, out var project))
                {
                    continue;
                }

                entries.Add(ToRecentEntry(project, lastActiveProjectId));
            }

            return entries;
        }
    }

    public void RecordSuccessfulOpen(ProjectConcordProjectId projectId, DateTimeOffset openedUtc)
    {
        lock (_sync)
        {
            if (!_projectsById.TryGetValue(projectId.Value, out var existing))
            {
                throw new InvalidOperationException("Project is not registered.");
            }

            var updated = existing with { LastOpenedUtc = openedUtc };
            _projectsById[projectId.Value] = updated;
            TouchRecent(projectId);
        }
    }

    public void RemoveFromRecent(ProjectConcordProjectId projectId)
    {
        lock (_sync)
        {
            _recentOrder.RemoveAll(id => id.Value == projectId.Value);
        }
    }

    public ReconcileLocatorResult ReconcileProjectLocator(
        ProjectConcordProjectId projectId,
        ProjectLocator newLocator,
        string displayName,
        DateTimeOffset reconciledUtc)
    {
        lock (_sync)
        {
            if (!_projectsById.TryGetValue(projectId.Value, out var existing))
            {
                return ReconcileLocatorResult.Failed("Project was not found.");
            }

            if (_idByLocatorPath.TryGetValue(newLocator.NormalizedAbsolutePath, out var ownerId)
                && ownerId.Value != projectId.Value)
            {
                return ReconcileLocatorResult.Failed("Locator is already registered to another project.");
            }

            _idByLocatorPath.Remove(existing.RegisteredLocator.NormalizedAbsolutePath);

            var updated = existing with
            {
                DisplayName = displayName,
                RegisteredLocator = newLocator,
                LastOpenedUtc = reconciledUtc,
            };

            _projectsById[projectId.Value] = updated;
            _idByLocatorPath[newLocator.NormalizedAbsolutePath] = projectId;
            TouchRecent(projectId);
            return ReconcileLocatorResult.Succeeded(updated);
        }
    }

    private void TouchRecent(ProjectConcordProjectId projectId)
    {
        _recentOrder.RemoveAll(id => id.Value == projectId.Value);
        _recentOrder.Insert(0, projectId);

        while (_recentOrder.Count > MaxRecentProjects)
        {
            _recentOrder.RemoveAt(_recentOrder.Count - 1);
        }
    }

    private RecentProjectEntry ToRecentEntry(ManagedProject project, ProjectConcordProjectId? lastActiveProjectId)
    {
        var availability = LocatorAvailabilityEvaluator.Evaluate(project.RegisteredLocator);
        var isLastActive = lastActiveProjectId is { } lastActive && lastActive.Value == project.ProjectId.Value;
        return new RecentProjectEntry(
            project.ProjectId,
            project.DisplayName,
            project.RegisteredLocator,
            availability,
            project.LastOpenedUtc,
            isLastActive);
    }
}
