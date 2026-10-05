using Edf.Domain.Projects;

namespace Edf.Domain.Workflow;

public sealed record WorkflowInstanceStopSummary(
    WorkflowInstanceId WorkflowInstanceId,
    ProjectConcordProjectId ProjectId,
    bool IsStopActive,
    WorkflowInstanceStopEventId LastEventId,
    long ResourceVersion,
    DateTimeOffset UpdatedUtc);
