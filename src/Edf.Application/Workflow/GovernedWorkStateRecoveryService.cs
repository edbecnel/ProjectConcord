using Edf.Application.Projects;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class GovernedWorkStateRecoveryService : IGovernedWorkStateRecoveryService
{
    private readonly IWorkflowInstanceStore _store;
    private readonly IWorkflowOriginStore _origins;
    private readonly IWorkflowDependencyStore _dependencies;
    private readonly IDevelopmentWorkAuthorizationStore _authorizations;
    private readonly IWorkflowInstanceStopStore _stops;
    private readonly IPrescribedWorkflowRegistry _registry;

    public GovernedWorkStateRecoveryService(
        IWorkflowInstanceStore store,
        IWorkflowOriginStore origins,
        IWorkflowDependencyStore dependencies,
        IDevelopmentWorkAuthorizationStore authorizations,
        IWorkflowInstanceStopStore stops,
        IPrescribedWorkflowRegistry registry)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _origins = origins ?? throw new ArgumentNullException(nameof(origins));
        _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
        _authorizations = authorizations ?? throw new ArgumentNullException(nameof(authorizations));
        _stops = stops ?? throw new ArgumentNullException(nameof(stops));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public GovernedWorkStateRecoverySnapshot RecoverForProject(ProjectConcordProjectId projectId)
    {
        var active = _store.ListByProject(projectId, WorkflowInstanceLifecycle.Active);
        foreach (var instance in active)
        {
            ValidateRecoveredInstance(instance);
        }

        var origins = _origins.ListByProject(projectId);
        var dependencies = _dependencies.ListByProject(projectId);
        var authorizations = _authorizations.ListByProject(projectId);
        var stopSummaries = _stops.ListSummariesByProject(projectId);

        ValidateRelationships(projectId, origins, dependencies);
        ValidateAuthorizations(projectId, authorizations);
        ValidateStopSummaries(projectId, stopSummaries);

        return new GovernedWorkStateRecoverySnapshot(
            projectId,
            active,
            origins,
            dependencies,
            authorizations,
            stopSummaries);
    }

    private void ValidateRelationships(
        ProjectConcordProjectId projectId,
        IReadOnlyList<WorkflowOrigin> origins,
        IReadOnlyList<WorkflowDependency> dependencies)
    {
        foreach (var origin in origins)
        {
            if (origin.ProjectId != projectId)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Workflow origin '{origin.OriginId}' has mismatched project id.");
            }

            EnsureInstanceInProject(origin.SourceWorkflowInstanceId, projectId);
            EnsureInstanceInProject(origin.DerivedWorkflowInstanceId, projectId);
        }

        foreach (var dependency in dependencies)
        {
            if (dependency.ProjectId != projectId)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Workflow dependency '{dependency.DependencyId}' has mismatched project id.");
            }

            EnsureInstanceInProject(dependency.BlockedWorkflowInstanceId, projectId);
            EnsureInstanceInProject(dependency.RequiredWorkflowInstanceId, projectId);
        }
    }

    private void ValidateAuthorizations(
        ProjectConcordProjectId projectId,
        IReadOnlyList<DevelopmentWorkAuthorization> authorizations)
    {
        foreach (var authorization in authorizations)
        {
            if (authorization.ProjectId != projectId)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Development work authorization '{authorization.AuthorizationId}' has mismatched project id.");
            }

            EnsureInstanceInProject(authorization.WorkflowInstanceId, projectId);
        }
    }

    private void ValidateStopSummaries(
        ProjectConcordProjectId projectId,
        IReadOnlyList<WorkflowInstanceStopSummary> stopSummaries)
    {
        foreach (var summary in stopSummaries)
        {
            if (summary.ProjectId != projectId)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Workflow instance stop summary for '{summary.WorkflowInstanceId}' has mismatched project id.");
            }

            EnsureInstanceInProject(summary.WorkflowInstanceId, projectId);

            var events = _stops.ListEventsByWorkflowInstance(summary.WorkflowInstanceId);
            if (events.Count == 0)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Workflow instance stop summary for '{summary.WorkflowInstanceId}' has no events.");
            }

            var lastEvent = events.Last();
            if (lastEvent.EventId != summary.LastEventId)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Workflow instance stop summary for '{summary.WorkflowInstanceId}' does not reference the latest event.");
            }

            var expectedActive = lastEvent.EventKind == WorkflowInstanceStopEventKind.Set;
            if (summary.IsStopActive != expectedActive)
            {
                throw new GovernedWorkStateRecoveryException(
                    $"Workflow instance stop summary for '{summary.WorkflowInstanceId}' is inconsistent with the latest event.");
            }
        }
    }

    private void EnsureInstanceInProject(WorkflowInstanceId instanceId, ProjectConcordProjectId projectId)
    {
        var instance = _store.GetById(instanceId);
        if (instance is null || instance.ProjectId != projectId)
        {
            throw new GovernedWorkStateRecoveryException(
                $"Workflow instance '{instanceId}' is missing or not in project '{projectId}'.");
        }
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
