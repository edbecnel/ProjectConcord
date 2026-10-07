namespace Edf.Application.Operator.PlanningRegion;

using Edf.Application.Relay;
using Edf.Domain.Relay;

public sealed record PlanningRegionRelayHandoverSnapshot(
    GovernedRelayPackage Package,
    RelayValidationResult Validation);
