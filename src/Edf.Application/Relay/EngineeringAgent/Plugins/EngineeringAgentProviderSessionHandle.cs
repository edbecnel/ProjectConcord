namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Opaque, provider-specific, noncanonical session continuity hint (ADR-0022 §11). Not governed identity.
/// </summary>
public readonly record struct EngineeringAgentProviderSessionHandle(string Value)
{
    public static EngineeringAgentProviderSessionHandle FromOpaque(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new EngineeringAgentProviderSessionHandle(value);
    }

    public override string ToString() => "(opaque provider session handle)";
}
