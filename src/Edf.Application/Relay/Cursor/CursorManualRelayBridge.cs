namespace Edf.Application.Relay.Cursor;

using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// P0 manual Cursor bridge (PC-PAR-014, PC-PAR-022). Reuses <c>projectconcord-relay-v1</c>; no automated transport.
/// </summary>
public sealed class CursorManualRelayBridge : ICursorRelayBridge
{
    private readonly GovernedRelayV1Renderer _renderer = new();
    private readonly GovernedRelayV1Importer _importer = new();
    private readonly GovernedRelayPackageValidator _validator =
        new(SoftwareDevelopmentRelayProfileValidator.Instance);

    public CursorRelayBridgeCapabilities DeclareCapabilities() =>
        new(
            SupportsManualPaste: true,
            SupportsAutomatedTransport: false,
            SupportedRenderVersionMajor: RelayRenderVersion.V1.Major);

    public CursorHandoverPreparationResult TryRenderValidatedHandover(
        GovernedRelayPackage package,
        RelayValidationResult boundaryValidation)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(boundaryValidation);

        if (!boundaryValidation.IsEligibleForValidatedCursorHandover)
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
                        RelayValidationCodes.CursorHandoverPackageKindUnsupported,
                        "Validated Cursor handover preparation requires a PA handover import or prior Cursor export package kind.",
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
                        RelayValidationCodes.CursorHandoverBlockedByActiveStop,
                        "Active STOP blocks Cursor handover preparation; package validity is unchanged (Valid != actionable while STOP is active).",
                        RelayValidationDiagnosticSeverity.Information),
                ]));
        }

        var exportPackage = package with { Kind = GovernedPackageKind.CursorHandoverExport };
        var substantiveValidation = _validator.Validate(exportPackage);
        if (!substantiveValidation.IsEligibleForValidatedCursorHandover)
        {
            return Blocked(exportPackage, substantiveValidation);
        }

        var rendered = _renderer.Render(exportPackage);
        return new CursorHandoverPreparationResult(
            IsReadyForManualTransfer: true,
            RenderedHandover: rendered,
            ExportPackage: exportPackage,
            Validation: substantiveValidation);
    }

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
        kind is GovernedPackageKind.PaHandoverImport or GovernedPackageKind.CursorHandoverExport;

    private static CursorHandoverPreparationResult Blocked(
        GovernedRelayPackage? exportPackage,
        RelayValidationResult validation) =>
        new(
            IsReadyForManualTransfer: false,
            RenderedHandover: null,
            ExportPackage: exportPackage,
            Validation: validation);
}
