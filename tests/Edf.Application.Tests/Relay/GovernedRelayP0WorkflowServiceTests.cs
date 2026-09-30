using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayP0WorkflowServiceTests
{
    [Fact]
    public void GeneratePaReviewExport_RequiresExplicitSessionIntents()
    {
        var (service, projectId, root) = CreateService();
        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(EngineeringAgentMode.Plan, null));

        Assert.Null(result.RenderedPackage);
        Assert.Equal(RelayValidationState.Incomplete, result.Validation.State);
    }

    [Fact]
    public void GeneratePaReviewExport_WithSessions_RendersValidPackage()
    {
        var (service, projectId, root) = CreateService();
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.New);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);

        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(EngineeringAgentMode.Agent, EngineeringAgentMode.Plan));

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.RenderedPackage);
        Assert.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, result.RenderedPackage, StringComparison.Ordinal);
    }

    private static (IGovernedRelayP0WorkflowService Service, ProjectConcordProjectId ProjectId, ProjectRoot Root) CreateService()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var service = GovernedRelayP0WorkflowService.Create(persistence);
        var resolver = new ProjectRootResolver();
        var workspace = new Edf.Application.Projects.ProjectWorkspaceService(
            resolver,
            new DegenerateAdministratorActor(),
            persistence,
            new LocalProjectRuntime());
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-t7-app-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);
        return (service, open.ProjectId!.Value, open.Root!);
    }
}
