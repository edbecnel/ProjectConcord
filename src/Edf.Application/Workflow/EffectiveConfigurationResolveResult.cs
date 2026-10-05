namespace Edf.Application.Workflow;

public abstract record EffectiveConfigurationResolveResult;

public sealed record EffectiveConfigurationResolved(GewV1EffectiveConfigurationView View)
    : EffectiveConfigurationResolveResult;

public sealed record EffectiveConfigurationUnresolved(
    EffectiveConfigurationResolveFailureCode Code,
    string Message)
    : EffectiveConfigurationResolveResult;
