namespace Edf.Application.Workflow.Eligibility;

public static class WorkflowEligibilityReasonCodes
{
    public const string BlockedLifecycle = "gew.eligibility.blocked.lifecycle";
    public const string BlockedDependency = "gew.eligibility.blocked.dependency";
    public const string BlockedInstanceStop = "gew.eligibility.blocked.instance-stop";
    public const string BlockedAuthorization = "gew.eligibility.blocked.authorization";
    public const string BlockedEffectiveConfiguration = "gew.eligibility.blocked.effective-config";

    public const string FullyGovernedIndeterminateWf1d = "gew.actionability.fully-governed.indeterminate-wf1d-scope";
    public const string FullyGovernedFrontierIndeterminate = "gew.frontier.fully-governed.indeterminate-unevaluated-dimensions";

    public const string WaitingOnDependency = "gew.waiting-on.workflow-dependency";
    public const string WaitingOnInstanceStop = "gew.waiting-on.instance-stop";
    public const string WaitingOnAuthorization = "gew.waiting-on.authorization";
    public const string WaitingOnEffectiveConfiguration = "gew.waiting-on.effective-configuration";
    public const string WaitingOnSynchronizationNotEvaluated = "gew.waiting-on.synchronization.not-evaluated";

    public const string NextActionGovernedPlanningEntry = "gew.next-action.suggest.governed-planning-entry";
    public const string NextActionPlanningRegionWork = "gew.next-action.suggest.planning-region-work";
    public const string NextActionObtainPlanningAuthorization = "gew.next-action.suggest.obtain-planning-authorization";
    public const string NextActionImplementationRegionWork = "gew.next-action.suggest.implementation-region-work";
    public const string NextActionObtainImplementationAuthorization = "gew.next-action.suggest.obtain-implementation-authorization";
    public const string NextActionPostSubmissionReplanningRoute = "gew.next-action.suggest.post-submission-replanning-route";
    public const string NextActionPostSubmissionReworkRoute = "gew.next-action.suggest.post-submission-rework-route";
    public const string NextActionPostSubmissionLifecycleCompletionRoute = "gew.next-action.suggest.post-submission-lifecycle-completion-route";
}
