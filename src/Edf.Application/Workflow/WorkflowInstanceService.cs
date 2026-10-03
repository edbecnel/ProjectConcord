using Edf.Application.Projects;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Workflow;

public sealed class WorkflowInstanceService : IWorkflowInstanceService
{
    private readonly IWorkflowInstanceStore _store;
    private readonly IPrescribedWorkflowRegistry _registry;
    private readonly IGitHeadCommitResolver _gitHeadCommitResolver;

    public WorkflowInstanceService(
        IWorkflowInstanceStore store,
        IPrescribedWorkflowRegistry registry,
        IGitHeadCommitResolver gitHeadCommitResolver)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _gitHeadCommitResolver = gitHeadCommitResolver ?? throw new ArgumentNullException(nameof(gitHeadCommitResolver));
    }

    public WorkflowInstance CreateGewInstance(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId creationCorrelationId,
        string? projectRootAbsolutePath,
        WorkflowProfileId? profileId = null)
    {
        if (creationCorrelationId.IsEmpty)
        {
            throw new WorkflowInstanceOperationException("Creation correlation id is required.");
        }

        if (!_registry.TryGetDefinition(
                PrescribedWorkflowId.GovernedEngineering,
                WorkflowDefinitionVersion.GewV1,
                out var definition)
            || definition is null)
        {
            throw new WorkflowInstanceOperationException("GEW v1 workflow definition is not registered.");
        }

        if (profileId is { } supplied && !supplied.IsEmpty
            && !_registry.IsKnownProfile(definition, supplied))
        {
            throw new WorkflowInstanceOperationException($"Unknown workflow profile '{supplied.Value}'.");
        }

        var baseline = ResolveBaseline(projectRootAbsolutePath);
        var now = DateTimeOffset.UtcNow;
        var instance = new WorkflowInstance(
            WorkflowInstanceId.New(),
            projectId,
            PrescribedWorkflowId.GovernedEngineering,
            WorkflowDefinitionVersion.GewV1,
            profileId is { IsEmpty: false } ? profileId : null,
            WorkflowInstanceLifecycle.Active,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.Intake),
            TraversalOccurrenceId.New(),
            baseline,
            creationCorrelationId,
            null,
            null,
            null,
            1,
            now,
            now);

        _store.Insert(instance);
        return instance;
    }

    public WorkflowInstance RecordGovernedTopologyTransition(
        WorkflowInstanceId instanceId,
        TopologyPlaceId targetPlaceId,
        long expectedResourceVersion,
        GovernedWorkflowTransitionAuthority authority)
    {
        ValidateAuthority(authority);

        var current = LoadActiveInstance(instanceId);
        var definition = GetDefinitionForInstance(current);

        if (!_registry.IsKnownPlace(definition, targetPlaceId))
        {
            throw new WorkflowInstanceOperationException($"Unknown topology place '{targetPlaceId.Value}'.");
        }

        if (!_registry.IsPermittedTransition(definition, current.TopologyPlaceId, targetPlaceId))
        {
            throw new WorkflowInstanceOperationException(
                $"Topology transition from '{current.TopologyPlaceId.Value}' to '{targetPlaceId.Value}' is not permitted.");
        }

        var updated = ApplyGovernedMutation(
            current,
            expectedResourceVersion,
            authority,
            topologyPlaceId: targetPlaceId,
            newTraversalOccurrence: TraversalOccurrenceId.New(),
            lifecycle: WorkflowInstanceLifecycle.Active);

        return updated;
    }

    public WorkflowInstance RecordGovernedLifecycleCompletion(
        WorkflowInstanceId instanceId,
        long expectedResourceVersion,
        GovernedWorkflowTransitionAuthority authority)
    {
        ValidateAuthority(authority);

        var current = LoadActiveInstance(instanceId);
        _ = GetDefinitionForInstance(current);

        if (current.TopologyPlaceId.Value != GewV1TopologyPlaces.PostSubmissionGoverned)
        {
            throw new WorkflowInstanceOperationException(
                "Lifecycle completion is only permitted from post-submission-governed.");
        }

        var updated = ApplyGovernedMutation(
            current,
            expectedResourceVersion,
            authority,
            topologyPlaceId: current.TopologyPlaceId,
            newTraversalOccurrence: current.TraversalOccurrenceId,
            lifecycle: WorkflowInstanceLifecycle.Completed);

        return updated;
    }

    private WorkflowInstance LoadActiveInstance(WorkflowInstanceId instanceId)
    {
        var current = _store.GetById(instanceId);
        if (current is null)
        {
            throw new WorkflowInstanceOperationException($"Workflow instance '{instanceId}' was not found.");
        }

        if (current.Lifecycle != WorkflowInstanceLifecycle.Active)
        {
            throw new WorkflowInstanceOperationException(
                $"Workflow instance '{instanceId}' is not active (lifecycle={current.Lifecycle}).");
        }

        return current;
    }

    private PrescribedWorkflowDefinition GetDefinitionForInstance(WorkflowInstance instance)
    {
        if (!_registry.TryGetDefinition(instance.WorkflowId, instance.DefinitionVersion, out var definition)
            || definition is null)
        {
            throw new WorkflowInstanceOperationException(
                $"Unknown workflow definition {instance.WorkflowId.Value} v{instance.DefinitionVersion.Value}.");
        }

        if (!_registry.IsKnownPlace(definition, instance.TopologyPlaceId))
        {
            throw new WorkflowInstanceOperationException(
                $"Stored topology place '{instance.TopologyPlaceId.Value}' is not valid for the workflow definition.");
        }

        return definition;
    }

    private static void ValidateAuthority(GovernedWorkflowTransitionAuthority authority)
    {
        ArgumentNullException.ThrowIfNull(authority);
        if (!authority.IsWellFormed())
        {
            throw new WorkflowInstanceOperationException("Governed transition authority is missing or invalid.");
        }
    }

    private WorkflowInstance ApplyGovernedMutation(
        WorkflowInstance current,
        long expectedResourceVersion,
        GovernedWorkflowTransitionAuthority authority,
        TopologyPlaceId topologyPlaceId,
        TraversalOccurrenceId newTraversalOccurrence,
        WorkflowInstanceLifecycle lifecycle)
    {
        if (current.ResourceVersion != expectedResourceVersion)
        {
            throw new WorkflowInstanceConcurrencyException(current.InstanceId, expectedResourceVersion);
        }

        var now = DateTimeOffset.UtcNow;
        var updated = current with
        {
            Lifecycle = lifecycle,
            TopologyPlaceId = topologyPlaceId,
            TraversalOccurrenceId = newTraversalOccurrence,
            LastGovernedTransitionCorrelationId = authority.CorrelationId,
            LastGovernedTransitionPackageId = authority.OriginatingPackageId,
            LastGovernedTransitionAuthorityReference = authority.AuthorityReference,
            ResourceVersion = current.ResourceVersion + 1,
            UpdatedUtc = now,
        };

        if (!_store.TryUpdateWithExpectedVersion(updated, expectedResourceVersion))
        {
            throw new WorkflowInstanceConcurrencyException(current.InstanceId, expectedResourceVersion);
        }

        return updated;
    }

    private GovernedBaselineReference ResolveBaseline(string? projectRootAbsolutePath)
    {
        if (string.IsNullOrWhiteSpace(projectRootAbsolutePath))
        {
            return GovernedBaselineReference.Unspecified;
        }

        var head = _gitHeadCommitResolver.TryResolveHeadCommit(projectRootAbsolutePath);
        return head is null ? GovernedBaselineReference.Unspecified : GovernedBaselineReference.FromGitCommit(head);
    }
}
