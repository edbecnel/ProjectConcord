namespace Edf.Application.Relay.EngineeringAgent.Plugins;

using Edf.Domain.Relay;

/// <summary>
/// Declares whether the plugin supports semantically adequate automated transport per neutral routing intent (ADR-0022 §9).
/// </summary>
public sealed record EngineeringAgentRoutingIntentSupport(
    bool SupportsPlan,
    bool SupportsAgent,
    bool SupportsDebugSemantically)
{
    public bool SupportsRoutingIntent(EngineeringAgentMode mode) =>
        mode switch
        {
            EngineeringAgentMode.Plan => SupportsPlan,
            EngineeringAgentMode.Agent => SupportsAgent,
            EngineeringAgentMode.Debug => SupportsDebugSemantically,
            _ => false,
        };
}
