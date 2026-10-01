namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Relay;

/// <summary>
/// Provider-internal PLAN/AGENT mapping. DEBUG is never silently remapped (ADR-0022 §9).
/// </summary>
internal static class CursorRoutingIntentMapper
{
    internal static bool TryToCursorAcpMode(
        EngineeringAgentMode routingIntent,
        out string cursorMode,
        out EngineeringAgentProviderFailure? failure)
    {
        switch (routingIntent)
        {
            case EngineeringAgentMode.Plan:
                cursorMode = "plan";
                failure = null;
                return true;
            case EngineeringAgentMode.Agent:
                cursorMode = "agent";
                failure = null;
                return true;
            case EngineeringAgentMode.Debug:
                cursorMode = string.Empty;
                failure = new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.RoutingIntentUnsupported,
                    "Cursor automated transport does not support DEBUG routing intent.");
                return false;
            default:
                cursorMode = string.Empty;
                failure = new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.RoutingIntentUnsupported,
                    $"Unsupported routing intent: {routingIntent}.");
                return false;
        }
    }
}
