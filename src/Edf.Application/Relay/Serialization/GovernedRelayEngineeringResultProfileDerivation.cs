namespace Edf.Application.Relay.Serialization;

using Edf.Domain.Relay;

/// <summary>
/// Derives required Engineering Result reporting for automated transport (A4-T6).
/// Current automated path uses authority-safe thin reporting only.
/// </summary>
public static class GovernedRelayEngineeringResultProfileDerivation
{
    public static GovernedRelayEngineeringResultReportingRequirements DeriveReportingRequirements(
        GovernedRelayPackage handoverExportPackage)
    {
        _ = handoverExportPackage ?? throw new ArgumentNullException(nameof(handoverExportPackage));
        return GovernedRelayEngineeringResultReportingRequirements.Thin;
    }
}
