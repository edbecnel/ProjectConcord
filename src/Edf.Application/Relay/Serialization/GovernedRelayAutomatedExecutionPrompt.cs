namespace Edf.Application.Relay.Serialization;

using Edf.Domain.Relay;

/// <summary>
/// Composes canonical P0 handover text with the provider-neutral Engineering Result response instruction
/// for automated transport execution (opaque to provider plugins).
/// </summary>
public static class GovernedRelayAutomatedExecutionPrompt
{
    /// <summary>
    /// Builds the execution prompt: validated handover render + response contract. P0 handover-only render is unchanged.
    /// </summary>
    public static string Compose(string canonicalRenderedHandover, GovernedRelayPackage handoverExportPackage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(canonicalRenderedHandover);
        ArgumentNullException.ThrowIfNull(handoverExportPackage);

        var instruction = GovernedRelayEngineeringResultResponseInstruction.Render(handoverExportPackage);
        return canonicalRenderedHandover.TrimEnd()
            + "\n\n"
            + instruction
            + Environment.NewLine;
    }
}
