namespace Edf.Domain.Projects;

public sealed record RecentProjectEntry(
    ProjectConcordProjectId ProjectId,
    string DisplayName,
    ProjectLocator RegisteredLocator,
    LocatorAvailability LocatorAvailability,
    DateTimeOffset LastOpenedUtc,
    bool IsLastActive);
