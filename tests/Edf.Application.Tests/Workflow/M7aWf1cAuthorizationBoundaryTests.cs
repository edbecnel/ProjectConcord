using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class M7aWf1cAuthorizationBoundaryTests
{
    [Fact]
    public void StopSet_Succeeds_WithMissingProfile_AndNoDwa()
    {
        var (services, instance, persistence) = CreateServices();
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var summary = services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority);

        Assert.True(summary.IsStopActive);
        Assert.Empty(persistence.DevelopmentWorkAuthorizations.ListByWorkflowInstance(instance.InstanceId));
    }

    [Fact]
    public void StopSet_Succeeds_WithUnknownProfile()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1c-").FullName),
            "wf1c",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var now = DateTimeOffset.UtcNow;
        var instance = new WorkflowInstance(
            WorkflowInstanceId.New(),
            project.ProjectId,
            PrescribedWorkflowId.GovernedEngineering,
            WorkflowDefinitionVersion.GewV1,
            WorkflowProfileId.Parse("gew.unknown"),
            WorkflowInstanceLifecycle.Active,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.Intake),
            TraversalOccurrenceId.New(),
            GovernedBaselineReference.Unspecified,
            GovernedCorrelationId.New(),
            null,
            null,
            null,
            1,
            now,
            now);
        persistence.WorkflowInstances.Insert(instance);

        var summary = services.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        Assert.True(summary.IsStopActive);
    }

    [Fact]
    public void StopClear_Succeeds_WhenEffectiveConfigurationUnresolved()
    {
        var (services, instance, persistence) = CreateServices();
        var authority = () =>
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var set = services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority());
        var cleared = services.WorkflowInstanceStops.RecordGovernedStopClear(
            instance.InstanceId,
            set.ResourceVersion,
            authority());

        Assert.False(cleared.IsStopActive);
        var work = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.Equal(ProjectionAvailability.Unavailable, work.EffectiveConfigurationAvailability);
    }

    [Fact]
    public void StopSet_DoesNotMutateDwa()
    {
        var (services, instance, persistence) = CreateServices(withProfile: true);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var before = persistence.DevelopmentWorkAuthorizations.ListByWorkflowInstance(instance.InstanceId);
        services.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var after = persistence.DevelopmentWorkAuthorizations.ListByWorkflowInstance(instance.InstanceId);

        Assert.Equal(before.Count, after.Count);
        Assert.Equal(before[0].Disposition, after[0].Disposition);
        Assert.Equal(before[0].ResourceVersion, after[0].ResourceVersion);
    }

    [Fact]
    public void StopSet_StaleVersion_DoesNotAppendEvent()
    {
        var (services, instance, persistence) = CreateServices();
        var authority = () =>
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        var set = services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority());
        var cleared = services.WorkflowInstanceStops.RecordGovernedStopClear(
            instance.InstanceId,
            set.ResourceVersion,
            authority());

        Assert.Throws<WorkflowInstanceStopConcurrencyException>(() =>
            services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, 0, authority()));

        Assert.Equal(2, persistence.WorkflowInstanceStops.ListEventsByWorkflowInstance(instance.InstanceId).Count);
        Assert.False(cleared.IsStopActive);
    }

    [Fact]
    public void DwaGrant_WithNoPriorDwa_RequiresResolvableProfile_NotCircular()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.Equal(DevelopmentWorkAuthorizationDisposition.Active, granted.Disposition);
    }

    [Fact]
    public void NextAction_RemainsUnavailable_AfterWf1cCorrections()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.NextActionAvailability);
    }

    private static (WorkflowApplicationServices Services, WorkflowInstance Instance, InMemoryUserApplicationStatePersistence Persistence) CreateServices(
        bool withProfile = false)
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1c-").FullName),
            "wf1c",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var instance = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            withProfile ? WorkflowProfileId.Parse(GewV1ProfileIds.Standard) : null);
        return (services, instance, persistence);
    }
}
