using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

public sealed record WorkflowInstanceStopEvent(
    WorkflowInstanceStopEventId EventId,
    WorkflowInstanceId WorkflowInstanceId,
    ProjectConcordProjectId ProjectId,
    WorkflowInstanceStopEventKind EventKind,
    GovernedCorrelationId CorrelationId,
    GovernedPackageId? PackageId,
    string? AuthorityReference,
    DateTimeOffset OccurredUtc);
