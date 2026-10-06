using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class DevelopmentWorkAuthorizationApplicabilityTests
{
    [Fact]
    public void Applicability_RequiresMatchingGrantPlaceAndOccurrence()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-dwa-app-").FullName),
            "dwa",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var registry = new GewV1PrescribedWorkflowRegistry();
        var instance = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        instance = services.WorkflowInstances.RecordGovernedTopologyTransition(
            instance.InstanceId,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            instance.ResourceVersion,
            GovernedWorkflowTransitionAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        var grant = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        Assert.True(DevelopmentWorkAuthorizationApplicability.IsCurrentlyApplicable(grant, instance, registry));

        var wrongPlace = instance with
        {
            TopologyPlaceId = TopologyPlaceId.Parse(GewV1TopologyPlaces.ImplementationGoverned),
        };
        Assert.False(DevelopmentWorkAuthorizationApplicability.IsCurrentlyApplicable(grant, wrongPlace, registry));

        var wrongOccurrence = instance with { TraversalOccurrenceId = TraversalOccurrenceId.New() };
        Assert.False(DevelopmentWorkAuthorizationApplicability.IsCurrentlyApplicable(grant, wrongOccurrence, registry));
    }
}
