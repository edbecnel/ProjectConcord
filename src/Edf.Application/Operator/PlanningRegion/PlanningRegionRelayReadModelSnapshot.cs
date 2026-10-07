namespace Edf.Application.Operator.PlanningRegion;

public sealed record PlanningRegionRelayReadModelSnapshot(
    PlanningRegionRelayHandoverSnapshot? LatestConsumedPaHandover,
    PlanningRegionRelayHandoverSnapshot? LatestPaReviewExport);
