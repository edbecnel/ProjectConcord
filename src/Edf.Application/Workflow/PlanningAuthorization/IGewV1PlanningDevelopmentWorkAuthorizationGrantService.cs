namespace Edf.Application.Workflow.PlanningAuthorization;

using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public interface IGewV1PlanningDevelopmentWorkAuthorizationGrantService
{
    PlanningAuthorizationGrantEligibility EvaluateGrantEligibility(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover,
        RelayValidationResult validation);

    PlanningAuthorizationGrantResult TryRecordPlanningAuthorizationGrant(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage consumedPaHandover,
        RelayValidationResult validation);
}
