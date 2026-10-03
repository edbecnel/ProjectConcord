using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public interface IPrescribedWorkflowRegistry
{
    bool TryGetDefinition(
        PrescribedWorkflowId workflowId,
        WorkflowDefinitionVersion definitionVersion,
        out PrescribedWorkflowDefinition? definition);

    bool IsKnownPlace(PrescribedWorkflowDefinition definition, TopologyPlaceId placeId);

    bool IsKnownProfile(PrescribedWorkflowDefinition definition, WorkflowProfileId profileId);

    bool IsPermittedTransition(
        PrescribedWorkflowDefinition definition,
        TopologyPlaceId fromPlace,
        TopologyPlaceId toPlace);
}
