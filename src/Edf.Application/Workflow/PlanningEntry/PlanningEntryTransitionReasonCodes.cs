namespace Edf.Application.Workflow.PlanningEntry;

public static class PlanningEntryTransitionReasonCodes
{
    public const string MissingConsumedHandover = "planning-entry.evidence.missing";
    public const string PackageNotValid = "planning-entry.package.not-valid";
    public const string PackageWrongProject = "planning-entry.package.wrong-project";
    public const string ContractNotSatisfied = "planning-entry.contract.not-satisfied";
    public const string RelayStopActive = "planning-entry.relay.stop-active";
    public const string NoApplicableIntakeInstance = "planning-entry.instance.none";
    public const string MultipleApplicableIntakeInstances = "planning-entry.instance.multiple";
    public const string WrongWorkflowPlace = "planning-entry.instance.wrong-place";
    public const string EvaluatedBlocker = "planning-entry.instance.evaluated-blocker";
    public const string AlreadyApplied = "planning-entry.transition.already-applied";
}
