namespace Edf.Domain.Relay;

public readonly record struct RelayProvenanceEventId(Guid Value)
{
    public static RelayProvenanceEventId New() => new(Guid.NewGuid());

    public static RelayProvenanceEventId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Relay provenance event id must be a GUID.");
        }

        return new RelayProvenanceEventId(guid);
    }

    public override string ToString() => Value.ToString("D");
}
