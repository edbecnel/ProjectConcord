using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

public sealed record WorkflowDependency(
    WorkflowDependencyId DependencyId,
    ProjectConcordProjectId ProjectId,
    WorkflowInstanceId BlockedWorkflowInstanceId,
    WorkflowInstanceId RequiredWorkflowInstanceId,
    WorkflowDependencyKind DependencyKind,
    WorkflowDependencySatisfactionCondition SatisfactionCondition,
    WorkflowDependencyStatus Status,
    GovernedCorrelationId CreationCorrelationId,
    GovernedPackageId? CreationPackageId,
    string? CreationAuthorityReference,
    GovernedCorrelationId? SatisfactionCorrelationId,
    GovernedPackageId? SatisfactionPackageId,
    string? SatisfactionAuthorityReference,
    GovernedCorrelationId? ReleaseCorrelationId,
    GovernedPackageId? ReleasePackageId,
    string? ReleaseAuthorityReference,
    long ResourceVersion,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc,
    DateTimeOffset? SatisfiedUtc,
    DateTimeOffset? ReleasedUtc);
