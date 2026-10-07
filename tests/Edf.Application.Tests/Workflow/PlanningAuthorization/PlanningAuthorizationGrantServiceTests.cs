using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Application.Workflow.PlanningAuthorization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow.PlanningAuthorization;

public class PlanningAuthorizationGrantServiceTests
{
    [Fact]
    public void ValidImportAlone_DoesNotRecordDwa()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var instance = CreatePlanningGovernedInstance(services, projectId);

        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        var validation = RelayValidationResult.Valid([]);

        var before = persistence.DevelopmentWorkAuthorizations.ListByProject(projectId).Count;
        var eligibility = services.PlanningAuthorizationGrants.EvaluateGrantEligibility(
            projectId,
            handover,
            validation);
        Assert.True(eligibility.IsEligible);
        Assert.Equal(before, persistence.DevelopmentWorkAuthorizations.ListByProject(projectId).Count);
    }

    [Fact]
    public void TryRecordGrant_QualifyingHandover_RecordsPlanningDwa()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var instance = CreatePlanningGovernedInstance(services, projectId);

        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        var validation = RelayValidationResult.Valid([]);

        var result = services.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            validation);

        Assert.Equal(PlanningAuthorizationGrantOutcome.Succeeded, result.Outcome);
        Assert.NotNull(result.Authorization);
        Assert.Equal(DevelopmentWorkAuthorizationKind.Planning, result.Authorization!.AuthorizationKind);
        Assert.Equal(instance.InstanceId, result.Authorization.WorkflowInstanceId);

        var projection = services.WorkStateOperatorProjection.ProjectForProject(projectId, null);
        var work = projection.CurrentWork.Single();
        Assert.DoesNotContain(
            WorkflowEligibilityReasonCodes.BlockedAuthorization,
            work.GovernedEligibility.EvaluatedConstraintViolationCodes);
    }

    [Fact]
    public void TryRecordGrant_PlanningEntryHandover_FailsClosed()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        CreatePlanningGovernedInstance(services, projectId);

        var handover = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(projectId);
        var validation = RelayValidationResult.Valid([]);

        var result = services.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            validation);

        Assert.Equal(PlanningAuthorizationGrantOutcome.NotEligible, result.Outcome);
        Assert.Empty(persistence.DevelopmentWorkAuthorizations.ListByProject(projectId));
    }

    [Fact]
    public void TryRecordGrant_ImplementationDwaProjection_FailsClosed()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        CreatePlanningGovernedInstance(services, projectId);

        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(
            projectId,
            p => p with
            {
                DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                    SoftwareDevelopmentAuthorizationKind.Implementation,
                    null,
                    ["scope"],
                    "ref",
                    false),
            });
        var validation = RelayValidationResult.Valid([]);

        var result = services.PlanningAuthorizationGrants.TryRecordPlanningAuthorizationGrant(
            projectId,
            handover,
            validation);

        Assert.Equal(PlanningAuthorizationGrantOutcome.NotEligible, result.Outcome);
        Assert.Empty(persistence.DevelopmentWorkAuthorizations.ListByProject(projectId));
    }

    private static WorkflowInstance CreatePlanningGovernedInstance(
        WorkflowApplicationServices services,
        ProjectConcordProjectId projectId)
    {
        var instance = services.WorkflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        services.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthority.FromRelayProvenance(
                GovernedCorrelationId.New(),
                GovernedPackageId.New()));

        return services.WorkStateRecovery.RecoverForProject(projectId).ActiveInstances.Single();
    }
}
