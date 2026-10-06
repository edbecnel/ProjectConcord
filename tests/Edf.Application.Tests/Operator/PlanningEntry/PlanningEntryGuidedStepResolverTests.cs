using Edf.Application.Composition;
using Edf.Application.Relay;
using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Projects.InMemory;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Operator.PlanningEntry;

public class PlanningEntryGuidedStepResolverTests
{
    [Fact]
    public void IntakeWithQualifyingHandover_ResolvesReviewDecision()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(projectId);
        var validation = RelayValidationResult.Valid([]);
        persistence.RelayOperational.SavePackage(
            new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, validation.Diagnostics));
        workflow.WorkflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        persistence.RelayOperational.AppendProvenanceEvent(
            new RelayProvenanceEvent(
                RelayProvenanceEventId.New(),
                projectId,
                package.PackageId,
                package.CorrelationId,
                RelayProvenanceEventType.PackageConsumed,
                "{}",
                DateTimeOffset.UtcNow));

        var relay = workflow.PlanningEntryRelayReadModel.Resolve(projectId);
        var step = PlanningEntryGuidedStepResolver.Resolve(
            GewV1TopologyPlaces.Intake,
            relay,
            new PlanningEntryGuidedTransientState(),
            workflow.IntakePlanningEntryTransitions,
            projectId,
            sessionIntentsReadyForReviewExport: true);

        Assert.Equal(PlanningEntryGuidedStep.ReviewDecision, step);
    }

    [Fact]
    public void IntakeWithoutReview_ResolvesPrepareWhenSessionReady()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var relay = workflow.PlanningEntryRelayReadModel.Resolve(projectId);

        var step = PlanningEntryGuidedStepResolver.Resolve(
            GewV1TopologyPlaces.Intake,
            relay,
            new PlanningEntryGuidedTransientState(),
            workflow.IntakePlanningEntryTransitions,
            projectId,
            sessionIntentsReadyForReviewExport: true);

        Assert.Equal(PlanningEntryGuidedStep.PrepareReview, step);
    }

    [Fact]
    public void IntakeWithoutSessionIntents_ResolvesConfirmSessionContinuity()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        var projectId = ProjectConcordProjectId.New();
        var relay = workflow.PlanningEntryRelayReadModel.Resolve(projectId);

        var step = PlanningEntryGuidedStepResolver.Resolve(
            GewV1TopologyPlaces.Intake,
            relay,
            new PlanningEntryGuidedTransientState(),
            workflow.IntakePlanningEntryTransitions,
            projectId,
            sessionIntentsReadyForReviewExport: false);

        Assert.Equal(PlanningEntryGuidedStep.ConfirmSessionContinuity, step);
    }
}
