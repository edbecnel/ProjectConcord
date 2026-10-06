using Edf.Domain.Workflow;

namespace Edf.Application.Workflow.Eligibility;

public sealed class GewV1WorkflowInstanceEligibilityEvaluator : IGewV1WorkflowInstanceEligibilityEvaluator
{
    private readonly IPrescribedWorkflowRegistry _registry;

    public GewV1WorkflowInstanceEligibilityEvaluator(IPrescribedWorkflowRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public WorkflowInstanceGovernedEligibility Evaluate(
        WorkflowInstance instance,
        IReadOnlyList<WorkflowInstanceId> unresolvedRequiredWorkflowInstanceIds,
        bool instanceStopActive,
        IReadOnlyList<DevelopmentWorkAuthorization> authorizationRecordsForInstance,
        EffectiveConfigurationResolveResult effectiveConfigurationResult)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentNullException.ThrowIfNull(unresolvedRequiredWorkflowInstanceIds);
        ArgumentNullException.ThrowIfNull(authorizationRecordsForInstance);
        ArgumentNullException.ThrowIfNull(effectiveConfigurationResult);

        var violations = new List<string>();
        var routing = instance.Lifecycle == WorkflowInstanceLifecycle.Active
            ? GewV1PlaceAuthorizationLaw.ResolveRoutingState(instance.TopologyPlaceId)
            : WorkflowInstanceEvaluatedRoutingState.NonActive;

        if (instance.Lifecycle != WorkflowInstanceLifecycle.Active)
        {
            violations.Add(WorkflowEligibilityReasonCodes.BlockedLifecycle);
        }

        if (unresolvedRequiredWorkflowInstanceIds.Count > 0)
        {
            violations.Add(WorkflowEligibilityReasonCodes.BlockedDependency);
        }

        if (instanceStopActive)
        {
            violations.Add(WorkflowEligibilityReasonCodes.BlockedInstanceStop);
        }

        if (effectiveConfigurationResult is EffectiveConfigurationUnresolved)
        {
            violations.Add(WorkflowEligibilityReasonCodes.BlockedEffectiveConfiguration);
        }

        if (instance.Lifecycle == WorkflowInstanceLifecycle.Active)
        {
            EvaluatePlaceAuthorizationLaw(
                instance,
                authorizationRecordsForInstance,
                violations);
        }

        var evaluatedSatisfied = violations.Count == 0;
        var eligible = evaluatedSatisfied && instance.Lifecycle == WorkflowInstanceLifecycle.Active;

        var unevaluatedApplicable = ResolveUnevaluatedApplicableDimensions(instance);
        var applicabilityUnknown = ResolveApplicabilityUnknownDimensions(instance);

        return new WorkflowInstanceGovernedEligibility(
            evaluatedSatisfied,
            violations,
            unevaluatedApplicable,
            applicabilityUnknown,
            eligible,
            GovernedActionabilityCompleteness.Indeterminate,
            WorkflowEligibilityReasonCodes.FullyGovernedIndeterminateWf1d,
            routing);
    }

    private void EvaluatePlaceAuthorizationLaw(
        WorkflowInstance instance,
        IReadOnlyList<DevelopmentWorkAuthorization> authorizationRecordsForInstance,
        List<string> violations)
    {
        if (!GewV1PlaceAuthorizationLaw.RequiresExecutionAuthorizationAtPlace(instance.TopologyPlaceId))
        {
            return;
        }

        var requiredKind = GewV1PlaceAuthorizationLaw.RequiresPlanningAuthorizationAtPlace(instance.TopologyPlaceId)
            ? DevelopmentWorkAuthorizationKind.Planning
            : DevelopmentWorkAuthorizationKind.Implementation;

        var hasApplicable = authorizationRecordsForInstance.Any(record =>
            record.AuthorizationKind == requiredKind
            && DevelopmentWorkAuthorizationApplicability.IsCurrentlyApplicable(record, instance, _registry));

        if (!hasApplicable)
        {
            violations.Add(WorkflowEligibilityReasonCodes.BlockedAuthorization);
        }
    }

    private static IReadOnlyList<GovernedEligibilityDimension> ResolveUnevaluatedApplicableDimensions(
        WorkflowInstance instance)
    {
        // GEW v1 registry (WF-1d) declares no synchronization, AEI, evidence, or Control attachments.
        // Do not treat deferred runtime absence as "applicable but unevaluated" for every instance.
        _ = instance;
        return Array.Empty<GovernedEligibilityDimension>();
    }

    private static IReadOnlyList<GovernedEligibilityDimension> ResolveApplicabilityUnknownDimensions(
        WorkflowInstance instance)
    {
        _ = instance;
        return Array.Empty<GovernedEligibilityDimension>();
    }
}
