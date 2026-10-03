using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed record GovernedWorkStateRecoverySnapshot(
    ProjectConcordProjectId ProjectId,
    IReadOnlyList<WorkflowInstance> ActiveInstances,
    IReadOnlyList<WorkflowOrigin> Origins,
    IReadOnlyList<WorkflowDependency> Dependencies);
