using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public static class GovernedEffectiveConfigurationGuard
{
    public static void EnsureResolvable(
        IGewV1EffectiveConfigurationResolver resolver,
        WorkflowInstance instance,
        IReadOnlyList<DevelopmentWorkAuthorization> authorizationRecords)
    {
        var result = resolver.Resolve(new EffectiveConfigurationResolveInput(instance, authorizationRecords));
        if (result is EffectiveConfigurationUnresolved unresolved)
        {
            throw new EffectiveConfigurationRequiredException(unresolved.Code, unresolved.Message);
        }
    }
}
