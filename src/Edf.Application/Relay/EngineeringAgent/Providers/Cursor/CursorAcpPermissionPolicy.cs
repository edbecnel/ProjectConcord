namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Operational permission policy for Cursor ACP (deny-by-default; allow-once permitted).
/// </summary>
internal sealed class CursorAcpPermissionPolicy
{
    public Func<CursorAcpPermissionRequest, EngineeringAgentProviderPermissionDecision> Evaluate { get; init; } =
        static _ => new EngineeringAgentProviderPermissionDecision(
            EngineeringAgentProviderPermissionDisposition.Deny,
            "Deny-by-default operational permission policy.");
}

internal sealed record CursorAcpPermissionRequest(string RequestId, string PermissionKind, string? Detail);
