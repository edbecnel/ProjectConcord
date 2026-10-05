using Edf.Domain.Projects;
using Edf.Domain.Workflow;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryDevelopmentWorkAuthorizationStore : IDevelopmentWorkAuthorizationStore
{
    private readonly Dictionary<DevelopmentWorkAuthorizationId, DevelopmentWorkAuthorization> _authorizations = new();

    public void Insert(DevelopmentWorkAuthorization authorization)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        if (_authorizations.ContainsKey(authorization.AuthorizationId))
        {
            throw new InvalidOperationException(
                $"Development work authorization '{authorization.AuthorizationId}' already exists.");
        }

        if (authorization.Disposition == DevelopmentWorkAuthorizationDisposition.Active
            && TryGetActiveByContext(
                authorization.WorkflowInstanceId,
                authorization.AuthorizationKind,
                authorization.NormalizedTrancheKey) is not null)
        {
            throw new InvalidOperationException(
                "An active authorization already exists for this workflow instance, kind, and tranche context.");
        }

        _authorizations[authorization.AuthorizationId] = authorization;
    }

    public DevelopmentWorkAuthorization? GetById(DevelopmentWorkAuthorizationId authorizationId) =>
        _authorizations.TryGetValue(authorizationId, out var authorization) ? authorization : null;

    public IReadOnlyList<DevelopmentWorkAuthorization> ListByProject(ProjectConcordProjectId projectId) =>
        _authorizations.Values.Where(a => a.ProjectId == projectId).OrderBy(a => a.CreatedUtc).ToList();

    public IReadOnlyList<DevelopmentWorkAuthorization> ListByWorkflowInstance(WorkflowInstanceId workflowInstanceId) =>
        _authorizations.Values
            .Where(a => a.WorkflowInstanceId == workflowInstanceId)
            .OrderBy(a => a.CreatedUtc)
            .ToList();

    public DevelopmentWorkAuthorization? TryGetActiveByContext(
        WorkflowInstanceId workflowInstanceId,
        DevelopmentWorkAuthorizationKind authorizationKind,
        string normalizedTrancheKey) =>
        _authorizations.Values.FirstOrDefault(a =>
            a.WorkflowInstanceId == workflowInstanceId
            && a.AuthorizationKind == authorizationKind
            && a.NormalizedTrancheKey == normalizedTrancheKey
            && a.Disposition == DevelopmentWorkAuthorizationDisposition.Active);

    public bool TryUpdateWithExpectedVersion(DevelopmentWorkAuthorization authorization, long expectedResourceVersion)
    {
        if (!_authorizations.TryGetValue(authorization.AuthorizationId, out var existing))
        {
            return false;
        }

        if (existing.ResourceVersion != expectedResourceVersion)
        {
            return false;
        }

        if (authorization.Disposition == DevelopmentWorkAuthorizationDisposition.Active
            && existing.Disposition != DevelopmentWorkAuthorizationDisposition.Active)
        {
            var conflict = TryGetActiveByContext(
                authorization.WorkflowInstanceId,
                authorization.AuthorizationKind,
                authorization.NormalizedTrancheKey);
            if (conflict is not null && conflict.AuthorizationId != authorization.AuthorizationId)
            {
                return false;
            }
        }

        _authorizations[authorization.AuthorizationId] = authorization;
        return true;
    }
}
