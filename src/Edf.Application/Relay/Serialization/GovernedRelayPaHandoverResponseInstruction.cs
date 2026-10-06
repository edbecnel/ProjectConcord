namespace Edf.Application.Relay.Serialization;

using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral PA Handover output contract v1 appended to PA review exports.
/// </summary>
public static class GovernedRelayPaHandoverResponseInstruction
{
    public const string SectionHeading = GovernedRelayPaHandoverOutputContract.SectionHeading;

    public static string Render(
        GovernedRelayPackage reviewExportPackage,
        PaHandoverResponseProfile responseProfile) =>
        GovernedRelayPaHandoverOutputContract.RenderCompleteContract(reviewExportPackage, responseProfile);
}
