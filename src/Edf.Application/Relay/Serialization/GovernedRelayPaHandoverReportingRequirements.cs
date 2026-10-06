namespace Edf.Application.Relay.Serialization;

using Edf.Application.Relay;

/// <summary>
/// Provider-neutral structural requirements for a PA handover import response to a review export.
/// </summary>
public sealed record GovernedRelayPaHandoverReportingRequirements(
    PaHandoverResponseProfile ResponseProfile,
    bool AuthorizationDispositionPresent,
    bool WorkContextPresent,
    bool DirectsImplementationWork,
    bool DirectsTrancheWork,
    bool RequiresDevelopmentWorkAuthorizationProjection,
    bool PlanningAuthorizedFixed,
    bool? PlanningAuthorizedFixedValue,
    bool ImplementationAuthorizedFixed,
    bool? ImplementationAuthorizedFixedValue)
{
    public static GovernedRelayPaHandoverReportingRequirements ForProfile(PaHandoverResponseProfile profile) =>
        profile switch
        {
            PaHandoverResponseProfile.PlanningEntry => PlanningEntry,
            PaHandoverResponseProfile.ImplementationDirected => ImplementationDirected,
            _ => PlanningEntry,
        };

    /// <summary>
    /// Intake governed planning-entry decision. PA decides planning authorization and STOP; implementation stays false.
    /// </summary>
    public static GovernedRelayPaHandoverReportingRequirements PlanningEntry =>
        new(
            PaHandoverResponseProfile.PlanningEntry,
            AuthorizationDispositionPresent: true,
            WorkContextPresent: false,
            DirectsImplementationWork: false,
            DirectsTrancheWork: false,
            RequiresDevelopmentWorkAuthorizationProjection: false,
            PlanningAuthorizedFixed: false,
            PlanningAuthorizedFixedValue: null,
            ImplementationAuthorizedFixed: true,
            ImplementationAuthorizedFixedValue: false);

    /// <summary>
    /// Implementation-directed tranche work (MVR-0002-style semantics).
    /// </summary>
    public static GovernedRelayPaHandoverReportingRequirements ImplementationDirected =>
        new(
            PaHandoverResponseProfile.ImplementationDirected,
            AuthorizationDispositionPresent: true,
            WorkContextPresent: true,
            DirectsImplementationWork: true,
            DirectsTrancheWork: true,
            RequiresDevelopmentWorkAuthorizationProjection: true,
            PlanningAuthorizedFixed: false,
            PlanningAuthorizedFixedValue: null,
            ImplementationAuthorizedFixed: false,
            ImplementationAuthorizedFixedValue: null);
}
