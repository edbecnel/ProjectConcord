using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

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
        var work = projection.CurrentWork.Single();
        Assert.Equal(ProjectionAvailability.Unavailable, work.EffectiveConfigurationAvailability);
        Assert.Equal(
            GovernedWorkStateUnavailableReasons.EffectiveConfigMissingProfile,
            work.EffectiveConfigurationUnavailableReason);
        Assert.False(work.StopActive);
        Assert.Empty(work.ApplicableActiveAuthorizationKinds);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.WaitingOnAvailability);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.NextActionAvailability);
    }

    [Fact]
    public void DependencyBlocked_ExposesWaitingOn_ForBlockedInstance()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf-proj-").FullName),
            "blocked",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var a = services.WorkflowInstances.CreateGewInstance(project.ProjectId, GovernedCorrelationId.New(), null);
        var b = services.WorkflowInstances.CreateGewInstance(project.ProjectId, GovernedCorrelationId.New(), null);
        services.WorkflowRelationships.RecordGovernedBlockingDependency(
            a.InstanceId,
            b.InstanceId,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);

        Assert.Equal(ProjectionAvailability.Available, projection.WaitingOnAvailability);
        var workA = projection.CurrentWork.Single(w => w.InstanceId == a.InstanceId);
        Assert.True(workA.DependencyBlocked);
        Assert.Contains(b.InstanceId, workA.UnresolvedRequiredWorkflowInstanceIds);
    }
}
