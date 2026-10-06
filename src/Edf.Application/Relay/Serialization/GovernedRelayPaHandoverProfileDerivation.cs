namespace Edf.Application.Relay.Serialization;

using Edf.Application.Relay;
using Edf.Domain.Relay;

public static class GovernedRelayPaHandoverProfileDerivation
{
    public static GovernedRelayPaHandoverReportingRequirements DeriveReportingRequirements(
        GovernedRelayPackage reviewExportPackage,
        PaHandoverResponseProfile responseProfile)
    {
        ArgumentNullException.ThrowIfNull(reviewExportPackage);
        if (reviewExportPackage.Kind != GovernedPackageKind.PaReviewExport)
        {
            throw new ArgumentException(
                "PA handover output contract requires a PaReviewExport package.",
                nameof(reviewExportPackage));
        }

        return GovernedRelayPaHandoverReportingRequirements.ForProfile(responseProfile);
    }
}
