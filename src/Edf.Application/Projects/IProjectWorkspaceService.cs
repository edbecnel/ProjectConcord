using Edf.Domain.Projects;
using Edf.Identity.Actors;

namespace Edf.Application.Projects;

public interface IProjectWorkspaceService
{
    ICurrentProjectActor CurrentActor { get; }

    ProjectRoot? CurrentRoot { get; }

    OpenProjectResult OpenProjectRoot(string absolutePath);
}
