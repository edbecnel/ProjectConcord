using Edf.Application.Operator;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public static class WorkflowNextActionContributor
{
    public static IReadOnlyList<WorkflowOperatorNextActionItem> Contribute(
        IReadOnlyList<GovernedWorkStateCurrentWorkItem> currentWork)
    {
        var items = new List<WorkflowOperatorNextActionItem>();
        foreach (var work in currentWork)
        {
            items.AddRange(ContributeForInstance(work));
        }

        return items
            .OrderBy(i => i.WorkflowInstanceId.Value)
            .ThenBy(i => i.SuggestedStepCode, StringComparer.Ordinal)
            .ToList();
    }

    private static IEnumerable<WorkflowOperatorNextActionItem> ContributeForInstance(
        GovernedWorkStateCurrentWorkItem work)
    {
        var eligibility = work.GovernedEligibility;
        var permissionWhenBlocked = GovernedExecutionPermission.Denied;
        var permissionWhenRoutingOnly = GovernedExecutionPermission.NotApplicable;
        var permissionWhenEvaluatedOk = GovernedExecutionPermission.Indeterminate;

        switch (eligibility.RoutingState)
        {
            case WorkflowInstanceEvaluatedRoutingState.IntakeAwaitingGovernedPlanningEntry:
                yield return new WorkflowOperatorNextActionItem(
                    work.InstanceId,
                    WorkflowEligibilityReasonCodes.NextActionGovernedPlanningEntry,
                    eligibility.EvaluatedConstraintsSatisfied
                        ? permissionWhenRoutingOnly
                        : permissionWhenBlocked,
                    OperatorNextActionClass.Available,
                    "Governed entry toward planning-governed region (topology mutation not performed by projection).");
                break;

            case WorkflowInstanceEvaluatedRoutingState.PlanningGovernedRegion:
                if (eligibility.EvaluatedConstraintViolationCodes.Contains(
                        WorkflowEligibilityReasonCodes.BlockedAuthorization))
                {
                    yield return new WorkflowOperatorNextActionItem(
                        work.InstanceId,
                        WorkflowEligibilityReasonCodes.NextActionObtainPlanningAuthorization,
                        permissionWhenBlocked,
                        OperatorNextActionClass.Required,
                        "Obtain applicable Planning development work authorization for the current traversal occurrence.");
                }
                else if (eligibility.EvaluatedConstraintsSatisfied)
                {
                    yield return new WorkflowOperatorNextActionItem(
                        work.InstanceId,
                        WorkflowEligibilityReasonCodes.NextActionPlanningRegionWork,
                        permissionWhenEvaluatedOk,
                        OperatorNextActionClass.Available,
                        "Planning-governed region work may proceed only under fully governed rules (permission indeterminate in WF-1d).");
                }

                break;

            case WorkflowInstanceEvaluatedRoutingState.ImplementationGovernedRegion:
                if (eligibility.EvaluatedConstraintViolationCodes.Contains(
                        WorkflowEligibilityReasonCodes.BlockedAuthorization))
                {
                    yield return new WorkflowOperatorNextActionItem(
                        work.InstanceId,
                        WorkflowEligibilityReasonCodes.NextActionObtainImplementationAuthorization,
                        permissionWhenBlocked,
                        OperatorNextActionClass.Required,
                        "Obtain applicable Implementation development work authorization for the current traversal occurrence.");
                }
                else if (eligibility.EvaluatedConstraintsSatisfied)
                {
                    yield return new WorkflowOperatorNextActionItem(
                        work.InstanceId,
                        WorkflowEligibilityReasonCodes.NextActionImplementationRegionWork,
                        permissionWhenEvaluatedOk,
                        OperatorNextActionClass.Available,
                        "Implementation-governed region work may proceed only under fully governed rules (permission indeterminate in WF-1d).");
                }

                break;

            case WorkflowInstanceEvaluatedRoutingState.PostSubmissionAwaitingGovernance:
            {
                var postSubmissionRoutingPermission = eligibility.EvaluatedConstraintsSatisfied
                    ? permissionWhenRoutingOnly
                    : permissionWhenBlocked;
                yield return new WorkflowOperatorNextActionItem(
                    work.InstanceId,
                    WorkflowEligibilityReasonCodes.NextActionPostSubmissionReplanningRoute,
                    postSubmissionRoutingPermission,
                    OperatorNextActionClass.Available,
                    "Topology-supported replanning route toward planning-governed (not authorized by projection).");
                yield return new WorkflowOperatorNextActionItem(
                    work.InstanceId,
                    WorkflowEligibilityReasonCodes.NextActionPostSubmissionReworkRoute,
                    postSubmissionRoutingPermission,
                    OperatorNextActionClass.Available,
                    "Topology-supported rework route toward implementation-governed (not authorized by projection).");
                yield return new WorkflowOperatorNextActionItem(
                    work.InstanceId,
                    WorkflowEligibilityReasonCodes.NextActionPostSubmissionLifecycleCompletionRoute,
                    postSubmissionRoutingPermission,
                    OperatorNextActionClass.Available,
                    "Topology-supported lifecycle completion from post-submission (not authorized by projection).");
                break;
            }
        }
    }
}
