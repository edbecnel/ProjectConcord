namespace Edf.Domain.Relay;

public enum RelayStopState
{
    None = 0,
    Active = 1,
    Acknowledged = 2,
}

/// <summary>
/// Explicit STOP metadata at the relay boundary. Does not imply implementation authorization.
/// </summary>
public sealed record RelayStopMetadata(RelayStopState State, string? ExplicitLabel = null)
{
    public static RelayStopMetadata None => new(RelayStopState.None);
}
