namespace Edf.Application.Workflow;

public sealed class EffectiveConfigurationRequiredException : Exception
{
    public EffectiveConfigurationRequiredException(EffectiveConfigurationResolveFailureCode code, string message)
        : base(message)
    {
        Code = code;
    }

    public EffectiveConfigurationResolveFailureCode Code { get; }
}
