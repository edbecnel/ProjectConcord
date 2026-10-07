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
        var (service, projectId, root, _) = CreateService();
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
        var (service, projectId, root, _) = CreateService();
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.New);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);

        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(EngineeringAgentMode.Agent, EngineeringAgentMode.Plan));

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.RenderedPackage);
        Assert.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, result.RenderedPackage, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayPaHandoverOutputContract.SectionHeading, result.RenderedPackage, StringComparison.Ordinal);
        Assert.Contains("PLANNING ENTRY", result.RenderedPackage, StringComparison.Ordinal);
    }

    [Fact]
    public void GeneratePaReviewExport_PlanningDevelopmentWorkAuthorization_WithSessions_RendersValidPackage()
    {
        var (service, projectId, root, _) = CreateService();
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.New);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);

        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(
                EngineeringAgentMode.Plan,
                null,
                PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization));

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.RenderedPackage);
        Assert.Contains(
            "PLANNING DEVELOPMENT WORK AUTHORIZATION",
            result.RenderedPackage,
            StringComparison.Ordinal);
        Assert.NotNull(result.PackageId);
    }

    [Fact]
    public void GeneratePaReviewExport_PlanningDevelopmentWorkAuthorization_RecordsProfileProvenance_AndValidGovernance()
    {
        var (service, projectId, root, persistence) = CreateService();
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.New);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);

        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(
                EngineeringAgentMode.Plan,
                null,
                PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization));

        Assert.NotNull(result.PackageId);
        var persisted = persistence.RelayOperational.GetPackage(result.PackageId.Value);
        Assert.NotNull(persisted);
        Assert.False(persisted!.Package.GovernanceCritical.AuthorizationDispositionPresent);

        var produced = persistence.RelayOperational
            .ListProvenanceEvents(projectId)
            .Last(e => e.EventType == RelayProvenanceEventType.PackageProduced);
        Assert.Equal(
            PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization,
            RelayPaReviewExportProvenance.TryReadProfile(produced));
    }

    private static (
        IGovernedRelayP0WorkflowService Service,
        ProjectConcordProjectId ProjectId,
        ProjectRoot Root,
        InMemoryUserApplicationStatePersistence Persistence) CreateService()
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
        return (service, open.ProjectId!.Value, open.Root!, persistence);
    }
}
