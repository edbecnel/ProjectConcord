using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed record PrescribedWorkflowDefinition(
    PrescribedWorkflowId WorkflowId,
    WorkflowDefinitionVersion DefinitionVersion,
    IReadOnlyList<TopologyPlaceId> Places,
    IReadOnlyList<(TopologyPlaceId From, TopologyPlaceId To)> Transitions,
    IReadOnlyList<WorkflowProfileId> KnownProfiles);
