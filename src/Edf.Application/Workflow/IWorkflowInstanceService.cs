using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public interface IWorkflowInstanceService
{
    WorkflowInstance CreateGewInstance(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId creationCorrelationId,
        string? projectRootAbsolutePath,
        WorkflowProfileId? profileId = null);

    WorkflowInstance RecordGovernedTopologyTransition(
        WorkflowInstanceId instanceId,
        TopologyPlaceId targetPlaceId,
        long expectedResourceVersion,
        GovernedWorkflowTransitionAuthority authority);

    WorkflowInstance RecordGovernedLifecycleCompletion(
        WorkflowInstanceId instanceId,
        long expectedResourceVersion,
        GovernedWorkflowTransitionAuthority authority);
}
