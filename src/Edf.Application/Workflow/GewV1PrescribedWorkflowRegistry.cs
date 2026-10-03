using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class GewV1PrescribedWorkflowRegistry : IPrescribedWorkflowRegistry
{
    private static readonly PrescribedWorkflowDefinition GewV1 = BuildGewV1();

    public bool TryGetDefinition(
        PrescribedWorkflowId workflowId,
        WorkflowDefinitionVersion definitionVersion,
        out PrescribedWorkflowDefinition? definition)
    {
        if (workflowId.Value == PrescribedWorkflowIds.GovernedEngineering
            && definitionVersion.Value == WorkflowDefinitionVersion.GewV1.Value)
        {
            definition = GewV1;
            return true;
        }

        definition = null;
        return false;
    }

    public bool IsKnownPlace(PrescribedWorkflowDefinition definition, TopologyPlaceId placeId) =>
        definition.Places.Any(p => p.Value == placeId.Value);

    public bool IsKnownProfile(PrescribedWorkflowDefinition definition, WorkflowProfileId profileId) =>
        definition.KnownProfiles.Any(p => p.Value == profileId.Value);

    public bool IsPermittedTransition(
        PrescribedWorkflowDefinition definition,
        TopologyPlaceId fromPlace,
        TopologyPlaceId toPlace) =>
        definition.Transitions.Any(t => t.From.Value == fromPlace.Value && t.To.Value == toPlace.Value);

    private static PrescribedWorkflowDefinition BuildGewV1()
    {
        var places = new[]
        {
            TopologyPlaceId.Parse(GewV1TopologyPlaces.Intake),
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
            TopologyPlaceId.Parse(GewV1TopologyPlaces.ImplementationGoverned),
            TopologyPlaceId.Parse(GewV1TopologyPlaces.PostSubmissionGoverned),
        };

        var transitions = new (string From, string To)[]
        {
            (GewV1TopologyPlaces.Intake, GewV1TopologyPlaces.PlanningGoverned),
            (GewV1TopologyPlaces.PlanningGoverned, GewV1TopologyPlaces.ImplementationGoverned),
            (GewV1TopologyPlaces.ImplementationGoverned, GewV1TopologyPlaces.PostSubmissionGoverned),
            (GewV1TopologyPlaces.PostSubmissionGoverned, GewV1TopologyPlaces.PlanningGoverned),
            (GewV1TopologyPlaces.PostSubmissionGoverned, GewV1TopologyPlaces.ImplementationGoverned),
        };

        var profiles = new[]
        {
            WorkflowProfileId.Parse(GewV1ProfileIds.Accelerated),
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard),
            WorkflowProfileId.Parse(GewV1ProfileIds.HighAssurance),
        };

        return new PrescribedWorkflowDefinition(
            PrescribedWorkflowId.GovernedEngineering,
            WorkflowDefinitionVersion.GewV1,
            places,
            transitions.Select(t => (TopologyPlaceId.Parse(t.From), TopologyPlaceId.Parse(t.To))).ToList(),
            profiles);
    }
}
