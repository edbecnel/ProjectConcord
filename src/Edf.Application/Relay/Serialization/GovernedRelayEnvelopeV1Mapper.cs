namespace Edf.Application.Relay.Serialization;

using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

internal static class GovernedRelayEnvelopeV1Mapper
{
    public static GovernedRelayEnvelopeV1Dto ToDto(GovernedRelayPackage package)
    {
        SoftwareDevelopmentProfilePayload? profile = null;
        if (!package.ProfilePayload.IsEmpty
            && SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                package.ProfilePayload,
                out var deserialized,
                out _)
            && deserialized is not null)
        {
            profile = deserialized;
        }

        return new GovernedRelayEnvelopeV1Dto(
            package.PackageId.Value,
            package.CorrelationId.Value,
            package.Kind,
            package.SchemaVersion.Major,
            package.SchemaVersion.Minor,
            package.ProjectId.Value,
            package.CreatedUtc,
            package.UpdatedUtc,
            package.GovernanceCritical,
            package.Tier0Snapshot,
            profile);
    }

    public static GovernedRelayPackage ToPackage(
        GovernedRelayEnvelopeV1Dto dto,
        RelayRenderVersion renderVersion,
        GovernedRelayStructuralAgreement structuralAgreement)
    {
        var profileBytes = dto.SoftwareDevelopmentProfile is null
            ? ReadOnlyMemory<byte>.Empty
            : SoftwareDevelopmentProfilePayloadSerializer.Serialize(dto.SoftwareDevelopmentProfile);

        return new GovernedRelayPackage(
            new GovernedPackageId(dto.PackageId),
            new GovernedCorrelationId(dto.CorrelationId),
            dto.Kind,
            new RelaySchemaVersion(dto.SchemaVersionMajor, dto.SchemaVersionMinor),
            renderVersion,
            new ProjectConcordProjectId(dto.ProjectId),
            dto.CreatedUtc,
            dto.UpdatedUtc,
            dto.GovernanceCritical,
            dto.Tier0Snapshot,
            profileBytes,
            structuralAgreement);
    }
}
