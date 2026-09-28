namespace Edf.Domain.Projects;

public sealed record ManagedProject(
    ProjectConcordProjectId ProjectId,
    string DisplayName,
    ProjectLocator RegisteredLocator,
    DateTimeOffset CreatedUtc,
    DateTimeOffset LastOpenedUtc);
