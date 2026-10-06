namespace Edf.Application.Operator.PlanningEntry;

using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// Durable relay evidence for Planning Entry guided exchange (read projection only).
/// </summary>
public sealed record PlanningEntryRelayHandoverSnapshot(
    GovernedRelayPackage Package,
    RelayValidationResult Validation);

public sealed record PlanningEntryRelayReadModelSnapshot(
    PlanningEntryRelayHandoverSnapshot? LatestConsumedPaHandover,
    PlanningEntryRelayHandoverSnapshot? LatestPaReviewExport);
