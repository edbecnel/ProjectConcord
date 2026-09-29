namespace Edf.Domain.Relay;

/// <summary>
/// Stable identifier for one governed relay package instance.
/// </summary>
public readonly record struct GovernedPackageId(Guid Value)
{
    public static GovernedPackageId New() => new(Guid.NewGuid());

    public static GovernedPackageId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Governed package id must be a GUID.");
        }

        return new GovernedPackageId(guid);
    }

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
