namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Stable Cursor reference provider identity (ADR-0023; A4 plan §6 non-normative example).
/// </summary>
public static class CursorEngineeringAgentPluginIds
{
    public static EngineeringAgentProviderPluginId Reference =>
        EngineeringAgentProviderPluginId.Parse("cursor-acp-reference");
}
