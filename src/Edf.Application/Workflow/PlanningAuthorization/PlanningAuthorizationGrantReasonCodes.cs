namespace Edf.Application.Workflow.PlanningAuthorization;

public static class PlanningAuthorizationGrantReasonCodes
{
    public const string MissingConsumedHandover = "planning-auth.grant.missing-consumed-handover";
    public const string PackageNotValid = "planning-auth.grant.package-not-valid";
    public const string PackageWrongProject = "planning-auth.grant.package-wrong-project";
    public const string ContractNotSatisfied = "planning-auth.grant.contract-not-satisfied";
    public const string RelayStopActive = "planning-auth.grant.relay-stop-active";
    public const string NoApplicableInstance = "planning-auth.grant.no-applicable-instance";
    public const string MultipleApplicableInstances = "planning-auth.grant.multiple-applicable-instances";
    public const string WrongWorkflowPlace = "planning-auth.grant.wrong-workflow-place";
    public const string EvaluatedBlocker = "planning-auth.grant.evaluated-blocker";
    public const string AlreadyApplied = "planning-auth.grant.already-applied";
}
