using Edf.Application.Projects;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class GovernedWorkStateRecoveryService : IGovernedWorkStateRecoveryService
{
    private readonly IWorkflowInstanceStore _store;
    private readonly IPrescribedWorkflowRegistry _registry;

    public GovernedWorkStateRecoveryService(
        IWorkflowInstanceStore store,
        IPrescribedWorkflowRegistry registry)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public GovernedWorkStateRecoverySnapshot RecoverForProject(ProjectConcordProjectId projectId)
    {
        var active = _store.ListByProject(projectId, WorkflowInstanceLifecycle.Active);
        foreach (var instance in active)
        {
            ValidateRecoveredInstance(instance);
        }

        return new GovernedWorkStateRecoverySnapshot(projectId, active);
    }

    private void ValidateRecoveredInstance(WorkflowInstance instance)
    {
        if (!_registry.TryGetDefinition(instance.WorkflowId, instance.DefinitionVersion, out var definition)
            || definition is null)
        {
            throw new GovernedWorkStateRecoveryException(
                $"Workflow instance '{instance.InstanceId}' references unknown definition {instance.WorkflowId.Value} v{instance.DefinitionVersion.Value}.");
        }

        if (!_registry.IsKnownPlace(definition, instance.TopologyPlaceId))
        {
            throw new GovernedWorkStateRecoveryException(
                $"Workflow instance '{instance.InstanceId}' has unknown topology place '{instance.TopologyPlaceId.Value}'.");
        }

        if (instance.ProfileId is { } profile && !profile.IsEmpty
            && !_registry.IsKnownProfile(definition, profile))
        {
            throw new GovernedWorkStateRecoveryException(
                $"Workflow instance '{instance.InstanceId}' has unknown profile '{profile.Value}'.");
        }
    }
}
