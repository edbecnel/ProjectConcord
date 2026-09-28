using Edf.Domain.Projects;

namespace Edf.Application.Projects;

public interface ICurrentProjectSession
{
    ProjectConcordProjectId? CurrentProjectId { get; }

    ProjectRoot? CurrentRoot { get; }
}
