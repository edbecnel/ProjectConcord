using Edf.Domain.Workflow;

namespace Edf.Application.Operator.WorkState;

public sealed record GovernedWorkStateCurrentWorkItem(
    WorkflowInstanceId InstanceId,
    PrescribedWorkflowId WorkflowId,
    WorkflowDefinitionVersion DefinitionVersion,
    WorkflowProfileId? ProfileId,
    TopologyPlaceId TopologyPlaceId,
    TraversalOccurrenceId TraversalOccurrenceId,
    GovernedBaselineReference StoredGovernedBaseline,
    long ResourceVersion,
    bool? HeadDriftsFromStoredBaseline);
