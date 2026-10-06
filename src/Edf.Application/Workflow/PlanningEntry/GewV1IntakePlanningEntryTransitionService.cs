namespace Edf.Application.Workflow.PlanningEntry;

using Edf.Application.Relay;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

public sealed class GewV1IntakePlanningEntryTransitionService : IGewV1IntakePlanningEntryTransitionService
{
    private readonly IGovernedWorkStateRecoveryService _recovery;
    private readonly IGewV1EffectiveConfigurationResolver _effectiveConfigurationResolver;
    private readonly IGewV1WorkflowInstanceEligibilityEvaluator _eligibilityEvaluator;
    private readonly IPrescribedWorkflowRegistry _registry;
    private readonly IWorkflowInstanceService _workflowInstances;

    public GewV1IntakePlanningEntryTransitionService(
        IGovernedWorkStateRecoveryService recovery,
        IGewV1EffectiveConfigurationResolver effectiveConfigurationResolver,
        IGewV1WorkflowInstanceEligibilityEvaluator eligibilityEvaluator,
        IPrescribedWorkflowRegistry registry,
        IWorkflowInstanceService workflowInstances)
    {
        _recovery = recovery ?? throw new ArgumentNullException(nameof(recovery));
        _effectiveConfigurationResolver = effectiveConfigurationResolver
            ?? throw new ArgumentNullException(nameof(effectiveConfigurationResolver));
        _eligibilityEvaluator = eligibilityEvaluator
            ?? throw new ArgumentNullException(nameof(eligibilityEvaluator));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _workflowInstances = workflowInstances ?? throw new ArgumentNullException(nameof(workflowInstances));
    }

    public IntakePlanningEntryEligibility EvaluateEligibility(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage? consumedPaHandover,
        RelayValidationResult? consumedValidation)
    {
        if (consumedPaHandover is null || consumedValidation is null)
        {
            return NotEligible(PlanningEntryTransitionReasonCodes.MissingConsumedHandover);
        }

        if (consumedPaHandover.ProjectId != projectId)
        {
            return NotEligible(PlanningEntryTransitionReasonCodes.PackageWrongProject);
        }

        if (!PlanningEntryPaHandoverContract.SatisfiesPlanningEntryContract(
                consumedPaHandover,
                consumedValidation,
                out var contractReason))
        {
            return NotEligible(contractReason);
        }

        return EvaluateInstanceApplicability(projectId, consumedPaHandover);
    }

    public IntakePlanningEntryTransitionResult TryEnterGovernedPlanning(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover,
        RelayValidationResult consumedValidation)
    {
        ArgumentNullException.ThrowIfNull(consumedPaHandover);
        ArgumentNullException.ThrowIfNull(consumedValidation);

        var eligibility = EvaluateEligibility(projectId, consumedPaHandover, consumedValidation);
        if (!eligibility.IsEligible)
        {
            if (eligibility.ReasonCode == PlanningEntryTransitionReasonCodes.AlreadyApplied
                && eligibility.ApplicableInstanceId is { } replayId)
            {
                var replayInstance = _recovery.RecoverForProject(projectId).ActiveInstances
                    .FirstOrDefault(i => i.InstanceId == replayId);
                if (replayInstance is not null)
                {
                    return new IntakePlanningEntryTransitionResult(
                        IntakePlanningEntryTransitionOutcome.AlreadyApplied,
                        PlanningEntryTransitionReasonCodes.AlreadyApplied,
                        replayInstance,
                        "Governed planning entry was already recorded for this PA handover.");
                }
            }

            return new IntakePlanningEntryTransitionResult(
                IntakePlanningEntryTransitionOutcome.NotEligible,
                eligibility.ReasonCode,
                null,
                DescribeIneligibility(eligibility.ReasonCode));
        }

        if (eligibility.ApplicableInstanceId is not { } instanceId
            || eligibility.ApplicableInstanceResourceVersion is not { } resourceVersion)
        {
            return new IntakePlanningEntryTransitionResult(
                IntakePlanningEntryTransitionOutcome.NotEligible,
                PlanningEntryTransitionReasonCodes.NoApplicableIntakeInstance,
                null,
                DescribeIneligibility(PlanningEntryTransitionReasonCodes.NoApplicableIntakeInstance));
        }

        var authority = GovernedWorkflowTransitionAuthority.FromRelayProvenance(
            consumedPaHandover.CorrelationId,
            consumedPaHandover.PackageId);

        try
        {
            var updated = _workflowInstances.RecordGovernedTopologyTransition(
                instanceId,
                TopologyPlaceId.Parse(GewV1TopologyPlaces.PlanningGoverned),
                resourceVersion,
                authority);

            return new IntakePlanningEntryTransitionResult(
                IntakePlanningEntryTransitionOutcome.Succeeded,
                string.Empty,
                updated,
                "ProjectConcord recorded entry into governed planning using the imported PA handover as provenance.");
        }
        catch (WorkflowInstanceConcurrencyException)
        {
            throw;
        }
        catch (WorkflowInstanceOperationException ex)
        {
            return new IntakePlanningEntryTransitionResult(
                IntakePlanningEntryTransitionOutcome.NotEligible,
                PlanningEntryTransitionReasonCodes.EvaluatedBlocker,
                null,
                ex.Message);
        }
    }

    private IntakePlanningEntryEligibility EvaluateInstanceApplicability(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover)
    {
        var recovery = _recovery.RecoverForProject(projectId);
        var intakeInstances = SelectActiveGewV1IntakeInstances(recovery.ActiveInstances);

        if (intakeInstances.Count == 0)
        {
            var replay = TryFindReplayInstance(recovery.ActiveInstances, consumedPaHandover.PackageId);
            if (replay is not null)
            {
                return new IntakePlanningEntryEligibility(
                    false,
                    PlanningEntryTransitionReasonCodes.AlreadyApplied,
                    replay.InstanceId,
                    replay.ResourceVersion);
            }

            if (recovery.ActiveInstances.Any(i =>
                    i.TopologyPlaceId.Value != GewV1TopologyPlaces.Intake
                    && IsGewV1Active(i)))
            {
                return NotEligible(PlanningEntryTransitionReasonCodes.WrongWorkflowPlace);
            }

            return NotEligible(PlanningEntryTransitionReasonCodes.NoApplicableIntakeInstance);
        }

        if (intakeInstances.Count > 1)
        {
            return NotEligible(PlanningEntryTransitionReasonCodes.MultipleApplicableIntakeInstances);
        }

        var instance = intakeInstances[0];
        if (!InstancePermitsPlanningEntryTransition(recovery, instance))
        {
            return NotEligible(PlanningEntryTransitionReasonCodes.EvaluatedBlocker);
        }

        return new IntakePlanningEntryEligibility(
            true,
            string.Empty,
            instance.InstanceId,
            instance.ResourceVersion);
    }

    private bool InstancePermitsPlanningEntryTransition(
        GovernedWorkStateRecoverySnapshot recovery,
        WorkflowInstance instance)
    {
        if (instance.TopologyPlaceId.Value != GewV1TopologyPlaces.Intake)
        {
            return false;
        }

        var pendingByBlocked = recovery.Dependencies
            .Where(d => d.Status == WorkflowDependencyStatus.Pending)
            .GroupBy(d => d.BlockedWorkflowInstanceId)
            .ToDictionary(g => g.Key, g => g.Select(d => d.RequiredWorkflowInstanceId).ToList());

        IReadOnlyList<WorkflowInstanceId> unresolved = pendingByBlocked.TryGetValue(instance.InstanceId, out var required)
            ? required
            : Array.Empty<WorkflowInstanceId>();

        var instanceAuthorizations = recovery.DevelopmentWorkAuthorizations
            .Where(a => a.WorkflowInstanceId == instance.InstanceId)
            .ToList();

        var stopActive = recovery.WorkflowInstanceStopSummaries
            .Any(s => s.WorkflowInstanceId == instance.InstanceId && s.IsStopActive);

        var effectiveConfig = _effectiveConfigurationResolver.Resolve(
            new EffectiveConfigurationResolveInput(instance, instanceAuthorizations));

        var eligibility = _eligibilityEvaluator.Evaluate(
            instance,
            unresolved,
            stopActive,
            instanceAuthorizations,
            effectiveConfig);

        return eligibility.EvaluatedConstraintsSatisfied
               && eligibility.RoutingState == WorkflowInstanceEvaluatedRoutingState.IntakeAwaitingGovernedPlanningEntry;
    }

    private static WorkflowInstance? TryFindReplayInstance(
        IReadOnlyList<WorkflowInstance> activeInstances,
        GovernedPackageId packageId)
    {
        var matches = activeInstances
            .Where(i => IsGewV1Active(i)
                        && i.TopologyPlaceId.Value == GewV1TopologyPlaces.PlanningGoverned
                        && i.LastGovernedTransitionPackageId == packageId)
            .ToList();

        return matches.Count == 1 ? matches[0] : null;
    }

    private static List<WorkflowInstance> SelectActiveGewV1IntakeInstances(IReadOnlyList<WorkflowInstance> activeInstances) =>
        activeInstances
            .Where(i => IsGewV1Active(i) && i.TopologyPlaceId.Value == GewV1TopologyPlaces.Intake)
            .ToList();

    private static bool IsGewV1Active(WorkflowInstance instance) =>
        instance.Lifecycle == WorkflowInstanceLifecycle.Active
        && instance.WorkflowId == PrescribedWorkflowId.GovernedEngineering
        && instance.DefinitionVersion == WorkflowDefinitionVersion.GewV1;

    private static IntakePlanningEntryEligibility NotEligible(string reasonCode) =>
        new(false, reasonCode, null, null);

    private static string DescribeIneligibility(string reasonCode) =>
        reasonCode switch
        {
            PlanningEntryTransitionReasonCodes.MissingConsumedHandover =>
                "Import a qualifying Project Architect handover on the Exchange tab before entering governed planning.",
            PlanningEntryTransitionReasonCodes.PackageWrongProject =>
                "The imported PA handover belongs to a different project.",
            PlanningEntryTransitionReasonCodes.RelayStopActive =>
                "STOP is active on the imported PA handover; planning entry is not permitted.",
            PlanningEntryTransitionReasonCodes.MultipleApplicableIntakeInstances =>
                "More than one active GEW instance is at Intake; planning entry cannot proceed automatically.",
            PlanningEntryTransitionReasonCodes.NoApplicableIntakeInstance =>
                "No active GEW instance is at Intake for this project.",
            PlanningEntryTransitionReasonCodes.WrongWorkflowPlace =>
                "The active workflow instance is not at Intake.",
            PlanningEntryTransitionReasonCodes.EvaluatedBlocker =>
                "Evaluated workflow constraints block the Intake → governed planning transition.",
            PlanningEntryTransitionReasonCodes.ContractNotSatisfied =>
                "The imported PA handover does not satisfy the planning-entry contract (planning authorized; implementation not authorized; no durable DWA projection).",
            PlanningEntryTransitionReasonCodes.PackageNotValid =>
                "The imported PA handover is not valid for governed use.",
            _ => "Entering governed planning is not available with the current evidence and workflow state.",
        };
}
