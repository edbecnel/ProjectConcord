using Edf.Domain.Projects;

namespace Edf.Application.Projects;

public interface IProjectRegistry
{
    int MaxRecentProjects { get; }

    ManagedProject RegisterNewProjectAtLocator(ProjectLocator locator, string displayName, DateTimeOffset openedUtc);

    ManagedProject? ResolveByRegisteredLocator(ProjectLocator locator);

    ManagedProject? GetById(ProjectConcordProjectId projectId);

    IReadOnlyList<RecentProjectEntry> ListRecent(ProjectConcordProjectId? lastActiveProjectId);

    void RecordSuccessfulOpen(ProjectConcordProjectId projectId, DateTimeOffset openedUtc);

    void RemoveFromRecent(ProjectConcordProjectId projectId);

    ReconcileLocatorResult ReconcileProjectLocator(
        ProjectConcordProjectId projectId,
        ProjectLocator newLocator,
        string displayName,
        DateTimeOffset reconciledUtc);
}
