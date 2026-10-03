using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects.InMemory;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Operator;

public class GovernedWorkStateOperatorProjectionTests
{
    [Fact]
    public void EmptyProject_ReturnsEmptyCurrentWork()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf-proj-").FullName),
            "empty",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);

        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);

        Assert.Empty(projection.CurrentWork);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.WaitingOnAvailability);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.NextActionAvailability);
        Assert.Equal(GovernedWorkStateUnavailableReasons.NotImplementedInM7aWf1, projection.WaitingOnUnavailableReason);
    }

    [Fact]
    public void Projection_DoesNotFabricateWaitingOnOrNextAction_WhenInstancesExist()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf-proj-").FullName),
            "work",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: null);

        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);

        Assert.Single(projection.CurrentWork);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.WaitingOnAvailability);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.NextActionAvailability);
    }
}
