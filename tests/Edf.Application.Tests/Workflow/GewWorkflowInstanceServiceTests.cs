using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Tests.Workflow;

public class GewWorkflowInstanceServiceTests
{
    [Fact]
    public void CreateGewInstance_StartsAtIntake_WithCreationProvenance()
    {
        var harness = CreateHarness();
        var creationCorrelation = GovernedCorrelationId.New();
        var instance = harness.Workflow.CreateGewInstance(
            harness.ProjectId,
            creationCorrelation,
            projectRootAbsolutePath: null);

        Assert.Equal(WorkflowInstanceLifecycle.Active, instance.Lifecycle);
        Assert.Equal(GewV1TopologyPlaces.Intake, instance.TopologyPlaceId.Value);
        Assert.Equal(creationCorrelation, instance.CreationCorrelationId);
        Assert.Null(instance.ProfileId);
        Assert.Equal(1, instance.ResourceVersion);
        Assert.False(instance.TraversalOccurrenceId.IsEmpty);
    }

    [Fact]
    public void RecordGovernedTopologyTransition_RequiresAuthority()
    {
        var harness = CreateHarness();
        var instance = CreateInstance(harness);

        Assert.Throws<ArgumentNullException>(() =>
            harness.Workflow.RecordGovernedTopologyTransition(
                instance.InstanceId,
                TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
                instance.ResourceVersion,
                authority: null!));

        Assert.Throws<ArgumentException>(() =>
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                new GovernedCorrelationId(Guid.Empty)));
    }

    [Fact]
    public void AllowedTransition_WithAuthority_UpdatesPlaceAndOccurrence()
    {
        var harness = CreateHarness();
        var instance = CreateInstance(harness);
        var authority = GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var updated = harness.Workflow.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            authority);

        Assert.Equal(GewV1TopologyPlaces.PlanningGoverned, updated.TopologyPlaceId.Value);
        Assert.NotEqual(instance.TraversalOccurrenceId, updated.TraversalOccurrenceId);
        Assert.Equal(2, updated.ResourceVersion);
        Assert.Equal(authority.CorrelationId, updated.LastGovernedTransitionCorrelationId);
    }

    [Fact]
    public void InvalidEdge_Rejects_EvenWithAuthority()
    {
        var harness = CreateHarness();
        var instance = CreateInstance(harness);
        var authority = GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var ex = Assert.Throws<WorkflowInstanceOperationException>(() =>
            harness.Workflow.RecordGovernedTopologyTransition(
                instance.InstanceId,
                TopologyPlaceId.Parse(GewV1TopologyPlaces.ImplementationGoverned),
                instance.ResourceVersion,
                authority));

        Assert.Contains("not permitted", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StaleResourceVersion_Rejects()
    {
        var harness = CreateHarness();
        var instance = CreateInstance(harness);
        var authority = GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        Assert.Throws<WorkflowInstanceConcurrencyException>(() =>
            harness.Workflow.RecordGovernedTopologyTransition(
                instance.InstanceId,
                TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
                expectedResourceVersion: instance.ResourceVersion + 1,
                authority));
    }

    [Fact]
    public void ReEntryToSamePlace_ProducesDistinctOccurrence()
    {
        var harness = CreateHarness();
        var atPostSubmission = AdvanceToPostSubmission(harness);
        var authority = () =>
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var firstImplementation = harness.Workflow.RecordGovernedTopologyTransition(
            atPostSubmission.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.ImplementationGoverned),
            atPostSubmission.ResourceVersion,
            authority());

        var backToPostSubmission = harness.Workflow.RecordGovernedTopologyTransition(
            firstImplementation.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PostSubmissionGoverned),
            firstImplementation.ResourceVersion,
            authority());

        var secondImplementation = harness.Workflow.RecordGovernedTopologyTransition(
            backToPostSubmission.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.ImplementationGoverned),
            backToPostSubmission.ResourceVersion,
            authority());

        Assert.Equal(GewV1TopologyPlaces.ImplementationGoverned, secondImplementation.TopologyPlaceId.Value);
        Assert.NotEqual(firstImplementation.TraversalOccurrenceId, secondImplementation.TraversalOccurrenceId);
    }

    [Fact]
    public void RecordGovernedLifecycleCompletion_RequiresAuthorityAndPostSubmissionPlace()
    {
        var harness = CreateHarness();
        var instance = CreateInstance(harness);
        var authority = GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        Assert.Throws<WorkflowInstanceOperationException>(() =>
            harness.Workflow.RecordGovernedLifecycleCompletion(
                instance.InstanceId,
                instance.ResourceVersion,
                authority));

        var atPostSubmission = AdvanceToPostSubmission(harness);
        var completed = harness.Workflow.RecordGovernedLifecycleCompletion(
            atPostSubmission.InstanceId,
            atPostSubmission.ResourceVersion,
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.Equal(WorkflowInstanceLifecycle.Completed, completed.Lifecycle);
        Assert.Equal(atPostSubmission.TopologyPlaceId, completed.TopologyPlaceId);
    }

    [Fact]
    public void MultipleActiveInstances_Coexist_WithoutPrimarySemantics()
    {
        var harness = CreateHarness();
        var first = CreateInstance(harness);
        var second = CreateInstance(harness);

        var recovered = harness.Recovery.RecoverForProject(harness.ProjectId);
        Assert.Equal(2, recovered.ActiveInstances.Count);
        Assert.Contains(recovered.ActiveInstances, i => i.InstanceId == first.InstanceId);
        Assert.Contains(recovered.ActiveInstances, i => i.InstanceId == second.InstanceId);
    }

    [Fact]
    public void GovernedBaseline_RemainsUnchanged_WhenHeadAdvances()
    {
        var git = new MutableGitHeadCommitResolver("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        var harness = CreateHarness(git);
        var instance = harness.Workflow.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: "/tmp/project",
            profileId: WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        git.Head = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";

        var reloaded = harness.Persistence.WorkflowInstances.GetById(instance.InstanceId);
        Assert.NotNull(reloaded);
        Assert.Equal("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", reloaded!.GovernedBaseline.Value);
    }

    [Fact]
    public void UnknownProfile_OnCreate_FailsClosed()
    {
        var harness = CreateHarness();
        Assert.Throws<WorkflowInstanceOperationException>(() =>
            harness.Workflow.CreateGewInstance(
                harness.ProjectId,
                GovernedCorrelationId.New(),
                projectRootAbsolutePath: null,
                profileId: WorkflowProfileId.Parse("gew.unknown")));
    }

    [Fact]
    public void GewV1Registry_UnknownTransition_FailsClosed()
    {
        var registry = new GewV1PrescribedWorkflowRegistry();
        Assert.True(registry.TryGetDefinition(
            PrescribedWorkflowId.GovernedEngineering,
            WorkflowDefinitionVersion.GewV1,
            out var definition));
        Assert.False(registry.IsPermittedTransition(
            definition!,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.Intake),
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PostSubmissionGoverned)));
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

    [Fact]
    public void TopologyTransition_WithoutProfile_DoesNotRequireEffectiveConfiguration()
    {
        var harness = CreateHarness();
        var instance = harness.Workflow.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: null);

        var updated = harness.Workflow.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.Equal(GewV1TopologyPlaces.PlanningGoverned, updated.TopologyPlaceId.Value);
    }

    private static TestHarness CreateHarness(IGitHeadCommitResolver? git = null)
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf-app-").FullName),
            "wf",
            DateTimeOffset.UtcNow);
        var registry = new GewV1PrescribedWorkflowRegistry();
        var gitResolver = git ?? new GitHeadCommitResolver();
        var workflow = new WorkflowInstanceService(persistence.WorkflowInstances, registry, gitResolver);
        var recovery = new GovernedWorkStateRecoveryService(
            persistence.WorkflowInstances,
            persistence.WorkflowOrigins,
            persistence.WorkflowDependencies,
            persistence.DevelopmentWorkAuthorizations,
            persistence.WorkflowInstanceStops,
            registry);
        return new TestHarness(persistence, project.ProjectId, workflow, recovery);
    }

    private sealed record TestHarness(
        InMemoryUserApplicationStatePersistence Persistence,
        ProjectConcordProjectId ProjectId,
        IWorkflowInstanceService Workflow,
        IGovernedWorkStateRecoveryService Recovery);

    private sealed class MutableGitHeadCommitResolver : IGitHeadCommitResolver
    {
        public MutableGitHeadCommitResolver(string? head) => Head = head;

        public string? Head { get; set; }

        public string? TryResolveHeadCommit(string projectRootAbsolutePath) => Head;
    }
}
