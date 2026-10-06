namespace Edf.Application.Workflow.PlanningEntry;

using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public interface IGewV1IntakePlanningEntryTransitionService
{
    IntakePlanningEntryEligibility EvaluateEligibility(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage? consumedPaHandover,
        RelayValidationResult? consumedValidation);

    IntakePlanningEntryTransitionResult TryEnterGovernedPlanning(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover,
        RelayValidationResult consumedValidation);
}
