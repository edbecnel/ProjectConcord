using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

public sealed record DevelopmentWorkAuthorization(
    DevelopmentWorkAuthorizationId AuthorizationId,
    ProjectConcordProjectId ProjectId,
    WorkflowInstanceId WorkflowInstanceId,
    PrescribedWorkflowId PrescribedWorkflowId,
    WorkflowDefinitionVersion DefinitionVersion,
    WorkflowProfileId? ProfileId,
    TopologyPlaceId GrantTopologyPlaceId,
    TraversalOccurrenceId GrantTraversalOccurrenceId,
    DevelopmentWorkAuthorizationKind AuthorizationKind,
    string NormalizedTrancheKey,
    string? AuthorizedTrancheId,
    string? AuthorizedScopeMarkersJson,
    string? AuthorityReference,
    DevelopmentWorkAuthorizationDisposition Disposition,
    GovernedCorrelationId GrantCorrelationId,
    GovernedPackageId? GrantPackageId,
    string? GrantAuthorityReference,
    DateTimeOffset GrantedUtc,
    GovernedCorrelationId? SupersessionCorrelationId,
    GovernedPackageId? SupersessionPackageId,
    string? SupersessionAuthorityReference,
    DateTimeOffset? SupersededUtc,
    long ResourceVersion,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc);
