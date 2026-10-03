using Edf.Domain.Projects;

namespace Edf.Application.Workflow;

public interface IGovernedWorkStateRecoveryService
{
    GovernedWorkStateRecoverySnapshot RecoverForProject(ProjectConcordProjectId projectId);
}
