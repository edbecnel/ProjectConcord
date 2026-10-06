namespace Edf.Application.Operator.PlanningEntry;

using Edf.Domain.Projects;

public interface IPlanningEntryRelayReadModel
{
    PlanningEntryRelayReadModelSnapshot Resolve(ProjectConcordProjectId projectId);
}
