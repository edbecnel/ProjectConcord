namespace Edf.Application.Operator.PlanningRegion;

using Edf.Domain.Projects;

public interface IPlanningRegionRelayReadModel
{
    PlanningRegionRelayReadModelSnapshot Resolve(ProjectConcordProjectId projectId);
}
