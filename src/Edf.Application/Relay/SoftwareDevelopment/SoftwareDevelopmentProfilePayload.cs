namespace Edf.Application.Relay.SoftwareDevelopment;

/// <summary>
/// B-layer relay profile payload (PC-PAR-012 B-clause). Opaque to Core domain types; carried as JSON bytes on <see cref="Edf.Domain.Relay.GovernedRelayPackage"/>.
/// </summary>
public sealed record SoftwareDevelopmentProfilePayload(
    int PayloadVersion,
    HandoverContextProjection? HandoverContext,
    AuthorizationDispositionProjection? AuthorizationDisposition,
    WorkContextProjection? WorkContext,
    DevelopmentWorkAuthorizationProjection? DevelopmentWorkAuthorization,
    ArchitecturalReviewProjection? ArchitecturalReview,
    ProjectWorkRecordCorrelation? ProjectWorkRecord,
    HumanInitiatedWorkItemCorrelation? HumanInitiatedWorkItem,
    AuthorityGrantPlaceholder? AuthorityGrant);

/// <summary>
/// Inherited handover context (PC-AIGOV-003). Must not substitute for DevelopmentWorkAuthorization.
/// </summary>
public sealed record HandoverContextProjection(
    string? Summary,
    bool IsInheritedContextOnly);

/// <summary>
/// Structured authorization disposition mirrored from relay governance-critical presence (not DWA).
/// </summary>
public sealed record AuthorizationDispositionProjection(
    string? DispositionSummary,
    bool PlanningAuthorized,
    bool ImplementationAuthorized,
    string? AuthorizedTrancheId);

/// <summary>
/// Requested tranche/work scope for packages that direct tranche work.
/// </summary>
public sealed record WorkContextProjection(
    string? RequestedTrancheId,
    string? RequestedWorkSummary);

/// <summary>
/// Relay-bound projection of permitted Software Development execution (not a generic Core grant or PWR).
/// </summary>
public sealed record DevelopmentWorkAuthorizationProjection(
    SoftwareDevelopmentAuthorizationKind Kind,
    string? AuthorizedTrancheId,
    IReadOnlyList<string>? AuthorizedScopeMarkers,
    string? AuthorizationReference,
    bool AuthorizedByStopAcknowledgment);

/// <summary>
/// Explicit architectural review / PA disposition (F correlation); does not imply implementation authorization alone.
/// </summary>
public sealed record ArchitecturalReviewProjection(
    ArchitecturalReviewPaDisposition PaDisposition,
    string? AcceptanceRecordReference,
    bool ImpliesImplementationAuthorization);

public enum ArchitecturalReviewPaDisposition
{
    Unknown = 0,
    Pending = 1,
    Accepted = 2,
    ReturnedForRevision = 3,
}

/// <summary>
/// Optional PWR correlation (ADR-0017). Identity only — must not grant authority.
/// </summary>
public sealed record ProjectWorkRecordCorrelation(
    string? RecordId,
    bool TreatsRecordAsAuthorization);

/// <summary>
/// Optional HIW correlation (SPEC-004 normative name). Intake — must not grant authority.
/// </summary>
public sealed record HumanInitiatedWorkItemCorrelation(
    string? ItemId,
    bool TreatsIntakeAsAuthorization);

/// <summary>
/// Detects deferred generic AuthorityGrant payloads; must not be implemented in A2.
/// </summary>
public sealed record AuthorityGrantPlaceholder(
    string? GrantId);
