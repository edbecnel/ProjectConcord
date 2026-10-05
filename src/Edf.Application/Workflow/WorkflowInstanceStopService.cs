using Edf.Application.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class WorkflowInstanceStopService : IWorkflowInstanceStopService
{
    private readonly IWorkflowInstanceStore _instances;
    private readonly IWorkflowInstanceStopStore _stops;

    public WorkflowInstanceStopService(
        IWorkflowInstanceStore instances,
        IWorkflowInstanceStopStore stops)
    {
        _instances = instances ?? throw new ArgumentNullException(nameof(instances));
        _stops = stops ?? throw new ArgumentNullException(nameof(stops));
    }

    public WorkflowInstanceStopSummary RecordGovernedStopSet(
        WorkflowInstanceId workflowInstanceId,
        long? expectedStopResourceVersion,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        var instance = LoadInstance(workflowInstanceId);

        var summary = _stops.GetSummary(workflowInstanceId);
        if (summary is null)
        {
            return InsertInitialSet(instance, authority);
        }

        if (summary.IsStopActive)
        {
            throw new WorkflowInstanceStopOperationException("STOP is already active for this workflow instance.");
        }

        if (expectedStopResourceVersion is null || summary.ResourceVersion != expectedStopResourceVersion)
        {
            throw new WorkflowInstanceStopConcurrencyException(workflowInstanceId, expectedStopResourceVersion ?? summary.ResourceVersion);
        }

        return ApplyMutation(summary, WorkflowInstanceStopEventKind.Set, true, authority);
    }

    public WorkflowInstanceStopSummary RecordGovernedStopClear(
        WorkflowInstanceId workflowInstanceId,
        long expectedStopResourceVersion,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        _ = LoadInstance(workflowInstanceId);

        var summary = _stops.GetSummary(workflowInstanceId);
        if (summary is null || !summary.IsStopActive)
        {
            throw new WorkflowInstanceStopOperationException("STOP is not active for this workflow instance.");
        }

        if (summary.ResourceVersion != expectedStopResourceVersion)
        {
            throw new WorkflowInstanceStopConcurrencyException(workflowInstanceId, expectedStopResourceVersion);
        }

        return ApplyMutation(summary, WorkflowInstanceStopEventKind.Clear, false, authority);
    }

    private WorkflowInstanceStopSummary InsertInitialSet(WorkflowInstance instance, GovernedWorkflowMutationAuthority authority)
    {
        var now = DateTimeOffset.UtcNow;
        var eventId = WorkflowInstanceStopEventId.New();
        var stopEvent = new WorkflowInstanceStopEvent(
            eventId,
            instance.InstanceId,
            instance.ProjectId,
            WorkflowInstanceStopEventKind.Set,
            authority.CorrelationId,
            authority.OriginatingPackageId,
            authority.AuthorityReference,
            now);

        var summary = new WorkflowInstanceStopSummary(
            instance.InstanceId,
            instance.ProjectId,
            true,
            eventId,
            1,
            now);

        _stops.InsertEvent(stopEvent);
        _stops.InsertSummary(summary);
        return summary;
    }

    private WorkflowInstanceStopSummary ApplyMutation(
        WorkflowInstanceStopSummary summary,
        WorkflowInstanceStopEventKind eventKind,
        bool isStopActive,
        GovernedWorkflowMutationAuthority authority)
    {
        var now = DateTimeOffset.UtcNow;
        var eventId = WorkflowInstanceStopEventId.New();
        var stopEvent = new WorkflowInstanceStopEvent(
            eventId,
            summary.WorkflowInstanceId,
            summary.ProjectId,
            eventKind,
            authority.CorrelationId,
            authority.OriginatingPackageId,
            authority.AuthorityReference,
            now);

        var updated = summary with
        {
            IsStopActive = isStopActive,
            LastEventId = eventId,
            ResourceVersion = summary.ResourceVersion + 1,
            UpdatedUtc = now,
        };

        _stops.InsertEvent(stopEvent);
        if (!_stops.TryUpdateSummaryWithExpectedVersion(updated, summary.ResourceVersion))
        {
            throw new WorkflowInstanceStopConcurrencyException(summary.WorkflowInstanceId, summary.ResourceVersion);
        }

        return updated;
    }

    private WorkflowInstance LoadInstance(WorkflowInstanceId instanceId)
    {
        var instance = _instances.GetById(instanceId);
        if (instance is null)
        {
            throw new WorkflowInstanceStopOperationException($"Workflow instance '{instanceId}' was not found.");
        }

        return instance;
    }

    private static void ValidateAuthority(GovernedWorkflowMutationAuthority authority)
    {
        ArgumentNullException.ThrowIfNull(authority);
        if (!authority.IsWellFormed())
        {
            throw new WorkflowInstanceStopOperationException("Governed mutation authority is missing or invalid.");
        }
    }
}
