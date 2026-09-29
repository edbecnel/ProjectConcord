namespace Edf.Domain.Relay;

/// <summary>
/// Internal envelope DTO schema version (distinct from render/serialization version).
/// </summary>
public readonly record struct RelaySchemaVersion(int Major, int Minor)
{
    public static RelaySchemaVersion Current => new(1, 0);

    public override string ToString() => $"{Major}.{Minor}";
}
