using Edf.Application.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class WorkflowRelationshipService : IWorkflowRelationshipService
{
    private readonly IWorkflowInstanceStore _instances;
    private readonly IWorkflowOriginStore _origins;
    private readonly IWorkflowDependencyStore _dependencies;

    public WorkflowRelationshipService(
        IWorkflowInstanceStore instances,
        IWorkflowOriginStore origins,
        IWorkflowDependencyStore dependencies)
    {
        _instances = instances ?? throw new ArgumentNullException(nameof(instances));
        _origins = origins ?? throw new ArgumentNullException(nameof(origins));
        _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
    }

    public WorkflowOrigin RecordGovernedWorkflowOrigin(
        WorkflowInstanceId sourceInstanceId,
        WorkflowInstanceId derivedInstanceId,
        WorkflowOriginKind originKind,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        var (source, derived) = LoadEndpoints(sourceInstanceId, derivedInstanceId);
        EnsureSameProject(source, derived);

        var origin = new WorkflowOrigin(
            WorkflowOriginId.New(),
            source.ProjectId,
            source.InstanceId,
            derived.InstanceId,
            originKind,
            authority.CorrelationId,
            authority.OriginatingPackageId,
            authority.AuthorityReference,
            DateTimeOffset.UtcNow);

        _origins.Insert(origin);
        return origin;
    }

    public WorkflowDependency RecordGovernedBlockingDependency(
        WorkflowInstanceId blockedInstanceId,
        WorkflowInstanceId requiredInstanceId,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);

        if (blockedInstanceId == requiredInstanceId)
        {
            throw new WorkflowRelationshipOperationException("A workflow instance cannot depend on itself.");
        }

        var (blocked, required) = LoadEndpoints(blockedInstanceId, requiredInstanceId);
        EnsureSameProject(blocked, required);

        var pending = _dependencies.ListPendingForProject(blocked.ProjectId);
        if (pending.Any(d =>
                d.BlockedWorkflowInstanceId == blockedInstanceId
                && d.RequiredWorkflowInstanceId == requiredInstanceId))
        {
            throw new WorkflowRelationshipOperationException(
                "A pending blocking dependency already exists for this blocked/required pair.");
        }

        if (WorkflowBlockingDependencyCycleChecker.WouldCreateCycle(
                blockedInstanceId,
                requiredInstanceId,
                pending))
        {
            throw new WorkflowRelationshipOperationException(
                "Blocking dependency would create an unsatisfiable cycle.");
        }

        var now = DateTimeOffset.UtcNow;
        var dependency = new WorkflowDependency(
            WorkflowDependencyId.New(),
            blocked.ProjectId,
            blocked.InstanceId,
            required.InstanceId,
            WorkflowDependencyKind.Blocking,
            WorkflowDependencySatisfactionCondition.RequiredInstanceLifecycleCompleted,
            WorkflowDependencyStatus.Pending,
            authority.CorrelationId,
            authority.OriginatingPackageId,
            authority.AuthorityReference,
            null,
            null,
            null,
            null,
            null,
            null,
            1,
            now,
            now,
            null,
            null);

        _dependencies.Insert(dependency);
        return dependency;
    }

    public WorkflowDependency RecordGovernedDependencySatisfaction(
        WorkflowDependencyId dependencyId,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        var current = LoadPendingDependency(dependencyId);

        if (current.ResourceVersion != expectedResourceVersion)
        {
            throw new WorkflowDependencyConcurrencyException(dependencyId, expectedResourceVersion);
        }

        var required = _instances.GetById(current.RequiredWorkflowInstanceId);
        if (required is null)
        {
            throw new WorkflowRelationshipOperationException("Required workflow instance was not found.");
        }

        if (required.Lifecycle != WorkflowInstanceLifecycle.Completed)
        {
            throw new WorkflowRelationshipOperationException(
                "Dependency satisfaction requires the required workflow instance to be Completed.");
        }

        return ApplyStatusMutation(
            current,
            expectedResourceVersion,
            authority,
            WorkflowDependencyStatus.Satisfied,
            satisfiedUtc: DateTimeOffset.UtcNow,
            releaseUtc: null);
    }

    public WorkflowDependency RecordGovernedDependencyRelease(
        WorkflowDependencyId dependencyId,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        var current = LoadPendingDependency(dependencyId);

        if (current.ResourceVersion != expectedResourceVersion)
        {
            throw new WorkflowDependencyConcurrencyException(dependencyId, expectedResourceVersion);
        }

        return ApplyStatusMutation(
            current,
            expectedResourceVersion,
            authority,
            WorkflowDependencyStatus.Released,
            satisfiedUtc: null,
            releaseUtc: DateTimeOffset.UtcNow);
    }

    private WorkflowDependency ApplyStatusMutation(
        WorkflowDependency current,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority,
        WorkflowDependencyStatus newStatus,
        DateTimeOffset? satisfiedUtc,
        DateTimeOffset? releaseUtc)
    {
        var now = DateTimeOffset.UtcNow;
        var updated = current with
        {
            Status = newStatus,
            SatisfactionCorrelationId = newStatus == WorkflowDependencyStatus.Satisfied
                ? authority.CorrelationId
                : current.SatisfactionCorrelationId,
            SatisfactionPackageId = newStatus == WorkflowDependencyStatus.Satisfied
                ? authority.OriginatingPackageId
                : current.SatisfactionPackageId,
            SatisfactionAuthorityReference = newStatus == WorkflowDependencyStatus.Satisfied
                ? authority.AuthorityReference
                : current.SatisfactionAuthorityReference,
            ReleaseCorrelationId = newStatus == WorkflowDependencyStatus.Released
                ? authority.CorrelationId
                : current.ReleaseCorrelationId,
            ReleasePackageId = newStatus == WorkflowDependencyStatus.Released
                ? authority.OriginatingPackageId
                : current.ReleasePackageId,
            ReleaseAuthorityReference = newStatus == WorkflowDependencyStatus.Released
                ? authority.AuthorityReference
                : current.ReleaseAuthorityReference,
            SatisfiedUtc = satisfiedUtc ?? current.SatisfiedUtc,
            ReleasedUtc = releaseUtc ?? current.ReleasedUtc,
            ResourceVersion = current.ResourceVersion + 1,
            UpdatedUtc = now,
        };

        if (!_dependencies.TryUpdateWithExpectedVersion(updated, expectedResourceVersion))
        {
            throw new WorkflowDependencyConcurrencyException(current.DependencyId, expectedResourceVersion);
        }

        return updated;
    }

    private WorkflowDependency LoadPendingDependency(WorkflowDependencyId dependencyId)
    {
        var current = _dependencies.GetById(dependencyId);
        if (current is null)
        {
            throw new WorkflowRelationshipOperationException($"Workflow dependency '{dependencyId}' was not found.");
        }

        if (current.Status != WorkflowDependencyStatus.Pending)
        {
            throw new WorkflowRelationshipOperationException(
                $"Workflow dependency '{dependencyId}' is not pending (status={current.Status}).");
        }

        return current;
    }

    private (WorkflowInstance First, WorkflowInstance Second) LoadEndpoints(
        WorkflowInstanceId firstId,
        WorkflowInstanceId secondId)
    {
        var first = _instances.GetById(firstId);
        if (first is null)
        {
            throw new WorkflowRelationshipOperationException($"Workflow instance '{firstId}' was not found.");
        }

        var second = _instances.GetById(secondId);
        if (second is null)
        {
            throw new WorkflowRelationshipOperationException($"Workflow instance '{secondId}' was not found.");
        }

        return (first, second);
    }

    private static void EnsureSameProject(WorkflowInstance a, WorkflowInstance b)
    {
        if (a.ProjectId != b.ProjectId)
        {
            throw new WorkflowRelationshipOperationException(
                "Workflow relationship endpoints must belong to the same project.");
        }
    }

    private static void ValidateAuthority(GovernedWorkflowMutationAuthority authority)
    {
        ArgumentNullException.ThrowIfNull(authority);
        if (!authority.IsWellFormed())
        {
            throw new WorkflowRelationshipOperationException("Governed mutation authority is missing or invalid.");
        }
    }
}
