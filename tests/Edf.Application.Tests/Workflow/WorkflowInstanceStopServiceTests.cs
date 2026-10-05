using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class WorkflowInstanceStopServiceTests
{
    [Fact]
    public void SetAndClear_PreservesEventHistory()
    {
        var (services, instance, persistence) = CreateInstanceWithProfile();
        var authority = () =>
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var setSummary = services.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            expectedStopResourceVersion: null,
            authority());
        Assert.True(setSummary.IsStopActive);

        var cleared = services.WorkflowInstanceStops.RecordGovernedStopClear(
            instance.InstanceId,
            setSummary.ResourceVersion,
            authority());
        Assert.False(cleared.IsStopActive);

        services.WorkStateRecovery.RecoverForProject(instance.ProjectId);
        var history = persistence.WorkflowInstanceStops.ListEventsByWorkflowInstance(instance.InstanceId);
        Assert.Equal(2, history.Count);
        Assert.Equal(WorkflowInstanceStopEventKind.Set, history[0].EventKind);
        Assert.Equal(WorkflowInstanceStopEventKind.Clear, history[1].EventKind);
    }

    [Fact]
    public void DoubleSet_FailsClosed()
    {
        var (services, instance, _) = CreateInstanceWithProfile();
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority);

        Assert.Throws<WorkflowInstanceStopOperationException>(() =>
            services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, 1, authority));
    }

    [Fact]
    public void TopologyTransition_DoesNotChange_WhenStopSet()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-stop-").FullName),
            "stop",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var instance = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        services.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var reloaded = persistence.WorkflowInstances.GetById(instance.InstanceId)!;
        Assert.Equal(instance.TopologyPlaceId, reloaded.TopologyPlaceId);
        Assert.Equal(instance.TraversalOccurrenceId, reloaded.TraversalOccurrenceId);

        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);
        Assert.True(projection.CurrentWork.Single().StopActive);
    }

    private static (WorkflowApplicationServices Services, WorkflowInstance Instance, InMemoryUserApplicationStatePersistence Persistence) CreateInstanceWithProfile()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-stop-").FullName),
            "stop",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var instance = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        return (services, instance, persistence);
    }
}
