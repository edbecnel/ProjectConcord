using Edf.Application.Composition;
using Edf.Application.Operator.WorkContinuity;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Operator;

public class OperatorWorkFocusServiceTests
{
    [Fact]
    public void EnsurePlanningRegionWorkFocus_ResumesExistingFocus_WithoutReplacingSubject()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var instanceId = WorkflowInstanceId.New();
        var existing = new OperatorActiveWorkFocus(
            Guid.NewGuid(),
            projectId,
            instanceId,
            "Retained subject",
            OperatorWorkFocusSubjectProvenance.OperatorConfirmed,
            null,
            OperatorWorkFocusResumptionTarget.PlanningRegionWork,
            "Prior continuity",
            null,
            1,
            DateTimeOffset.UtcNow);
        persistence.OperatorWorkFocus.Upsert(existing);

        var projection = workflow.WorkStateOperatorProjection.ProjectForProject(projectId, null);
        var result = workflow.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(projectId, projection, null);

        Assert.False(result.RequiredOperatorSubjectConfirmation);
        Assert.Equal("Retained subject", result.Focus!.SubjectLabel);
        Assert.Equal(existing.FocusId, result.Focus.FocusId);
    }

    [Fact]
    public void EnsurePlanningRegionWorkFocus_UsesGovernedVerificationReference_ForMvrRetainedProjectId()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = new ProjectConcordProjectId(OperatorGovernedVerificationReferences.Mvr0005RetainedProjectId);
        BootstrapPlanningGovernedWithPlanningDwa(workflow, projectId);
        var projection = workflow.WorkStateOperatorProjection.ProjectForProject(projectId, null);

        var result = workflow.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(projectId, projection, null);

        Assert.False(result.RequiredOperatorSubjectConfirmation);
        Assert.Equal(OperatorGovernedVerificationReferences.Mvr0005Key, result.Focus!.GovernedReferenceKey);
        Assert.Contains("MVR-0005", result.Focus.SubjectLabel, StringComparison.Ordinal);
        Assert.Equal(OperatorWorkFocusSubjectProvenance.GovernedVerificationReference, result.Focus.SubjectProvenance);
    }

    [Fact]
    public void EnsurePlanningRegionWorkFocus_RequiresConfirmation_WhenDisplayNameContainsMvr0005_ButNotRetainedProjectId()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        BootstrapPlanningGovernedWithPlanningDwa(workflow, projectId);
        var projection = workflow.WorkStateOperatorProjection.ProjectForProject(projectId, null);

        var result = workflow.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(
            projectId,
            projection,
            "Disposable project folder MVR-0005");

        Assert.True(result.RequiredOperatorSubjectConfirmation);
        Assert.Null(result.Focus);
        Assert.Null(persistence.OperatorWorkFocus.GetActiveForProject(projectId));
    }

    [Fact]
    public void EnsurePlanningRegionWorkFocus_RequiresConfirmation_WhenNoFocusOrGovernedReference()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        BootstrapPlanningGovernedWithPlanningDwa(workflow, projectId);
        var projection = workflow.WorkStateOperatorProjection.ProjectForProject(projectId, null);

        var result = workflow.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(projectId, projection, "Unrelated project");

        Assert.True(result.RequiredOperatorSubjectConfirmation);
        Assert.Null(result.Focus);
    }

    [Fact]
    public void EnsurePlanningRegionWorkFocus_OperatorConfirmedSubject_RecordsProvenanceWithoutGovernedKey()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        BootstrapPlanningGovernedWithPlanningDwa(workflow, projectId);
        var projection = workflow.WorkStateOperatorProjection.ProjectForProject(projectId, null);

        var result = workflow.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(
            projectId,
            projection,
            null,
            "Operator-chosen subject");

        Assert.False(result.RequiredOperatorSubjectConfirmation);
        Assert.Equal(OperatorWorkFocusSubjectProvenance.OperatorConfirmed, result.Focus!.SubjectProvenance);
        Assert.Null(result.Focus.GovernedReferenceKey);
        Assert.Equal("Operator-chosen subject", result.Focus.SubjectLabel);
    }

    private static void BootstrapPlanningGovernedWithPlanningDwa(
        WorkflowApplicationServices workflow,
        ProjectConcordProjectId projectId)
    {
        workflow.WorkflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        var instance = workflow.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
        workflow.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        workflow.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            RelayValidationResult.Valid([]));
    }
}
