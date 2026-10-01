namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Provider-neutral hosting boundary for initialization and invocation (ADR-0023 §2). Implementation in A4-T1.
/// </summary>
public interface IEngineeringAgentPluginHost
{
    Task<EngineeringAgentProviderInitializeResult> InitializePluginAsync(
        EngineeringAgentProviderPluginId pluginId,
        CancellationToken cancellationToken = default);

    Task<EngineeringAgentProviderShutdownResult> ShutdownPluginAsync(
        EngineeringAgentProviderPluginId pluginId,
        CancellationToken cancellationToken = default);
}
