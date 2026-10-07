namespace Edf.Desktop.ViewModels;

internal static class ExchangeGovernedContextPresentation
{
    public const string PlanningRegionContinuationSummary =
        "Where you are: Governed Planning"
        + "\n"
        + "Current authorization: Planning development work authorization is on record."
        + " Repository implementation is not authorized by this Planning development work authorization."
        + "\n"
        + "Why you are here: Continue governed planning work through a Project Architect / Engineering Agent exchange."
        + "\n"
        + "Recommended Engineering Agent mode: Plan"
        + "\n"
        + "Why: This exchange is continuing planning-region work."
        + "\n"
        + "Important: Engineering Agent mode is a routing/execution intent, not development work authorization."
        + " Choosing Agent does not grant implementation permission."
        + " Implementation-directed work still requires its own governed authorization.";

    public const string PriorModeExplanation =
        "Prior Engineering Agent mode is only needed when your selected mode differs from the mode used on the last governed package (a mode transition).";

    public const string RecommendedPlanNotice =
        "Plan is recommended for this governed operation (not authorization to proceed). You may select Agent if your next Engineering Agent leg requires it — that does not grant implementation permission.";
}
