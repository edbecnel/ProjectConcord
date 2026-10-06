using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Engine.Projects;

namespace Edf.Application.Tests.Workflow.PlanningEntry;

public class GewV1IntakePlanningEntryTransitionServiceTests
{
    [Fact]
    public void QualifyingPlanningEntryHandover_IsEligible()
    {
        var harness = CreateHarness();
        var instance = CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);
        Assert.Equal(RelayValidationState.Valid, validation.State);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.True(eligibility.IsEligible);
        Assert.Equal(instance.InstanceId, eligibility.ApplicableInstanceId);
    }

    [Fact]
    public void ImplementationAuthorizedHandover_RejectedForPlanningEntry()
    {
        var harness = CreateHarness();
        CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreateImplementationAuthorizedHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.ContractNotSatisfied, eligibility.ReasonCode);
    }

    [Fact]
    public void WrongProject_Rejected()
    {
        var harness = CreateHarness();
        CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(ProjectConcordProjectId.New());
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.PackageWrongProject, eligibility.ReasonCode);
    }

    [Fact]
    public void WrongWorkflowPlace_Rejected()
    {
        var harness = CreateHarness();
        var instance = CreateIntakeInstance(harness);
        var authority = GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        harness.Workflow.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            authority);

        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.WrongWorkflowPlace, eligibility.ReasonCode);
    }

    [Fact]
    public void RelayStopActive_Rejected()
    {
        var harness = CreateHarness();
        CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(
            harness.ProjectId,
            g => g with { Stop = new RelayStopMetadata(RelayStopState.Active, "STOP") });
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.RelayStopActive, eligibility.ReasonCode);
    }

    [Fact]
    public void InstanceStop_BlocksTransition()
    {
        var harness = CreateHarness();
        var instance = CreateIntakeInstance(harness);
        var stopAuthority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        harness.WorkflowServices.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            stopAuthority);

        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.EvaluatedBlocker, eligibility.ReasonCode);
    }

    [Fact]
    public void ZeroApplicableIntakeInstances_Rejected()
    {
        var harness = CreateHarness();
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.NoApplicableIntakeInstance, eligibility.ReasonCode);
    }

    [Fact]
    public void MultipleApplicableIntakeInstances_Rejected()
    {
        var harness = CreateHarness();
        CreateIntakeInstance(harness);
        CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);

        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.MultipleApplicableIntakeInstances, eligibility.ReasonCode);
    }

    [Fact]
    public void SuccessfulTransition_UsesFromRelayProvenance_AndNoDwaCreated()
    {
        var harness = CreateHarness();
        CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);
        var dwaBefore = harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId).Count;

        var result = harness.Service.TryEnterGovernedPlanning(harness.ProjectId, package, validation);

        Assert.Equal(IntakePlanningEntryTransitionOutcome.Succeeded, result.Outcome);
        Assert.NotNull(result.Instance);
        Assert.Equal(GewV1TopologyPlaces.PlanningGoverned, result.Instance!.TopologyPlaceId.Value);
        Assert.Equal(package.CorrelationId, result.Instance.LastGovernedTransitionCorrelationId);
        Assert.Equal(package.PackageId, result.Instance.LastGovernedTransitionPackageId);

        var dwaAfter = harness.Persistence.DevelopmentWorkAuthorizations.ListByProject(harness.ProjectId).Count;
        Assert.Equal(dwaBefore, dwaAfter);
    }

    [Fact]
    public void Replay_IsIdempotent()
    {
        var harness = CreateHarness();
        CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        var first = harness.Service.TryEnterGovernedPlanning(harness.ProjectId, package, validation);
        var second = harness.Service.TryEnterGovernedPlanning(harness.ProjectId, package, validation);

        Assert.Equal(IntakePlanningEntryTransitionOutcome.Succeeded, first.Outcome);
        Assert.Equal(IntakePlanningEntryTransitionOutcome.AlreadyApplied, second.Outcome);
        Assert.Equal(first.Instance!.ResourceVersion, second.Instance!.ResourceVersion);
    }

    [Fact]
    public void AfterTransitionToPlanning_EligibilityRequiresWrongPlace_NotIntake()
    {
        var harness = CreateHarness();
        var instance = CreateIntakeInstance(harness);
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(harness.ProjectId);
        var validation = Mvr0005PlanningEntryRelayFixtures.Validate(package);

        harness.Workflow.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));

        var eligibility = harness.Service.EvaluateEligibility(harness.ProjectId, package, validation);
        Assert.False(eligibility.IsEligible);
        Assert.Equal(PlanningEntryTransitionReasonCodes.WrongWorkflowPlace, eligibility.ReasonCode);
    }

    private static WorkflowInstance CreateIntakeInstance(TestHarness harness) =>
        harness.Workflow.CreateGewInstance(
            harness.ProjectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

    private static TestHarness CreateHarness()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-planning-entry-").FullName),
            "planning-entry",
            DateTimeOffset.UtcNow);
        var workflowServices = WorkflowApplicationServicesFactory.Create(persistence);
        return new TestHarness(
            persistence,
            project.ProjectId,
            workflowServices.WorkflowInstances,
            workflowServices,
            workflowServices.IntakePlanningEntryTransitions);
    }

    private sealed record TestHarness(
        InMemoryUserApplicationStatePersistence Persistence,
        ProjectConcordProjectId ProjectId,
        IWorkflowInstanceService Workflow,
        WorkflowApplicationServices WorkflowServices,
        IGewV1IntakePlanningEntryTransitionService Service);
}
