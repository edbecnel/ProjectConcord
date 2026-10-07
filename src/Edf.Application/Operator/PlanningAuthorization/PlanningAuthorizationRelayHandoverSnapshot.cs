namespace Edf.Application.Operator.PlanningAuthorization;

using Edf.Application.Relay;
using Edf.Domain.Relay;

public sealed record PlanningAuthorizationRelayHandoverSnapshot(
    GovernedRelayPackage Package,
    RelayValidationResult Validation);

public sealed record PlanningAuthorizationRelayReadModelSnapshot(
    PlanningAuthorizationRelayHandoverSnapshot? LatestConsumedPaHandover,
    PlanningAuthorizationRelayHandoverSnapshot? LatestPaReviewExport);
