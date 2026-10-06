namespace Edf.Application.Operator.WorkState;

public static class GovernedWorkStateUnavailableReasons
{
    public const string NoEvaluatedWaitingOnConditions = "gew.waiting-on.no-evaluated-blockers";
    public const string NoWorkflowNextActionSuggestions = "gew.next-action.no-workflow-suggestions";

    public const string WorkflowDependencyWait = "gew.waiting-on.workflow-dependency";

    public const string EffectiveConfigMissingProfile = "gew.effective-config.missing-profile";

    public const string EffectiveConfigUnknownProfile = "gew.effective-config.unknown-profile";

    public const string EffectiveConfigUnknownDefinition = "gew.effective-config.unknown-definition";

    public const string EffectiveConfigUnknownPlace = "gew.effective-config.unknown-place";

    public const string InstanceStopped = "gew.work-state.instance-stopped";
}
