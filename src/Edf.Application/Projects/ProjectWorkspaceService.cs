using Edf.Domain.Projects;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Projects;

public sealed class ProjectWorkspaceService : IProjectWorkspaceService
{
    private readonly ProjectRootResolver _rootResolver;
    private readonly ILocalProjectRuntime _localRuntime;

    public ProjectWorkspaceService(
        ProjectRootResolver rootResolver,
        ICurrentProjectActor currentActor,
        ILocalProjectRuntime localRuntime)
    {
        _rootResolver = rootResolver ?? throw new ArgumentNullException(nameof(rootResolver));
        CurrentActor = currentActor ?? throw new ArgumentNullException(nameof(currentActor));
        _localRuntime = localRuntime ?? throw new ArgumentNullException(nameof(localRuntime));
    }

    public ICurrentProjectActor CurrentActor { get; }

    public ProjectRoot? CurrentRoot { get; private set; }

    public OpenProjectResult OpenProjectRoot(string absolutePath)
    {
        try
        {
            var root = _rootResolver.Resolve(absolutePath);
            CurrentRoot = root;
            return OpenProjectResult.Succeeded(root);
        }
        catch (Exception ex)
        {
            return OpenProjectResult.Failed(ex.Message);
        }
    }
}
