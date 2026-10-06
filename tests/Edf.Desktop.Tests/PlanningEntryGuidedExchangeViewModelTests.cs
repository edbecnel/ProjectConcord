using Edf.Application.Composition;
using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Desktop.Tests;

public class PlanningEntryGuidedExchangeViewModelTests
{
    [Fact]
    public void Activate_WithoutSessionIntents_ShowsConfirmSessionContinuity()
    {
        var harness = CreateHarness();
        harness.Workflow.WorkflowInstances.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            harness.Root.AbsolutePath,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        harness.Guided.ActivatePlanningEntryGuided();

        Assert.Equal(PlanningEntryGuidedStep.ConfirmSessionContinuity, harness.Guided.CurrentStep);
        Assert.True(harness.Guided.ShowConfirmSessionContinuity);
    }

    [Fact]
    public void ReviewDecision_DoesNotAutoEnterGovernedPlanning()
    {
        var harness = CreateHarness();
        harness.Relay.SetProjectArchitectSessionIntent(harness.ProjectId, AgentSessionIntent.Continue);
        harness.Relay.SetEngineeringAgentSessionIntent(harness.ProjectId, AgentSessionIntent.Continue);
        var instance = harness.Workflow.WorkflowInstances.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            harness.Root.AbsolutePath,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        harness.Relay.ImportPaHandover(harness.ProjectId, new GovernedRelayV1Renderer().Render(package));
        harness.Guided.ActivatePlanningEntryGuided();

        Assert.Equal(PlanningEntryGuidedStep.ReviewDecision, harness.Guided.CurrentStep);
        Assert.Equal(GewV1TopologyPlaces.Intake, instance.TopologyPlaceId.Value);
    }

    [Fact]
    public async Task DurableHandover_SurvivesNewGuidedViewModelInstance()
    {
        var harness = CreateHarness();
        harness.Relay.SetProjectArchitectSessionIntent(harness.ProjectId, AgentSessionIntent.Continue);
        harness.Relay.SetEngineeringAgentSessionIntent(harness.ProjectId, AgentSessionIntent.Continue);
        harness.Workflow.WorkflowInstances.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            harness.Root.AbsolutePath,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        harness.Relay.ImportPaHandover(harness.ProjectId, new GovernedRelayV1Renderer().Render(package));

        var relaunched = new PlanningEntryGuidedExchangeViewModel(
            harness.Relay,
            harness.Workspace,
            harness.Workflow,
            _ => Task.CompletedTask,
            () => { },
            null);
        relaunched.OnActiveProjectChanged(harness.ProjectId, hasActiveProject: true);
        relaunched.ActivatePlanningEntryGuided();

        Assert.Equal(PlanningEntryGuidedStep.ReviewDecision, relaunched.CurrentStep);
    }

    private static Harness CreateHarness()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workspace = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            new DegenerateAdministratorActor(),
            persistence,
            new LocalProjectRuntime());
        var relay = GovernedRelayP0WorkflowService.Create(persistence);
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-guided-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);
        var projectId = open.ProjectId!.Value;
        var guided = new PlanningEntryGuidedExchangeViewModel(
            relay,
            workspace,
            workflow,
            _ => Task.CompletedTask,
            () => { },
            null);
        guided.OnActiveProjectChanged(projectId, hasActiveProject: true);
        return new Harness(persistence, workspace, relay, workflow, guided, projectId, open.Root!);
    }

    private sealed record Harness(
        InMemoryUserApplicationStatePersistence Persistence,
        IProjectWorkspaceService Workspace,
        IGovernedRelayP0WorkflowService Relay,
        WorkflowApplicationServices Workflow,
        PlanningEntryGuidedExchangeViewModel Guided,
        ProjectConcordProjectId ProjectId,
        ProjectRoot Root);
}
