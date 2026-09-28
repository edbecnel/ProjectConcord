using Edf.Domain.Projects;

namespace Edf.Engine.Projects;

/// <summary>
/// Resolves a user-selected directory into a <see cref="ProjectRoot"/> without EDF artifact processing.
/// </summary>
public sealed class ProjectRootResolver
{
    public ProjectRoot Resolve(string absolutePath) => ProjectRoot.Create(absolutePath);
}
