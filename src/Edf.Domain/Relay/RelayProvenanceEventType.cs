namespace Edf.Domain.Relay;

/// <summary>
/// PC-PAR-021 provenance chain stages persisted for governed relay activity.
/// </summary>
public enum RelayProvenanceEventType
{
    ObservedContext = 0,
    Advisory = 1,
    UserDecision = 2,
    RequestedNextAction = 3,
    PackageProduced = 4,
    PackageConsumed = 5,
}
