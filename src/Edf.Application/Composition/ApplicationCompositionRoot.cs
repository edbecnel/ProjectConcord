using Edf.Application.Projects;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Composition;

public static class ApplicationCompositionRoot
{
    public static IProjectWorkspaceService CreateDefaultWorkspaceService()
    {
        var actor = new DegenerateAdministratorActor();
        var resolver = new ProjectRootResolver();
        var runtime = new LocalProjectRuntime();
        return new ProjectWorkspaceService(resolver, actor, runtime);
    }
}
