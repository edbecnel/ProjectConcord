namespace Edf.Domain.Workflow;

public readonly record struct TopologyPlaceId(string Value)
{
    public static TopologyPlaceId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException("Topology place id must be non-empty.");
        }

        return new TopologyPlaceId(value);
    }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}
