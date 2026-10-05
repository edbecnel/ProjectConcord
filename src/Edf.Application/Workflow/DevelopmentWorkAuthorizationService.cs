using Edf.Application.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class DevelopmentWorkAuthorizationService : IDevelopmentWorkAuthorizationService
{
    private readonly IWorkflowInstanceStore _instances;
    private readonly IDevelopmentWorkAuthorizationStore _authorizations;
    private readonly IGewV1EffectiveConfigurationResolver _effectiveConfigurationResolver;
    private readonly IUserApplicationStatePersistence _persistence;

    public DevelopmentWorkAuthorizationService(
        IWorkflowInstanceStore instances,
        IDevelopmentWorkAuthorizationStore authorizations,
        IGewV1EffectiveConfigurationResolver effectiveConfigurationResolver,
        IUserApplicationStatePersistence persistence)
    {
        _instances = instances ?? throw new ArgumentNullException(nameof(instances));
        _authorizations = authorizations ?? throw new ArgumentNullException(nameof(authorizations));
        _effectiveConfigurationResolver = effectiveConfigurationResolver
            ?? throw new ArgumentNullException(nameof(effectiveConfigurationResolver));
        _persistence = persistence ?? throw new ArgumentNullException(nameof(persistence));
    }

    public DevelopmentWorkAuthorization RecordGovernedGrant(
        WorkflowInstanceId workflowInstanceId,
        DevelopmentWorkAuthorizationKind authorizationKind,
        string? authorizedTrancheId,
        IReadOnlyList<string>? authorizedScopeMarkers,
        string? authorityReference,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        var instance = LoadInstance(workflowInstanceId);
        var normalizedTrancheKey = DevelopmentWorkAuthorizationTrancheKey.Normalize(authorizedTrancheId);
        var existingRecords = _authorizations.ListByWorkflowInstance(workflowInstanceId);
        GovernedEffectiveConfigurationGuard.EnsureResolvable(
            _effectiveConfigurationResolver,
            instance,
            existingRecords);

        DevelopmentWorkAuthorization? created = null;
        _persistence.ExecuteInTransaction(() =>
        {
            var existingActive = _authorizations.TryGetActiveByContext(
                workflowInstanceId,
                authorizationKind,
                normalizedTrancheKey);
            if (existingActive is not null)
            {
                SupersedeInStore(existingActive, authority);
            }

            var now = DateTimeOffset.UtcNow;
            created = new DevelopmentWorkAuthorization(
                DevelopmentWorkAuthorizationId.New(),
                instance.ProjectId,
                instance.InstanceId,
                instance.WorkflowId,
                instance.DefinitionVersion,
                instance.ProfileId,
                instance.TopologyPlaceId,
                instance.TraversalOccurrenceId,
                authorizationKind,
                normalizedTrancheKey,
                string.IsNullOrWhiteSpace(authorizedTrancheId) ? null : authorizedTrancheId.Trim(),
                DevelopmentWorkAuthorizationScopeMarkers.Serialize(authorizedScopeMarkers),
                string.IsNullOrWhiteSpace(authorityReference) ? null : authorityReference.Trim(),
                DevelopmentWorkAuthorizationDisposition.Active,
                authority.CorrelationId,
                authority.OriginatingPackageId,
                authority.AuthorityReference,
                now,
                null,
                null,
                null,
                null,
                1,
                now,
                now);

            _authorizations.Insert(created);
        });

        return created!;
    }

    public DevelopmentWorkAuthorization RecordGovernedSupersede(
        DevelopmentWorkAuthorizationId authorizationId,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority)
    {
        ValidateAuthority(authority);
        var current = _authorizations.GetById(authorizationId);
        if (current is null)
        {
            throw new DevelopmentWorkAuthorizationOperationException(
                $"Development work authorization '{authorizationId}' was not found.");
        }

        if (current.Disposition != DevelopmentWorkAuthorizationDisposition.Active)
        {
            throw new DevelopmentWorkAuthorizationOperationException(
                "Only active authorizations may be superseded.");
        }

        _ = LoadInstance(current.WorkflowInstanceId);

        return ApplySupersede(current, expectedResourceVersion, authority);
    }

    private void SupersedeInStore(DevelopmentWorkAuthorization existingActive, GovernedWorkflowMutationAuthority authority)
    {
        ApplySupersede(existingActive, existingActive.ResourceVersion, authority);
    }

    private DevelopmentWorkAuthorization ApplySupersede(
        DevelopmentWorkAuthorization current,
        long expectedResourceVersion,
        GovernedWorkflowMutationAuthority authority)
    {
        if (current.ResourceVersion != expectedResourceVersion)
        {
            throw new DevelopmentWorkAuthorizationConcurrencyException(current.AuthorizationId, expectedResourceVersion);
        }

        var now = DateTimeOffset.UtcNow;
        var updated = current with
        {
            Disposition = DevelopmentWorkAuthorizationDisposition.Superseded,
            SupersessionCorrelationId = authority.CorrelationId,
            SupersessionPackageId = authority.OriginatingPackageId,
            SupersessionAuthorityReference = authority.AuthorityReference,
            SupersededUtc = now,
            ResourceVersion = current.ResourceVersion + 1,
            UpdatedUtc = now,
        };

        if (!_authorizations.TryUpdateWithExpectedVersion(updated, expectedResourceVersion))
        {
            throw new DevelopmentWorkAuthorizationConcurrencyException(current.AuthorizationId, expectedResourceVersion);
        }

        return updated;
    }

    private WorkflowInstance LoadInstance(WorkflowInstanceId instanceId)
    {
        var instance = _instances.GetById(instanceId);
        if (instance is null)
        {
            throw new DevelopmentWorkAuthorizationOperationException($"Workflow instance '{instanceId}' was not found.");
        }

        return instance;
    }

    private static void ValidateAuthority(GovernedWorkflowMutationAuthority authority)
    {
        ArgumentNullException.ThrowIfNull(authority);
        if (!authority.IsWellFormed())
        {
            throw new DevelopmentWorkAuthorizationOperationException("Governed mutation authority is missing or invalid.");
        }
    }
}
