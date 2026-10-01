namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Outcome of a provider permission evaluation. Deny-by-default; allow-once permitted; allow-always is not the architectural default.
/// </summary>
public sealed record EngineeringAgentProviderPermissionDecision(
    EngineeringAgentProviderPermissionDisposition Disposition,
    string? Rationale = null);
