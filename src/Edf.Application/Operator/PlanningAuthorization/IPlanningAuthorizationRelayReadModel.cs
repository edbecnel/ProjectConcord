namespace Edf.Application.Operator.PlanningAuthorization;

using Edf.Domain.Projects;

public interface IPlanningAuthorizationRelayReadModel
{
    PlanningAuthorizationRelayReadModelSnapshot Resolve(ProjectConcordProjectId projectId);
}
