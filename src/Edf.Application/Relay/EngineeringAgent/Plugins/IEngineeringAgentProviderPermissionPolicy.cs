namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Evaluates provider permission requests under ProjectConcord policy (ADR-0022 §15). Implemented by orchestration in later tranches.
/// </summary>
public interface IEngineeringAgentProviderPermissionPolicy
{
    EngineeringAgentProviderPermissionDecision Evaluate(EngineeringAgentProviderPermissionRequest request);
}

/// <summary>
/// Neutral permission request surface for provider tool/scope expansion (operational only).
/// </summary>
public sealed record EngineeringAgentProviderPermissionRequest(
    EngineeringAgentProviderPluginId PluginId,
    string OperationDescription);
