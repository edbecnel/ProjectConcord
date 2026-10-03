using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class WorkflowRelationshipServiceTests
{
    [Fact]
    public void Origin_DoesNotImplyBlocking()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        harness.Relationships.RecordGovernedWorkflowOrigin(
            a.InstanceId,
            b.InstanceId,
            WorkflowOriginKind.SpawnedFollowOn,
            authority);

        var projection = harness.Services.WorkStateOperatorProjection.ProjectForProject(harness.ProjectId, null);
        Assert.False(projection.CurrentWork.Single(w => w.InstanceId == a.InstanceId).DependencyBlocked);
    }

    [Fact]
    public void MultipleOrigins_PerDerived_Allowed()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var c = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        harness.Relationships.RecordGovernedWorkflowOrigin(a.InstanceId, c.InstanceId, WorkflowOriginKind.DiscoveredGap, authority());
        harness.Relationships.RecordGovernedWorkflowOrigin(b.InstanceId, c.InstanceId, WorkflowOriginKind.DiscoveredGap, authority());

        var recovery = harness.Services.WorkStateRecovery.RecoverForProject(harness.ProjectId);
        Assert.Equal(2, recovery.Origins.Count(o => o.DerivedWorkflowInstanceId == c.InstanceId));
    }

    [Fact]
    public void ReleaseDependency_AndBRemainActive_BUnchanged()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var dep = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        Assert.True(harness.Services.WorkStateOperatorProjection.ProjectForProject(harness.ProjectId, null)
            .CurrentWork.Single(w => w.InstanceId == a.InstanceId).DependencyBlocked);

        var bBefore = harness.Persistence.WorkflowInstances.GetById(b.InstanceId)!;
        var released = harness.Relationships.RecordGovernedDependencyRelease(
            dep.DependencyId,
            dep.ResourceVersion,
            authority());

        Assert.Equal(WorkflowDependencyStatus.Released, released.Status);
        var bAfter = harness.Persistence.WorkflowInstances.GetById(b.InstanceId)!;
        Assert.Equal(WorkflowInstanceLifecycle.Active, bAfter.Lifecycle);
        Assert.Equal(bBefore.TopologyPlaceId, bAfter.TopologyPlaceId);
        Assert.Equal(bBefore.TraversalOccurrenceId, bAfter.TraversalOccurrenceId);
        Assert.Equal(bBefore.ResourceVersion, bAfter.ResourceVersion);

        var projection = harness.Services.WorkStateOperatorProjection.ProjectForProject(harness.ProjectId, null);
        Assert.False(projection.CurrentWork.Single(w => w.InstanceId == a.InstanceId).DependencyBlocked);
    }

    [Fact]
    public void AfterRelease_BCompleting_DoesNotConvertToSatisfied()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = AdvanceToPostSubmission(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var dep = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        var releaseCorrelation = GovernedCorrelationId.New();
        var released = harness.Relationships.RecordGovernedDependencyRelease(
            dep.DependencyId,
            dep.ResourceVersion,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(releaseCorrelation));

        harness.Workflow.RecordGovernedLifecycleCompletion(
            b.InstanceId,
            b.ResourceVersion,
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var reloaded = harness.Persistence.WorkflowDependencies.GetById(released.DependencyId)!;
        Assert.Equal(WorkflowDependencyStatus.Released, reloaded.Status);
        Assert.Equal(releaseCorrelation, reloaded.ReleaseCorrelationId);
        Assert.Null(reloaded.SatisfactionCorrelationId);
    }

    [Fact]
    public void CycleInsertion_Rejected()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var c = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        harness.Relationships.RecordGovernedBlockingDependency(b.InstanceId, c.InstanceId, authority());

        Assert.Throws<WorkflowRelationshipOperationException>(() =>
            harness.Relationships.RecordGovernedBlockingDependency(c.InstanceId, a.InstanceId, authority()));
    }

    [Fact]
    public void Satisfaction_StaleResourceVersion_FailsClosed_WithoutCorruptingPendingState()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CompleteInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var dep = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        SimulateConcurrentResourceVersionAdvance(harness, dep);

        var ex = Assert.Throws<WorkflowDependencyConcurrencyException>(() =>
            harness.Relationships.RecordGovernedDependencySatisfaction(
                dep.DependencyId,
                dep.ResourceVersion,
                authority()));

        Assert.Equal(dep.DependencyId, ex.DependencyId);
        Assert.Equal(dep.ResourceVersion, ex.ExpectedResourceVersion);

        var reloaded = harness.Persistence.WorkflowDependencies.GetById(dep.DependencyId)!;
        Assert.Equal(WorkflowDependencyStatus.Pending, reloaded.Status);
        Assert.Null(reloaded.SatisfactionCorrelationId);
        Assert.Null(reloaded.SatisfiedUtc);
    }

    [Fact]
    public void Release_StaleResourceVersion_FailsClosed_RequiredInstanceUnchanged()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var dep = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        var bBefore = harness.Persistence.WorkflowInstances.GetById(b.InstanceId)!;
        SimulateConcurrentResourceVersionAdvance(harness, dep);

        Assert.Throws<WorkflowDependencyConcurrencyException>(() =>
            harness.Relationships.RecordGovernedDependencyRelease(
                dep.DependencyId,
                dep.ResourceVersion,
                authority()));

        var reloadedDep = harness.Persistence.WorkflowDependencies.GetById(dep.DependencyId)!;
        Assert.Equal(WorkflowDependencyStatus.Pending, reloadedDep.Status);
        Assert.Null(reloadedDep.ReleaseCorrelationId);

        var bAfter = harness.Persistence.WorkflowInstances.GetById(b.InstanceId)!;
        Assert.Equal(bBefore.ResourceVersion, bAfter.ResourceVersion);
        Assert.Equal(bBefore.TopologyPlaceId, bAfter.TopologyPlaceId);
        Assert.Equal(bBefore.Lifecycle, bAfter.Lifecycle);
    }

    [Fact]
    public void MultiBlocker_PartialSatisfaction_ARemainsBlocked_OnRemainingPendingDependency()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CompleteInstance(harness);
        var c = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var depB = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, c.InstanceId, authority());

        harness.Relationships.RecordGovernedDependencySatisfaction(
            depB.DependencyId,
            depB.ResourceVersion,
            authority());

        var projection = harness.Services.WorkStateOperatorProjection.ProjectForProject(harness.ProjectId, null);
        var workA = projection.CurrentWork.Single(w => w.InstanceId == a.InstanceId);
        Assert.True(workA.DependencyBlocked);
        Assert.Contains(c.InstanceId, workA.UnresolvedRequiredWorkflowInstanceIds);
        Assert.DoesNotContain(b.InstanceId, workA.UnresolvedRequiredWorkflowInstanceIds);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.NextActionAvailability);
    }

    [Fact]
    public void MultiBlocker_PartialRelease_ARemainsBlocked_BNoLongerUnresolvedRequired()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var c = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var depB = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, c.InstanceId, authority());
        var bBefore = harness.Persistence.WorkflowInstances.GetById(b.InstanceId)!;

        harness.Relationships.RecordGovernedDependencyRelease(
            depB.DependencyId,
            depB.ResourceVersion,
            authority());

        var projection = harness.Services.WorkStateOperatorProjection.ProjectForProject(harness.ProjectId, null);
        var workA = projection.CurrentWork.Single(w => w.InstanceId == a.InstanceId);
        Assert.True(workA.DependencyBlocked);
        Assert.Contains(c.InstanceId, workA.UnresolvedRequiredWorkflowInstanceIds);
        Assert.DoesNotContain(b.InstanceId, workA.UnresolvedRequiredWorkflowInstanceIds);

        var bAfter = harness.Persistence.WorkflowInstances.GetById(b.InstanceId)!;
        Assert.Equal(bBefore.ResourceVersion, bAfter.ResourceVersion);
        Assert.Equal(bBefore.TopologyPlaceId, bAfter.TopologyPlaceId);
        Assert.Equal(bBefore.Lifecycle, bAfter.Lifecycle);
    }

    [Fact]
    public void ReblockAfterReleased_CreatesNewPendingRow()
    {
        var harness = CreateHarness();
        var a = CreateInstance(harness);
        var b = CreateInstance(harness);
        var authority = () => GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var first = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());
        harness.Relationships.RecordGovernedDependencyRelease(first.DependencyId, first.ResourceVersion, authority());
        var second = harness.Relationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority());

        Assert.NotEqual(first.DependencyId, second.DependencyId);
        Assert.Equal(WorkflowDependencyStatus.Pending, second.Status);
    }

    private static WorkflowInstance CreateInstance(TestHarness harness) =>
        harness.Workflow.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: null);

    private static WorkflowInstance AdvanceToPostSubmission(TestHarness harness)
    {
        var instance = CreateInstance(harness);
        var authority = () =>
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var planning = harness.Workflow.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            authority());

        var implementation = harness.Workflow.RecordGovernedTopologyTransition(
            planning.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.ImplementationGoverned),
            planning.ResourceVersion,
            authority());

        return harness.Workflow.RecordGovernedTopologyTransition(
            implementation.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PostSubmissionGoverned),
            implementation.ResourceVersion,
            authority());
    }

    private static WorkflowInstance CompleteInstance(TestHarness harness)
    {
        var postSubmission = AdvanceToPostSubmission(harness);
        return harness.Workflow.RecordGovernedLifecycleCompletion(
            postSubmission.InstanceId,
            postSubmission.ResourceVersion,
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
    }

    private static void SimulateConcurrentResourceVersionAdvance(TestHarness harness, WorkflowDependency dependency)
    {
        var advanced = dependency with { ResourceVersion = dependency.ResourceVersion + 1 };
        Assert.True(
            harness.Persistence.WorkflowDependencies.TryUpdateWithExpectedVersion(
                advanced,
                dependency.ResourceVersion));
    }

    private static TestHarness CreateHarness()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf-rel-").FullName),
            "rel",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        return new TestHarness(persistence, project.ProjectId, services);
    }

    private sealed record TestHarness(
        InMemoryUserApplicationStatePersistence Persistence,
        ProjectConcordProjectId ProjectId,
        WorkflowApplicationServices Services)
    {
        public IWorkflowInstanceService Workflow => Services.WorkflowInstances;

        public IWorkflowRelationshipService Relationships => Services.WorkflowRelationships;
    }
}
