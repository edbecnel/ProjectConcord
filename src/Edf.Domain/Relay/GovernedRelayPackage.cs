namespace Edf.Domain.Relay;

using Edf.Domain.Projects;

/// <summary>
/// Transport-neutral governed relay package envelope (Core layer A).
/// </summary>
public sealed record GovernedRelayPackage(
    GovernedPackageId PackageId,
    GovernedCorrelationId CorrelationId,
    GovernedPackageKind Kind,
    RelaySchemaVersion SchemaVersion,
    RelayRenderVersion? RenderVersion,
    ProjectConcordProjectId ProjectId,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc,
    RelayGovernanceCriticalState GovernanceCritical,
    Tier0RelaySnapshot? Tier0Snapshot,
    ReadOnlyMemory<byte> ProfilePayload,
    GovernedRelayStructuralAgreement StructuralAgreement)
{
    public static GovernedRelayStructuralAgreement DefaultStructuralAgreement =>
        new(
            RequiresMachineBlock: false,
            MachineBlockPresent: true,
            RequiresGovernanceProjectionAgreement: false,
            GovernanceProjectionsAgree: true);
}
