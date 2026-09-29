namespace Edf.Domain.Relay;

/// <summary>
/// Links export, import, and downstream handover within one user relay cycle.
/// </summary>
public readonly record struct GovernedCorrelationId(Guid Value)
{
    public static GovernedCorrelationId New() => new(Guid.NewGuid());

    public static GovernedCorrelationId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Governed correlation id must be a GUID.");
        }

        return new GovernedCorrelationId(guid);
    }

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
