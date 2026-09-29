namespace Edf.Domain.Relay;

/// <summary>
/// Optional traceability pointers to canonical EDF artifacts (F-layer); not local governance authority.
/// </summary>
public sealed record EdfGovernanceCorrelation(
    IReadOnlyList<string> ArtifactReferences);
