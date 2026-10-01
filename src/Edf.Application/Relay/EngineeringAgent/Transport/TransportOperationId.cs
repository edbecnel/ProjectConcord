namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Stable identity for one logical automated forward cycle (ADR-0022 §4). Operational — not a governed package id.
/// </summary>
public readonly record struct TransportOperationId(Guid Value)
{
    public static TransportOperationId New() => new(Guid.NewGuid());

    public static TransportOperationId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException($"Invalid transport operation id: '{value}'.");
        }

        return new TransportOperationId(guid);
    }

    public override string ToString() => Value.ToString("D");
}
