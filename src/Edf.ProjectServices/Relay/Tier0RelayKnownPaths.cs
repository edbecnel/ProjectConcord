namespace Edf.ProjectServices.Relay;

/// <summary>
/// Fixed governed paths for shallow Tier-0 existence checks (no repository traversal).
/// </summary>
public static class Tier0RelayKnownPaths
{
    public static readonly IReadOnlyList<string> All =
    [
        "ARCHITECTURE_DECISIONS.md",
        "PROJECT_INDEX.md",
        "docs/Program/Gate_Reviews/",
    ];
}
