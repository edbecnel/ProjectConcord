namespace Edf.Application.Operator.PlanningRegion;

using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Operator.WorkState;
using Edf.Domain.Workflow;

public static class PlanningRegionGuidedPresentation
{
    public static string ComposeWhereWeAre() => "Governed Planning (current planning work)";

    public static string ComposeAuthorizationSummary(GovernedWorkStateOperatorProjection projection)
    {
        if (PlanningAuthorizationWorkStateFacts.InstanceNeedsPlanningDevelopmentWorkAuthorization(projection))
        {
            return "Planning development work authorization is not yet on record. This guided surface does not grant authorization.";
        }

        return "Planning development work authorization is on record and permits planning work under its governed scope. "
               + "Repository implementation is not authorized by that Planning development work authorization.";
    }

    public static string ComposeWhyNextStep(PlanningRegionGuidedStep step) =>
        step switch
        {
            PlanningRegionGuidedStep.SendToProjectArchitect =>
                "Project Architect judgment is needed for the next planning decision. ProjectConcord prepares the governed review package behind this action.",
            PlanningRegionGuidedStep.BringBackPaResponse =>
                "ProjectConcord must validate the complete external response before it can be trusted for continuation.",
            PlanningRegionGuidedStep.SendToEngineeringAgent =>
                "A qualifying Project Architect response is on record. The next meaningful step is to dispatch governed work to the Engineering Agent in Plan mode.",
            PlanningRegionGuidedStep.BlockedByStop =>
                "STOP blocks ordinary progression until cleared under governed workflow rules.",
            _ => string.Empty,
        };

    public static string ComposePrimaryActionLabel(PlanningRegionGuidedStep step) =>
        step switch
        {
            PlanningRegionGuidedStep.SendToProjectArchitect => "Send to Project Architect",
            PlanningRegionGuidedStep.BringBackPaResponse => "Validate Project Architect response",
            PlanningRegionGuidedStep.SendToEngineeringAgent => "Send to Engineering Agent",
            _ => string.Empty,
        };
}
