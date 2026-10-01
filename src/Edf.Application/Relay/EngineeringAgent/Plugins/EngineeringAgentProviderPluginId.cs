namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Stable provider plugin identity for registration, configuration, and transport attribution (ADR-0023).
/// Distinct from governed package ids, correlation ids, and transport operation ids.
/// </summary>
public readonly record struct EngineeringAgentProviderPluginId(string Value)
{
    public static EngineeringAgentProviderPluginId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Provider plugin identity must be non-empty.", nameof(value));
        }

        return new EngineeringAgentProviderPluginId(value.Trim());
    }

    public override string ToString() => Value;
}
