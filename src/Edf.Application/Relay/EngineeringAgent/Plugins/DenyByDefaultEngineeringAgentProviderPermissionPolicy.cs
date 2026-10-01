namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Architectural default: deny scope/authority expansion unless orchestration explicitly allows once (ADR-0022 §15).
/// </summary>
public sealed class DenyByDefaultEngineeringAgentProviderPermissionPolicy : IEngineeringAgentProviderPermissionPolicy
{
    public EngineeringAgentProviderPermissionDecision Evaluate(EngineeringAgentProviderPermissionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        return new EngineeringAgentProviderPermissionDecision(
            EngineeringAgentProviderPermissionDisposition.Deny,
            Rationale: "Deny-by-default provider permission policy.");
    }
}
