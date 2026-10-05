namespace Edf.Application.Workflow;

public interface IGewV1EffectiveConfigurationResolver
{
    EffectiveConfigurationResolveResult Resolve(EffectiveConfigurationResolveInput input);
}
