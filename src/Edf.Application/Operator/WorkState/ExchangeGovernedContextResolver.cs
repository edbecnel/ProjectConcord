namespace Edf.Application.Operator.WorkState;

public static class ExchangeGovernedContextResolver
{
    public static bool IsPlanningRegionContinuationApplicable(GovernedWorkStateOperatorProjection projection)
    {
        WorkflowOperatorNextActionItem? primaryNextAction = null;
        foreach (var action in projection.WorkflowNextActions)
        {
            primaryNextAction ??= action;
            break;
        }

        return PlanningRegionWorkContinuationFacts.InstanceShouldOfferGovernedExchangeContinuation(
            projection,
            primaryNextAction);
    }
}
