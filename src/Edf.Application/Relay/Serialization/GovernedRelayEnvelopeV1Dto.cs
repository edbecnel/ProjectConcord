namespace Edf.Application.Relay.Serialization;

using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Authoritative machine JSON envelope for relay serialization v1.
/// </summary>
public sealed record GovernedRelayEnvelopeV1Dto(
    Guid PackageId,
    Guid CorrelationId,
    GovernedPackageKind Kind,
    int SchemaVersionMajor,
    int SchemaVersionMinor,
    Guid ProjectId,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc,
    RelayGovernanceCriticalState GovernanceCritical,
    Tier0RelaySnapshot? Tier0Snapshot,
    SoftwareDevelopmentProfilePayload? SoftwareDevelopmentProfile);
