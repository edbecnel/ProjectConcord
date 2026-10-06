using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
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
        Assert.Empty(projection.CandidateFrontierInstanceIds);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.WaitingOnAvailability);
        Assert.Equal(ProjectionAvailability.Unavailable, projection.NextActionAvailability);
        Assert.Equal(GovernedWorkStateUnavailableReasons.NoEvaluatedWaitingOnConditions, projection.WaitingOnUnavailableReason);
        Assert.Equal(
            GovernedWorkStateUnavailableReasons.NoWorkflowNextActionSuggestions,
            projection.NextActionUnavailableReason);
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
        Assert.Equal(ProjectionAvailability.Available, projection.WaitingOnAvailability);
        Assert.Equal(ProjectionAvailability.Available, projection.NextActionAvailability);
        Assert.Contains(
            WorkflowEligibilityReasonCodes.BlockedEffectiveConfiguration,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
        var effectiveConfigWaiting = projection.WaitingOnItems.Single(i => i.WorkflowInstanceId == work.InstanceId);
        Assert.Equal(WorkflowEligibilityReasonCodes.WaitingOnEffectiveConfiguration, effectiveConfigWaiting.ReasonCode);
        Assert.Empty(effectiveConfigWaiting.RelatedWorkflowInstanceIds);
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
        var profileId = WorkflowProfileId.Parse(GewV1ProfileIds.Standard);
        var a = services.WorkflowInstances.CreateGewInstance(project.ProjectId, GovernedCorrelationId.New(), null, profileId);
        var b = services.WorkflowInstances.CreateGewInstance(project.ProjectId, GovernedCorrelationId.New(), null, profileId);
        services.WorkflowRelationships.RecordGovernedBlockingDependency(
            a.InstanceId,
            b.InstanceId,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var projection = services.WorkStateOperatorProjection.ProjectForProject(project.ProjectId, null);

        Assert.Equal(ProjectionAvailability.Available, projection.WaitingOnAvailability);
        var workA = projection.CurrentWork.Single(w => w.InstanceId == a.InstanceId);
        Assert.True(workA.DependencyBlocked);
        Assert.Contains(b.InstanceId, workA.UnresolvedRequiredWorkflowInstanceIds);
        var dependencyWaiting = projection.WaitingOnItems.Single(i =>
            i.WorkflowInstanceId == a.InstanceId
            && i.ReasonCode == WorkflowEligibilityReasonCodes.WaitingOnDependency);
        Assert.Equal(WorkflowEligibilityReasonCodes.WaitingOnDependency, dependencyWaiting.ReasonCode);
        Assert.Contains(b.InstanceId, dependencyWaiting.RelatedWorkflowInstanceIds);
    }
}
