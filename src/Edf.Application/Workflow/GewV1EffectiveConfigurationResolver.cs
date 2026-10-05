using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class GewV1EffectiveConfigurationResolver : IGewV1EffectiveConfigurationResolver
{
    private readonly IPrescribedWorkflowRegistry _registry;

    public GewV1EffectiveConfigurationResolver(IPrescribedWorkflowRegistry registry) =>
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));

    public EffectiveConfigurationResolveResult Resolve(EffectiveConfigurationResolveInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var instance = input.Instance;

        if (!_registry.TryGetDefinition(instance.WorkflowId, instance.DefinitionVersion, out var definition)
            || definition is null)
        {
            return new EffectiveConfigurationUnresolved(
                EffectiveConfigurationResolveFailureCode.UnknownDefinition,
                $"Unknown workflow definition {instance.WorkflowId.Value} v{instance.DefinitionVersion.Value}.");
        }

        if (!_registry.IsKnownPlace(definition, instance.TopologyPlaceId))
        {
            return new EffectiveConfigurationUnresolved(
                EffectiveConfigurationResolveFailureCode.UnknownPlace,
                $"Unknown topology place '{instance.TopologyPlaceId.Value}'.");
        }

        if (instance.ProfileId is not { } profile || profile.IsEmpty)
        {
            return new EffectiveConfigurationUnresolved(
                EffectiveConfigurationResolveFailureCode.MissingProfile,
                "Workflow instance profile is required to resolve effective configuration.");
        }

        if (!_registry.IsKnownProfile(definition, profile))
        {
            return new EffectiveConfigurationUnresolved(
                EffectiveConfigurationResolveFailureCode.UnknownProfile,
                $"Unknown workflow profile '{profile.Value}'.");
        }

        var applicableBounds = input.AuthorizationRecordsForInstance
            .Where(r => DevelopmentWorkAuthorizationApplicability.IsCurrentlyApplicable(r, instance, _registry))
            .Select(r => new GewV1EffectiveConfigurationDwaBound(
                r.AuthorizationKind,
                r.AuthorizedTrancheId,
                DevelopmentWorkAuthorizationScopeMarkers.Deserialize(r.AuthorizedScopeMarkersJson)))
            .ToList();

        var view = new GewV1EffectiveConfigurationView(
            instance.WorkflowId,
            instance.DefinitionVersion,
            profile,
            applicableBounds,
            ProfileSynchronizationFloorsUnspecified: true,
            ProfileEvidenceFloorsUnspecified: true);

        return new EffectiveConfigurationResolved(view);
    }
}
