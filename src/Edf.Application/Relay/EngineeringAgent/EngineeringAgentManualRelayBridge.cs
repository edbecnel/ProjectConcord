namespace Edf.Application.Relay.EngineeringAgent;

using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// P0 manual engineering-agent bridge (PC-PAR-014, PC-PAR-022). Reuses <c>projectconcord-relay-v1</c>; no automated transport.
/// </summary>
public sealed class EngineeringAgentManualRelayBridge : IEngineeringAgentRelayBridge
{
    private readonly GovernedRelayV1Renderer _renderer = new();
    private readonly GovernedRelayV1Importer _importer = new();
    private readonly GovernedRelayPackageValidator _validator =
        new(SoftwareDevelopmentRelayProfileValidator.Instance);

    public EngineeringAgentRelayBridgeCapabilities DeclareCapabilities() =>
        new(
            SupportsManualPaste: true,
            SupportsAutomatedTransport: false,
            SupportedRenderVersionMajor: RelayRenderVersion.V1.Major);

    public EngineeringAgentHandoverPreparationResult TryRenderValidatedHandover(
        GovernedRelayPackage package,
        RelayValidationResult boundaryValidation)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(boundaryValidation);

        if (!boundaryValidation.IsEligibleForValidatedEngineeringAgentHandover)
        {
            return Blocked(null, boundaryValidation);
        }

        if (!IsHandoverSourceKind(package.Kind))
        {
            return Blocked(
                package,
                RelayValidationResult.Incomplete(
                [
                    new RelayValidationDiagnostic(
                        RelayValidationCodes.EngineeringAgentHandoverPackageKindUnsupported,
                        "Validated engineering-agent handover preparation requires a PA handover import or prior engineering-agent export package kind.",
                        RelayValidationDiagnosticSeverity.Incomplete),
                ]));
        }

        if (package.GovernanceCritical.Stop.State == RelayStopState.Active)
        {
            return Blocked(
                package,
                RelayValidationResult.Valid(
                [
                    new RelayValidationDiagnostic(
                        RelayValidationCodes.EngineeringAgentHandoverBlockedByActiveStop,
                        "Active STOP blocks engineering-agent handover preparation; package validity is unchanged (Valid != actionable while STOP is active).",
                        RelayValidationDiagnosticSeverity.Information),
                ]));
        }

        var exportPackage = package with { Kind = GovernedPackageKind.EngineeringAgentHandoverExport };
        var substantiveValidation = _validator.Validate(exportPackage);
        if (!substantiveValidation.IsEligibleForValidatedEngineeringAgentHandover)
        {
            return Blocked(exportPackage, substantiveValidation);
        }

        var rendered = _renderer.Render(exportPackage);
        return new EngineeringAgentHandoverPreparationResult(
            IsReadyForManualTransfer: true,
            RenderedHandover: rendered,
            ExportPackage: exportPackage,
            Validation: substantiveValidation);
    }

    public string ComposeAutomatedExecutionPrompt(
        string canonicalRenderedHandover,
        GovernedRelayPackage handoverExportPackage) =>
        GovernedRelayAutomatedExecutionPrompt.Compose(canonicalRenderedHandover, handoverExportPackage);

    public string RenderEngineeringResultResponseInstruction(GovernedRelayPackage handoverExportPackage) =>
        GovernedRelayEngineeringResultResponseInstruction.Render(handoverExportPackage);

    public EngineeringResultImportResult TryParseEngineeringResult(string renderedText)
    {
        var import = _importer.Import(renderedText);
        if (import.Package is null)
        {
            return new EngineeringResultImportResult(null, import.StructuralResult);
        }

        if (import.StructuralResult.State == RelayValidationState.RejectedMalformed)
        {
            return new EngineeringResultImportResult(import.Package, import.StructuralResult);
        }

        if (import.Package.Kind != GovernedPackageKind.EngineeringResultImport)
        {
            return new EngineeringResultImportResult(
                import.Package,
                RelayValidationResult.Incomplete(
                [
                    new RelayValidationDiagnostic(
                        RelayValidationCodes.EngineeringResultPackageKindMismatch,
                        "Engineering result import must declare package kind EngineeringResultImport in the machine block.",
                        RelayValidationDiagnosticSeverity.Incomplete),
                ]));
        }

        var validation = _validator.Validate(import.Package);
        return new EngineeringResultImportResult(import.Package, validation);
    }

    private static bool IsHandoverSourceKind(GovernedPackageKind kind) =>
        kind is GovernedPackageKind.PaHandoverImport or GovernedPackageKind.EngineeringAgentHandoverExport;

    private static EngineeringAgentHandoverPreparationResult Blocked(
        GovernedRelayPackage? exportPackage,
        RelayValidationResult validation) =>
        new(
            IsReadyForManualTransfer: false,
            RenderedHandover: null,
            ExportPackage: exportPackage,
            Validation: validation);
}
