namespace Edf.Application.Operator.WorkState;

public static class GovernedWorkStateUnavailableReasons
{
    public const string NotImplementedInM7aWf1 = "gew.work-state.not-implemented-m7a-wf-1";

    public const string WorkflowDependencyWait = "gew.waiting-on.workflow-dependency";

    public const string EffectiveConfigMissingProfile = "gew.effective-config.missing-profile";

    public const string EffectiveConfigUnknownProfile = "gew.effective-config.unknown-profile";

    public const string EffectiveConfigUnknownDefinition = "gew.effective-config.unknown-definition";

    public const string EffectiveConfigUnknownPlace = "gew.effective-config.unknown-place";

    public const string InstanceStopped = "gew.work-state.instance-stopped";
}
