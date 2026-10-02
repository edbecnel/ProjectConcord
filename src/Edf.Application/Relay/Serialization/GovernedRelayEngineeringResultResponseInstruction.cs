namespace Edf.Application.Relay.Serialization;

using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral Engineering Result output contract for automated execution (PC-PAR-022; A4-T6 remediation #3 / #3B).
/// </summary>
public static class GovernedRelayEngineeringResultResponseInstruction
{
    /// <summary>Primary section heading for the complete output contract appended to automated execution prompts.</summary>
    public const string SectionHeading = GovernedRelayEngineeringResultOutputContract.SectionHeading;

    /// <summary>
    /// Renders the complete provider-neutral output contract. Does not alter the handover export package.
    /// </summary>
    public static string Render(GovernedRelayPackage handoverExportPackage) =>
        GovernedRelayEngineeringResultOutputContract.RenderCompleteContract(handoverExportPackage);
}
