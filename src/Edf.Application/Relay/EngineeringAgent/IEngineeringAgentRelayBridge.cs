namespace Edf.Application.Relay.EngineeringAgent;

using Edf.Domain.Relay;

/// <summary>
/// Engineering-agent transport boundary (PC-PAR-022). P0 manual copy/paste only — no automated IDE transport.
/// </summary>
public interface IEngineeringAgentRelayBridge
{
    EngineeringAgentRelayBridgeCapabilities DeclareCapabilities();

    /// <summary>
    /// Produces engineering-agent-directed Markdown from a governed package only when boundary validation is
    /// <see cref="RelayValidationState.Valid"/> per <see cref="RelayValidatedHandoverEligibility"/>.
    /// </summary>
    EngineeringAgentHandoverPreparationResult TryRenderValidatedHandover(
        GovernedRelayPackage package,
        RelayValidationResult boundaryValidation);

    /// <summary>
    /// Provider-neutral instruction requiring a governed <c>engineeringResultImport</c> response (automated execution).
    /// </summary>
    string RenderEngineeringResultResponseInstruction(GovernedRelayPackage handoverExportPackage);

    /// <summary>
    /// Composes canonical P0 handover with the Engineering Result response instruction for automated transport.
    /// </summary>
    string ComposeAutomatedExecutionPrompt(
        string canonicalRenderedHandover,
        GovernedRelayPackage handoverExportPackage);

    /// <summary>
    /// Parses optional thin engineering result / evidence pasted back from the engineering agent (untrusted input).
    /// </summary>
    EngineeringResultImportResult TryParseEngineeringResult(string renderedText);
}
