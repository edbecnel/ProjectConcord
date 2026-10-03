using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

public sealed record WorkflowInstance(
    WorkflowInstanceId InstanceId,
    ProjectConcordProjectId ProjectId,
    PrescribedWorkflowId WorkflowId,
    WorkflowDefinitionVersion DefinitionVersion,
    WorkflowProfileId? ProfileId,
    WorkflowInstanceLifecycle Lifecycle,
    TopologyPlaceId TopologyPlaceId,
    TraversalOccurrenceId TraversalOccurrenceId,
    GovernedBaselineReference GovernedBaseline,
    GovernedCorrelationId CreationCorrelationId,
    GovernedCorrelationId? LastGovernedTransitionCorrelationId,
    GovernedPackageId? LastGovernedTransitionPackageId,
    string? LastGovernedTransitionAuthorityReference,
    long ResourceVersion,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc);
