using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

public sealed record WorkflowOrigin(
    WorkflowOriginId OriginId,
    ProjectConcordProjectId ProjectId,
    WorkflowInstanceId SourceWorkflowInstanceId,
    WorkflowInstanceId DerivedWorkflowInstanceId,
    WorkflowOriginKind OriginKind,
    GovernedCorrelationId CreationCorrelationId,
    GovernedPackageId? CreationPackageId,
    string? CreationAuthorityReference,
    DateTimeOffset CreatedUtc);
