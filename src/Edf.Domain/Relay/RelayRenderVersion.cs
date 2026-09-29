namespace Edf.Domain.Relay;

/// <summary>
/// Serialization/rendering version marker carried on rendered relay artifacts.
/// </summary>
public readonly record struct RelayRenderVersion(int Major, int Minor)
{
    public static RelayRenderVersion V1 => new(1, 0);

    public override string ToString() => $"{Major}.{Minor}";
}
