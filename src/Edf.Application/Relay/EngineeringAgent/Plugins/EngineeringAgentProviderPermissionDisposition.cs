namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Neutral provider permission disposition (ADR-0022 §15). Operational only — not governance authority.
/// </summary>
public enum EngineeringAgentProviderPermissionDisposition
{
    Deny = 0,
    AllowOnce = 1,
}
