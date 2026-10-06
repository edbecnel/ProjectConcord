using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class M7aWf1dEligibilityTests
{
    [Fact]
    public void FullyGovernedActionability_IsNeverFullyActionable_InWf1d()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var work = projection.CurrentWork.Single();
        Assert.Equal(GovernedActionabilityCompleteness.Indeterminate, work.GovernedEligibility.FullyGovernedActionability);
        Assert.NotEqual(GovernedActionabilityCompleteness.FullyActionable, work.GovernedEligibility.FullyGovernedActionability);
        Assert.Equal(
            WorkflowEligibilityReasonCodes.FullyGovernedIndeterminateWf1d,
            work.GovernedEligibility.FullyGovernedActionabilityReasonCode);
    }

    [Fact]
    public void Intake_DoesNotRequirePlanningOrImplementationDwa_ForEvaluatedSatisfaction()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var work = projection.CurrentWork.Single();
        Assert.Equal(
            WorkflowInstanceEvaluatedRoutingState.IntakeAwaitingGovernedPlanningEntry,
            work.GovernedEligibility.RoutingState);
        Assert.DoesNotContain(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.True(work.GovernedEligibility.EligibleUnderEvaluatedConstraints);
        Assert.Contains(instance.InstanceId, projection.CandidateFrontierInstanceIds);
    }

    [Fact]
    public void PlanningGoverned_RequiresPlanningDwa()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var work = projection.CurrentWork.Single();
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.False(work.GovernedEligibility.EligibleUnderEvaluatedConstraints);
    }

    [Fact]
    public void PlanningGoverned_WithPlanningDwa_SatisfiesEvaluatedAuthorization()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var work = projection.CurrentWork.Single();
        Assert.DoesNotContain(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.True(work.GovernedEligibility.EligibleUnderEvaluatedConstraints);
    }

    [Fact]
    public void ImplementationGoverned_WrongDwaKind_BlocksAuthorization()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.ImplementationGoverned);
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            projection.CurrentWork.Single().GovernedEligibility.EvaluatedConstraintViolationCodes);
    }

    [Fact]
    public void ImplementationGoverned_WithImplementationDwa_SatisfiesEvaluatedAuthorization()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.ImplementationGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var work = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.DoesNotContain(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.True(work.GovernedEligibility.EligibleUnderEvaluatedConstraints);
    }

    [Fact]
    public void PostSubmission_DoesNotRequirePlanningOrImplementationDwa()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.ImplementationGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PostSubmissionGoverned);
        var work = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.Equal(
            WorkflowInstanceEvaluatedRoutingState.PostSubmissionAwaitingGovernance,
            work.GovernedEligibility.RoutingState);
        Assert.DoesNotContain(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
    }

    [Fact]
    public void ReEntry_DoesNotReusePriorOccurrenceBoundDwa()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.ImplementationGoverned);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PostSubmissionGoverned);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        var work = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        var records = services.WorkStateRecovery.RecoverForProject(instance.ProjectId).DevelopmentWorkAuthorizations;
        Assert.Contains(records, r => r.Disposition == DevelopmentWorkAuthorizationDisposition.Active);
    }

    [Fact]
    public void Stop_BlocksEvaluatedEligibility_AndClearRestoresFrontier()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        var stopSummary = services.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            authority);
        var blockedProjection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var blockedWork = blockedProjection.CurrentWork.Single();
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedInstanceStop,
            blockedWork.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.False(blockedWork.GovernedEligibility.EligibleUnderEvaluatedConstraints);
        Assert.DoesNotContain(instance.InstanceId, blockedProjection.CandidateFrontierInstanceIds);
        Assert.Equal(ProjectionAvailability.Available, blockedProjection.WaitingOnAvailability);
        var stopWaiting = blockedProjection.WaitingOnItems.Single(i => i.WorkflowInstanceId == instance.InstanceId);
        Assert.Equal(WorkflowEligibilityReasonCodes.WaitingOnInstanceStop, stopWaiting.ReasonCode);

        services.WorkflowInstanceStops.RecordGovernedStopClear(
            instance.InstanceId,
            stopSummary.ResourceVersion,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var restoredProjection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var restoredWork = restoredProjection.CurrentWork.Single();
        Assert.True(restoredWork.GovernedEligibility.EligibleUnderEvaluatedConstraints);
        Assert.Contains(instance.InstanceId, restoredProjection.CandidateFrontierInstanceIds);
    }

    [Fact]
    public void PlanningGoverned_SupersedeWithoutRegrant_BlocksUntilNewGrant()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        var grantAuthority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            grantAuthority);
        services.DevelopmentWorkAuthorizations.RecordGovernedSupersede(
            granted.AuthorizationId,
            granted.ResourceVersion,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var blockedWork = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            blockedWork.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.False(blockedWork.GovernedEligibility.EligibleUnderEvaluatedConstraints);

        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var restoredWork = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.DoesNotContain(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            restoredWork.GovernedEligibility.EvaluatedConstraintViolationCodes);
        Assert.True(restoredWork.GovernedEligibility.EligibleUnderEvaluatedConstraints);
    }

    [Fact]
    public void TwoEligibleInstances_CandidateFrontier_OrderedByInstanceIdThenPlace()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1d-frontier-").FullName),
            "frontier-order",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var first = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        var second = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);
        var expectedOrder = new[] { first.InstanceId, second.InstanceId }
            .OrderBy(id => id.Value)
            .ToList();
        Assert.Equal(2, projection.CandidateFrontierInstanceIds.Count);
        Assert.Equal(expectedOrder, projection.CandidateFrontierInstanceIds.ToList());
    }

    [Fact]
    public void CandidateFrontier_MixedCandidateAndBlocked()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1d-").FullName),
            "mix",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var blocked = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        var candidate = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        services.WorkflowRelationships.RecordGovernedBlockingDependency(
            blocked.InstanceId,
            candidate.InstanceId,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);
        Assert.Single(projection.CandidateFrontierInstanceIds);
        Assert.Equal(candidate.InstanceId, projection.CandidateFrontierInstanceIds[0]);
        Assert.DoesNotContain(blocked.InstanceId, projection.CandidateFrontierInstanceIds);
    }

    [Fact]
    public void UnevaluatedDimensions_Empty_WhenRegistryHasNoAttachments()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        var work = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.Empty(work.GovernedEligibility.UnevaluatedApplicableDimensions);
        Assert.Empty(work.GovernedEligibility.DimensionApplicabilityUnknown);
    }

    [Fact]
    public void NextAction_Suggestion_DoesNotGrantPermission()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        Assert.Equal(ProjectionAvailability.Available, projection.NextActionAvailability);
        Assert.NotEmpty(projection.WorkflowNextActions);
        foreach (var action in projection.WorkflowNextActions)
        {
            Assert.True(
                action.PermissionToExecute is GovernedExecutionPermission.Indeterminate
                    or GovernedExecutionPermission.Denied
                    or GovernedExecutionPermission.NotApplicable);
        }
    }

    [Fact]
    public void PostSubmission_RoutingSuggestions_NotApplicable_WhenEvaluatedConstraintsSatisfied()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = MoveToPostSubmissionWithSatisfiedEvaluatedConstraints(services, instance);
        var actions = PostSubmissionRoutingActions(services, instance);
        Assert.Equal(3, actions.Count);
        Assert.All(actions, a => Assert.Equal(GovernedExecutionPermission.NotApplicable, a.PermissionToExecute));
    }

    [Fact]
    public void PostSubmission_RoutingSuggestions_Denied_WhenInstanceStopActive()
    {
        var (services, instance, _) = CreateServices(withProfile: true);
        instance = MoveToPostSubmissionWithSatisfiedEvaluatedConstraints(services, instance);
        services.WorkflowInstanceStops.RecordGovernedStopSet(
            instance.InstanceId,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var actions = PostSubmissionRoutingActions(services, instance);
        Assert.Equal(3, actions.Count);
        Assert.All(actions, a => Assert.Equal(GovernedExecutionPermission.Denied, a.PermissionToExecute));
    }

    [Fact]
    public void PostSubmission_RoutingSuggestions_Denied_WhenPendingDependency()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1d-post-").FullName),
            "post-dep",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var required = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        var blocked = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        blocked = MoveToPostSubmissionWithSatisfiedEvaluatedConstraints(services, blocked);
        services.WorkflowRelationships.RecordGovernedBlockingDependency(
            blocked.InstanceId,
            required.InstanceId,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var actions = PostSubmissionRoutingActions(services, blocked);
        Assert.Equal(3, actions.Count);
        Assert.All(actions, a => Assert.Equal(GovernedExecutionPermission.Denied, a.PermissionToExecute));
    }

    [Fact]
    public void PostSubmission_RoutingSuggestions_Denied_WhenEffectiveConfigurationUnresolved()
    {
        var (services, instance, _) = CreateServices(withProfile: false);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.ImplementationGoverned);
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PostSubmissionGoverned);
        var work = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null).CurrentWork.Single();
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedEffectiveConfiguration,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        var actions = PostSubmissionRoutingActions(services, instance);
        Assert.Equal(3, actions.Count);
        Assert.All(actions, a => Assert.Equal(GovernedExecutionPermission.Denied, a.PermissionToExecute));
    }

    [Fact]
    public void WaitingOn_ContainsOnlyEvaluatedBlockers_NotSynchronization()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1d-").FullName),
            "wait",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var a = services.WorkflowInstances.CreateGewInstance(project.ProjectId, GovernedCorrelationId.New(), null);
        var b = services.WorkflowInstances.CreateGewInstance(project.ProjectId, GovernedCorrelationId.New(), null);
        services.WorkflowRelationships.RecordGovernedBlockingDependency(
            a.InstanceId,
            b.InstanceId,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);
        Assert.All(
            projection.WaitingOnItems,
            i => Assert.NotEqual(WorkflowEligibilityReasonCodes.WaitingOnSynchronizationNotEvaluated, i.ReasonCode));
    }

    private static IReadOnlyList<WorkflowOperatorNextActionItem> PostSubmissionRoutingActions(
        WorkflowApplicationServices services,
        WorkflowInstance instance)
    {
        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        return projection.WorkflowNextActions
            .Where(a => a.WorkflowInstanceId == instance.InstanceId)
            .Where(a => a.SuggestedStepCode is WorkflowEligibilityReasonCodes.NextActionPostSubmissionReplanningRoute
                or WorkflowEligibilityReasonCodes.NextActionPostSubmissionReworkRoute
                or WorkflowEligibilityReasonCodes.NextActionPostSubmissionLifecycleCompletionRoute)
            .ToList();
    }

    private static WorkflowInstance MoveToPostSubmissionWithSatisfiedEvaluatedConstraints(
        WorkflowApplicationServices services,
        WorkflowInstance instance)
    {
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.PlanningGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        instance = TransitionTo(services, instance, GewV1TopologyPlaces.ImplementationGoverned);
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        return TransitionTo(services, instance, GewV1TopologyPlaces.PostSubmissionGoverned);
    }

    private static WorkflowInstance TransitionTo(
        WorkflowApplicationServices services,
        WorkflowInstance instance,
        string targetPlace)
    {
        return services.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(targetPlace),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
    }

    private static (WorkflowApplicationServices Services, WorkflowInstance Instance, InMemoryUserApplicationStatePersistence Persistence) CreateServices(
        bool withProfile = false)
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-wf1d-").FullName),
            "wf1d",
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
