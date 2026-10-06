using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Operator.PlanningEntry;

public class PlanningEntryRelayReadModelTests
{
    [Fact]
    public void Resolve_RecoversLatestConsumedPaHandover_AfterSimulatedRelaunch()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var relay = GovernedRelayP0WorkflowService.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(projectId);
        var validation = RelayValidationResult.Valid([]);
        var rendered = new GovernedRelayV1Renderer().Render(package);
        relay.ImportPaHandover(projectId, rendered);

        var readModel = new PlanningEntryRelayReadModel(persistence.RelayOperational);
        var snapshot = readModel.Resolve(projectId);

        Assert.NotNull(snapshot.LatestConsumedPaHandover);
        Assert.Equal(RelayValidationState.Valid, snapshot.LatestConsumedPaHandover!.Validation.State);
    }
}
