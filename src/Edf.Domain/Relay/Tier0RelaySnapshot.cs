namespace Edf.Domain.Relay;

/// <summary>
/// Immutable Tier-0 context captured at package assembly time (DTO only in T1; provider is T2).
/// </summary>
public sealed record Tier0RelaySnapshot(
    string? GitHeadCommit,
    IReadOnlyList<string> KnownPathsPresent,
    IReadOnlyDictionary<string, string> NarrowMetadata)
{
    public static Tier0RelaySnapshot Empty =>
        new(null, Array.Empty<string>(), new Dictionary<string, string>());
}
