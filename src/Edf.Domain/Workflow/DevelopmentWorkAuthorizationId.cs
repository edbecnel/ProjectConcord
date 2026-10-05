namespace Edf.Domain.Workflow;

public readonly record struct DevelopmentWorkAuthorizationId(Guid Value)
{
    public static DevelopmentWorkAuthorizationId New() => new(Guid.NewGuid());

    public static DevelopmentWorkAuthorizationId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Development work authorization id must be a GUID.");
        }

        return new DevelopmentWorkAuthorizationId(guid);
    }

    public override string ToString() => Value.ToString();
}
