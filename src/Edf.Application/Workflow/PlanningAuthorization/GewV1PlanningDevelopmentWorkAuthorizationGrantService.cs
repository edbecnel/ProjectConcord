namespace Edf.Application.Workflow.PlanningAuthorization;

using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

public sealed class GewV1PlanningDevelopmentWorkAuthorizationGrantService
    : IGewV1PlanningDevelopmentWorkAuthorizationGrantService
{
    private readonly IGovernedWorkStateRecoveryService _recovery;
    private readonly IGewV1EffectiveConfigurationResolver _effectiveConfigurationResolver;
    private readonly IGewV1WorkflowInstanceEligibilityEvaluator _eligibilityEvaluator;
    private readonly IPrescribedWorkflowRegistry _registry;
    private readonly IDevelopmentWorkAuthorizationService _developmentWorkAuthorizations;

    public GewV1PlanningDevelopmentWorkAuthorizationGrantService(
        IGovernedWorkStateRecoveryService recovery,
        IGewV1EffectiveConfigurationResolver effectiveConfigurationResolver,
        IGewV1WorkflowInstanceEligibilityEvaluator eligibilityEvaluator,
        IPrescribedWorkflowRegistry registry,
        IDevelopmentWorkAuthorizationService developmentWorkAuthorizations)
    {
        _recovery = recovery ?? throw new ArgumentNullException(nameof(recovery));
        _effectiveConfigurationResolver = effectiveConfigurationResolver
            ?? throw new ArgumentNullException(nameof(effectiveConfigurationResolver));
        _eligibilityEvaluator = eligibilityEvaluator
            ?? throw new ArgumentNullException(nameof(eligibilityEvaluator));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _developmentWorkAuthorizations = developmentWorkAuthorizations
            ?? throw new ArgumentNullException(nameof(developmentWorkAuthorizations));
    }

    public PlanningAuthorizationGrantEligibility EvaluateGrantEligibility(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover,
        RelayValidationResult validation)
    {
        ArgumentNullException.ThrowIfNull(consumedPaHandover);
        ArgumentNullException.ThrowIfNull(validation);

        if (consumedPaHandover.ProjectId != projectId)
        {
            return NotEligible(PlanningAuthorizationGrantReasonCodes.PackageWrongProject);
        }

        if (!PlanningAuthorizationPaHandoverContract.SatisfiesPlanningAuthorizationGrantContract(
                consumedPaHandover,
                validation,
                out var contractReason))
        {
            return NotEligible(contractReason);
        }

        return EvaluateInstanceApplicability(projectId, consumedPaHandover);
    }

    public PlanningAuthorizationGrantResult TryRecordPlanningAuthorizationGrant(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover,
        RelayValidationResult validation)
    {
        ArgumentNullException.ThrowIfNull(consumedPaHandover);
        ArgumentNullException.ThrowIfNull(validation);

        var eligibility = EvaluateGrantEligibility(projectId, consumedPaHandover, validation);
        if (!eligibility.IsEligible)
        {
            if (eligibility.ReasonCode == PlanningAuthorizationGrantReasonCodes.AlreadyApplied
                && eligibility.WorkflowInstanceId is not null)
            {
                var existing = FindAuthorizationGrantedFromPackage(projectId, consumedPaHandover.PackageId);
                return new PlanningAuthorizationGrantResult(
                    PlanningAuthorizationGrantOutcome.AlreadyApplied,
                    PlanningAuthorizationGrantReasonCodes.AlreadyApplied,
                    existing,
                    "Planning development work authorization was already recorded for this Project Architect response.");
            }

            return new PlanningAuthorizationGrantResult(
                PlanningAuthorizationGrantOutcome.NotEligible,
                eligibility.ReasonCode,
                null,
                DescribeIneligibility(eligibility.ReasonCode));
        }

        if (eligibility.WorkflowInstanceId is not { } instanceId)
        {
            return new PlanningAuthorizationGrantResult(
                PlanningAuthorizationGrantOutcome.NotEligible,
                PlanningAuthorizationGrantReasonCodes.NoApplicableInstance,
                null,
                DescribeIneligibility(PlanningAuthorizationGrantReasonCodes.NoApplicableInstance));
        }

        if (!SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                consumedPaHandover.ProfilePayload,
                out var payload,
                out _)
            || payload?.DevelopmentWorkAuthorization is not { } dwaProjection)
        {
            return new PlanningAuthorizationGrantResult(
                PlanningAuthorizationGrantOutcome.NotEligible,
                PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied,
                null,
                DescribeIneligibility(PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied));
        }

        var authority = GovernedWorkflowMutationAuthority.FromRelayProvenance(
            consumedPaHandover.CorrelationId,
            consumedPaHandover.PackageId);

        try
        {
            var granted = _developmentWorkAuthorizations.RecordGovernedGrant(
                instanceId,
                DevelopmentWorkAuthorizationKind.Planning,
                dwaProjection.AuthorizedTrancheId,
                dwaProjection.AuthorizedScopeMarkers,
                dwaProjection.AuthorizationReference,
                authority);

            return new PlanningAuthorizationGrantResult(
                PlanningAuthorizationGrantOutcome.Succeeded,
                string.Empty,
                granted,
                "ProjectConcord recorded Planning development work authorization from the validated Project Architect response. "
                + "This authorizes planning-region governed work under evaluated rules — not implementation in the repository.");
        }
        catch (DevelopmentWorkAuthorizationOperationException ex)
        {
            return new PlanningAuthorizationGrantResult(
                PlanningAuthorizationGrantOutcome.NotEligible,
                PlanningAuthorizationGrantReasonCodes.EvaluatedBlocker,
                null,
                ex.Message);
        }
    }

    private PlanningAuthorizationGrantEligibility EvaluateInstanceApplicability(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover)
    {
        var recovery = _recovery.RecoverForProject(projectId);
        var replay = FindAuthorizationGrantedFromPackage(projectId, consumedPaHandover.PackageId);
        if (replay is not null)
        {
            return new PlanningAuthorizationGrantEligibility(
                false,
                PlanningAuthorizationGrantReasonCodes.AlreadyApplied,
                replay.WorkflowInstanceId);
        }

        var candidates = SelectPlanningGovernedInstancesNeedingPlanningDwa(recovery);
        if (candidates.Count == 0)
        {
            if (recovery.ActiveInstances.Any(i =>
                    i.TopologyPlaceId.Value != GewV1TopologyPlaces.PlanningGoverned && IsGewV1Active(i)))
            {
                return NotEligible(PlanningAuthorizationGrantReasonCodes.WrongWorkflowPlace);
            }

            return NotEligible(PlanningAuthorizationGrantReasonCodes.NoApplicableInstance);
        }

        if (candidates.Count > 1)
        {
            return NotEligible(PlanningAuthorizationGrantReasonCodes.MultipleApplicableInstances);
        }

        return new PlanningAuthorizationGrantEligibility(
            true,
            string.Empty,
            candidates[0].InstanceId);
    }

    private List<WorkflowInstance> SelectPlanningGovernedInstancesNeedingPlanningDwa(
        GovernedWorkStateRecoverySnapshot recovery)
    {
        var authorizationsByInstance = recovery.DevelopmentWorkAuthorizations
            .GroupBy(a => a.WorkflowInstanceId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<DevelopmentWorkAuthorization>)g.ToList());

        var pendingByBlocked = recovery.Dependencies
            .Where(d => d.Status == WorkflowDependencyStatus.Pending)
            .GroupBy(d => d.BlockedWorkflowInstanceId)
            .ToDictionary(g => g.Key, g => g.Select(d => d.RequiredWorkflowInstanceId).ToList());

        var stopByInstance = recovery.WorkflowInstanceStopSummaries
            .ToDictionary(s => s.WorkflowInstanceId);

        var result = new List<WorkflowInstance>();
        foreach (var instance in recovery.ActiveInstances.Where(IsGewV1Active))
        {
            if (instance.TopologyPlaceId.Value != GewV1TopologyPlaces.PlanningGoverned)
            {
                continue;
            }

            IReadOnlyList<WorkflowInstanceId> unresolved = pendingByBlocked.TryGetValue(instance.InstanceId, out var required)
                ? required
                : Array.Empty<WorkflowInstanceId>();

            var instanceAuthorizations = authorizationsByInstance.TryGetValue(instance.InstanceId, out var records)
                ? records
                : Array.Empty<DevelopmentWorkAuthorization>();

            var stopActive = stopByInstance.TryGetValue(instance.InstanceId, out var stop) && stop.IsStopActive;

            var effectiveConfig = _effectiveConfigurationResolver.Resolve(
                new EffectiveConfigurationResolveInput(instance, instanceAuthorizations));

            var eligibility = _eligibilityEvaluator.Evaluate(
                instance,
                unresolved,
                stopActive,
                instanceAuthorizations,
                effectiveConfig);

            if (eligibility.EvaluatedConstraintViolationCodes.Contains(
                    WorkflowEligibilityReasonCodes.BlockedAuthorization))
            {
                result.Add(instance);
            }
        }

        return result;
    }

    private DevelopmentWorkAuthorization? FindAuthorizationGrantedFromPackage(
        ProjectConcordProjectId projectId,
        GovernedPackageId packageId)
    {
        return _recovery.RecoverForProject(projectId).DevelopmentWorkAuthorizations
            .FirstOrDefault(a => a.GrantPackageId == packageId);
    }

    private static bool IsGewV1Active(WorkflowInstance instance) =>
        instance.Lifecycle == WorkflowInstanceLifecycle.Active
        && instance.WorkflowId == PrescribedWorkflowId.GovernedEngineering
        && instance.DefinitionVersion == WorkflowDefinitionVersion.GewV1;

    private static PlanningAuthorizationGrantEligibility NotEligible(string reasonCode) =>
        new(false, reasonCode, null);

    private static string DescribeIneligibility(string reasonCode) =>
        reasonCode switch
        {
            PlanningAuthorizationGrantReasonCodes.MissingConsumedHandover =>
                "A validated Project Architect response is required before recording Planning development work authorization.",
            PlanningAuthorizationGrantReasonCodes.PackageWrongProject =>
                "This Project Architect response belongs to a different project.",
            PlanningAuthorizationGrantReasonCodes.RelayStopActive =>
                "STOP is active on the imported response. Resolve STOP before recording authorization.",
            PlanningAuthorizationGrantReasonCodes.MultipleApplicableInstances =>
                "More than one workflow instance needs Planning development work authorization. Resolve ambiguity before continuing.",
            PlanningAuthorizationGrantReasonCodes.NoApplicableInstance =>
                "No active planning-governed workflow instance currently requires Planning development work authorization.",
            PlanningAuthorizationGrantReasonCodes.WrongWorkflowPlace =>
                "The active workflow is not in the planning-governed stage that requires this authorization.",
            PlanningAuthorizationGrantReasonCodes.EvaluatedBlocker =>
                "Evaluated workflow rules block recording Planning development work authorization for this instance.",
            PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied =>
                "The Project Architect response does not satisfy the Planning development work authorization contract.",
            PlanningAuthorizationGrantReasonCodes.PackageNotValid =>
                "The Project Architect response did not pass relay validation.",
            _ => "Planning development work authorization cannot be recorded from this response.",
        };
}
