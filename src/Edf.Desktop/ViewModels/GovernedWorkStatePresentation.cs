using Edf.Application.Operator.WorkState;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Workflow;

namespace Edf.Desktop.ViewModels;

internal static class GovernedWorkStatePresentation
{
    public static string ShortInstanceLabel(Guid instanceId)
    {
        var text = instanceId.ToString("D");
        return text.Length > 8 ? "…" + text[^8..] : text;
    }

    public static string DescribeProfileLabel(string? profileValue) =>
        profileValue switch
        {
            GewV1ProfileIds.Standard => "Standard",
            GewV1ProfileIds.Accelerated => "Accelerated",
            GewV1ProfileIds.HighAssurance => "High assurance",
            null => "Unspecified profile",
            _ => "Governed workflow",
        };

    public static string DescribeTopologyPlaceStage(string placeId) =>
        placeId switch
        {
            GewV1TopologyPlaces.Intake => "Intake",
            GewV1TopologyPlaces.PlanningGoverned => "Governed Planning",
            GewV1TopologyPlaces.ImplementationGoverned => "Implementation-governed",
            GewV1TopologyPlaces.PostSubmissionGoverned => "Post-submission",
            _ => "Unrecognized stage",
        };

    public static string DescribeTopologyPlace(string placeId) =>
        placeId switch
        {
            GewV1TopologyPlaces.Intake =>
                "Workflow intake — beginning the governed process",
            GewV1TopologyPlaces.PlanningGoverned =>
                "Planning-governed region — planning work under governed rules",
            GewV1TopologyPlaces.ImplementationGoverned =>
                "Implementation-governed region — implementation work under governed rules",
            GewV1TopologyPlaces.PostSubmissionGoverned =>
                "Post-submission — governance after submission",
            _ => "Unrecognized workflow stage",
        };

    public static string ComposeInstanceHeadline(string? profileValue, string placeId) =>
        $"{DescribeProfileLabel(profileValue)} governed workflow — {DescribeTopologyPlaceStage(placeId)}";

    public static string ComposeInstanceSituationSummary(GovernedWorkStateCurrentWorkItem item)
    {
        var segments = new List<string>
        {
            $"You are at {DescribeTopologyPlace(item.TopologyPlaceId.Value).ToLowerInvariant()}.",
        };

        if (item.StopActive)
        {
            segments.Add("STOP is active, so governed work on this instance is halted.");
        }
        else
        {
            segments.Add("STOP is not active.");
        }

        if (item.DependencyBlocked)
        {
            segments.Add("Another workflow instance must be satisfied before this one can proceed.");
        }

        if (item.EffectiveConfigurationAvailability != ProjectionAvailability.Available)
        {
            segments.Add(DescribeEffectiveConfigUnavailable(item.EffectiveConfigurationUnavailableReason));
        }

        segments.Add(DescribeBaselineSituation(item));

        if (item.GovernedEligibility.EvaluatedConstraintViolationCodes.Count > 0)
        {
            var blockers = string.Join(
                "; ",
                item.GovernedEligibility.EvaluatedConstraintViolationCodes.Select(DescribeBlockerCode));
            segments.Add($"Evaluated blockers: {blockers}.");
        }
        else if (!item.DependencyBlocked && !item.StopActive)
        {
            segments.Add("Nothing is blocking this instance under the rules evaluated so far.");
        }

        segments.Add(DescribeAuthorizationSituation(item));

        return string.Join(" ", segments);
    }

    public static string DescribePermissionToExecute(GovernedExecutionPermission permission) =>
        permission switch
        {
            GovernedExecutionPermission.NotApplicable =>
                "Permission to execute does not apply to this suggestion — it is not authorization to proceed.",
            GovernedExecutionPermission.Indeterminate =>
                "Whether you may execute is not determined from the current evaluation — this is not a grant of permission.",
            GovernedExecutionPermission.Denied =>
                "Execution is not permitted under the evaluated rules.",
            _ => $"Permission to execute: {permission} (not authorization).",
        };

    public static string ComposeNextStepOperatorSummary(WorkflowOperatorNextActionItem item)
    {
        var step = DescribeNextStepCodePlain(item.SuggestedStepCode);
        var permission = DescribePermissionToExecute(item.PermissionToExecute);
        return $"Suggested next: {step} Guidance only — not authorization. {permission}";
    }

    public static string DescribeNextStepCodePlain(string stepCode) =>
        stepCode switch
        {
            WorkflowEligibilityReasonCodes.NextActionGovernedPlanningEntry =>
                "complete the planning-entry exchange with the Project Architect, then use Enter Governed Planning when eligible",
            WorkflowEligibilityReasonCodes.NextActionPlanningRegionWork =>
                "continue planning-region work only under fully governed rules",
            WorkflowEligibilityReasonCodes.NextActionObtainPlanningAuthorization =>
                "obtain planning development work authorization",
            WorkflowEligibilityReasonCodes.NextActionImplementationRegionWork =>
                "continue implementation-region work only under fully governed rules",
            WorkflowEligibilityReasonCodes.NextActionObtainImplementationAuthorization =>
                "obtain implementation development work authorization",
            _ => "follow the suggested workflow step",
        };

    public static string DescribeWaitingOnReason(string reasonCode) =>
        reasonCode switch
        {
            WorkflowEligibilityReasonCodes.WaitingOnDependency =>
                "Waiting on another workflow instance",
            WorkflowEligibilityReasonCodes.WaitingOnInstanceStop =>
                "Workflow instance is stopped",
            WorkflowEligibilityReasonCodes.WaitingOnAuthorization =>
                "Development work authorization required",
            WorkflowEligibilityReasonCodes.WaitingOnEffectiveConfiguration =>
                "Workflow profile or configuration must be resolved",
            WorkflowEligibilityReasonCodes.WaitingOnSynchronizationNotEvaluated =>
                "Synchronization requirements not yet evaluated",
            _ => "Waiting on a governed condition",
        };

    public static string DescribeBlockerCode(string code) =>
        code switch
        {
            WorkflowEligibilityReasonCodes.BlockedDependency => "Blocked by workflow dependency",
            WorkflowEligibilityReasonCodes.BlockedInstanceStop => "Blocked by active STOP",
            WorkflowEligibilityReasonCodes.BlockedAuthorization => "Blocked — authorization required",
            WorkflowEligibilityReasonCodes.BlockedEffectiveConfiguration => "Blocked — effective configuration unresolved",
            WorkflowEligibilityReasonCodes.BlockedLifecycle => "Blocked — lifecycle state",
            _ => "Blocked by evaluated constraint",
        };

    public static string DescribeNextStepCode(string stepCode) =>
        stepCode switch
        {
            WorkflowEligibilityReasonCodes.NextActionGovernedPlanningEntry =>
                "Enter governed planning",
            WorkflowEligibilityReasonCodes.NextActionPlanningRegionWork =>
                "Planning-region work (permission indeterminate)",
            WorkflowEligibilityReasonCodes.NextActionObtainPlanningAuthorization =>
                "Obtain planning authorization",
            WorkflowEligibilityReasonCodes.NextActionImplementationRegionWork =>
                "Implementation-region work (permission indeterminate)",
            WorkflowEligibilityReasonCodes.NextActionObtainImplementationAuthorization =>
                "Obtain implementation authorization",
            _ => "Suggested workflow step",
        };

    public static string DescribeEffectiveConfigUnavailable(string? reasonCode) =>
        reasonCode switch
        {
            GovernedWorkStateUnavailableReasons.EffectiveConfigMissingProfile =>
                "Workflow profile is not set on this instance",
            GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownProfile =>
                "Workflow profile is not recognized",
            GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownDefinition =>
                "Workflow definition is not recognized",
            GovernedWorkStateUnavailableReasons.EffectiveConfigUnknownPlace =>
                "Workflow place is not recognized",
            _ => "Effective configuration is not available",
        };

    public static string DescribeAvailabilityUnavailable(string? reasonCode, string context) =>
        reasonCode switch
        {
            GovernedWorkStateUnavailableReasons.NoEvaluatedWaitingOnConditions =>
                $"No evaluated waiting conditions for {context}.",
            GovernedWorkStateUnavailableReasons.NoWorkflowNextActionSuggestions =>
                $"No workflow next-action suggestions for {context}.",
            _ => $"{context} is not available.",
        };

    private static string DescribeBaselineSituation(GovernedWorkStateCurrentWorkItem item)
    {
        return item.HeadDriftsFromStoredBaseline switch
        {
            true => "The repository has moved ahead of the stored governed baseline.",
            false => "The repository matches the stored governed baseline.",
            null => "Baseline comparison was not evaluated for this project root.",
        };
    }

    private static string DescribeAuthorizationSituation(GovernedWorkStateCurrentWorkItem item)
    {
        if (item.ApplicableActiveAuthorizationKinds.Count > 0)
        {
            var kinds = string.Join(", ", item.ApplicableActiveAuthorizationKinds);
            return $"Durable development work authorization on record: {kinds}. "
                   + "Recorded authorization is separate from suggested next steps.";
        }

        var actionability = item.GovernedEligibility.FullyGovernedActionability;
        if (actionability == GovernedActionabilityCompleteness.Indeterminate)
        {
            return "No durable development work authorization is recorded. "
                   + "Full execution permission is not determined from the current evaluation "
                   + "(starting the workflow does not authorize implementation).";
        }

        return "No durable development work authorization is recorded.";
    }

    public static string ComposeIntakeAwaitingPaApprovalSummary() =>
        "What happens next: ProjectConcord needs Project Architect approval to enter governed planning."
        + Environment.NewLine
        + "What to do now: Complete the planning-entry exchange with the Project Architect.";

    public static string ComposeIntakeWithQualifyingHandoverSummary() =>
        "The imported Project Architect handover permits entry into governed planning."
        + Environment.NewLine
        + "Use Enter Governed Planning when you are ready. ProjectConcord will record the governed workflow transition using that handover as provenance."
        + Environment.NewLine
        + "This does not authorize implementation work and does not create durable development work authorization.";

    public static string ComposeGovernedPlanningEnteredSummary(GovernedWorkStateCurrentWorkItem item)
    {
        var auth = DescribeAuthorizationSituation(item);
        return "The project has entered the governed planning stage."
               + Environment.NewLine
               + auth;
    }
}
