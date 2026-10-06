using Edf.Domain.Workflow;

namespace Edf.Application.Workflow.Eligibility;

internal static class GewV1PlaceAuthorizationLaw
{
    public static WorkflowInstanceEvaluatedRoutingState ResolveRoutingState(TopologyPlaceId topologyPlaceId)
    {
        var value = topologyPlaceId.Value;
        if (value == GewV1TopologyPlaces.Intake)
        {
            return WorkflowInstanceEvaluatedRoutingState.IntakeAwaitingGovernedPlanningEntry;
        }

        if (value == GewV1TopologyPlaces.PlanningGoverned)
        {
            return WorkflowInstanceEvaluatedRoutingState.PlanningGovernedRegion;
        }

        if (value == GewV1TopologyPlaces.ImplementationGoverned)
        {
            return WorkflowInstanceEvaluatedRoutingState.ImplementationGovernedRegion;
        }

        if (value == GewV1TopologyPlaces.PostSubmissionGoverned)
        {
            return WorkflowInstanceEvaluatedRoutingState.PostSubmissionAwaitingGovernance;
        }

        return WorkflowInstanceEvaluatedRoutingState.NonActive;
    }

    public static bool RequiresPlanningAuthorizationAtPlace(TopologyPlaceId topologyPlaceId) =>
        topologyPlaceId.Value == GewV1TopologyPlaces.PlanningGoverned;

    public static bool RequiresImplementationAuthorizationAtPlace(TopologyPlaceId topologyPlaceId) =>
        topologyPlaceId.Value == GewV1TopologyPlaces.ImplementationGoverned;

    public static bool RequiresExecutionAuthorizationAtPlace(TopologyPlaceId topologyPlaceId) =>
        RequiresPlanningAuthorizationAtPlace(topologyPlaceId)
        || RequiresImplementationAuthorizationAtPlace(topologyPlaceId);
}
