namespace Edf.Domain.Relay;

/// <summary>
/// Declares when B-layer structured blocks are required for a package (set by assembly/import, not inferred).
/// </summary>
public sealed record RelayGovernanceDirectiveFlags(
    bool DirectsImplementationWork,
    bool DirectsTrancheWork);
