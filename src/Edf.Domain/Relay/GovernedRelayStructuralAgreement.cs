namespace Edf.Domain.Relay;

/// <summary>
/// Structured-model agreement between machine envelope and projected governance duplicates (import seam for T5).
/// </summary>
public sealed record GovernedRelayStructuralAgreement(
    bool RequiresMachineBlock,
    bool MachineBlockPresent,
    bool RequiresGovernanceProjectionAgreement,
    bool GovernanceProjectionsAgree);
