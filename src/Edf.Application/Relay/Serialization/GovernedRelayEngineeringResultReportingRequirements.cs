namespace Edf.Application.Relay.Serialization;

/// <summary>
/// Provider-neutral required Engineering Result governance reporting on the result package (not copied from handover authority).
/// </summary>
public sealed record GovernedRelayEngineeringResultReportingRequirements(
    bool DirectsImplementationWork,
    bool DirectsTrancheWork,
    bool AuthorizationDispositionPresent,
    bool WorkContextPresent)
{
    public static GovernedRelayEngineeringResultReportingRequirements Thin =>
        new(false, false, false, false);

    public bool IsRich =>
        DirectsImplementationWork
        || DirectsTrancheWork
        || AuthorizationDispositionPresent
        || WorkContextPresent;
}
