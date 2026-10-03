namespace Edf.Domain.Workflow;

/// <summary>
/// Opaque identity for one entry/re-entry to a topology place. Not an ordinal.
/// </summary>
public readonly record struct TraversalOccurrenceId(Guid Value)
{
    public static TraversalOccurrenceId New() => new(Guid.NewGuid());

    public static TraversalOccurrenceId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Traversal occurrence id must be a GUID.");
        }

        return new TraversalOccurrenceId(guid);
    }

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
