using Edf.Domain.Projects;

namespace Edf.ProjectServices.Local;

public sealed class LocalProjectRuntime : ILocalProjectRuntime
{
    public ProjectConcordProjectId? CurrentProjectId { get; private set; }

    public ProjectRoot? CurrentProjectRoot { get; private set; }

    public void SetSession(ProjectConcordProjectId projectId, ProjectRoot root)
    {
        CurrentProjectId = projectId;
        CurrentProjectRoot = root ?? throw new ArgumentNullException(nameof(root));
    }

    public void ClearSession()
    {
        CurrentProjectId = null;
        CurrentProjectRoot = null;
    }
}
