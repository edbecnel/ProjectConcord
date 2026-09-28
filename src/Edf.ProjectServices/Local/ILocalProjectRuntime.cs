using Edf.Domain.Projects;

namespace Edf.ProjectServices.Local;

/// <summary>
/// Local project runtime session seam (M1 marker expanded for A1a — no project-local derived state).
/// </summary>
public interface ILocalProjectRuntime
{
    ProjectConcordProjectId? CurrentProjectId { get; }

    ProjectRoot? CurrentProjectRoot { get; }

    void SetSession(ProjectConcordProjectId projectId, ProjectRoot root);

    void ClearSession();
}
