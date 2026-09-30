namespace Edf.Application.Relay.ProjectArchitect;

using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// P0 manual ChatGPT-oriented Project Architect adapter (PC-PAR-017). No automated API transport.
/// </summary>
public sealed class ProjectArchitectManualAdapter : IProjectArchitectProvider
{
    private readonly GovernedRelayV1Renderer _renderer = new();
    private readonly GovernedRelayV1Importer _importer = new();
    private readonly GovernedRelayPackageValidator _validator =
        new(SoftwareDevelopmentRelayProfileValidator.Instance);

    public ProjectArchitectProviderCapabilities DeclareCapabilities() =>
        new(
            SupportsManualPaste: true,
            SupportedRenderVersionMajor: RelayRenderVersion.V1.Major,
            SupportsGovernanceCriticalProjections: true,
            SupportsSoftwareDevelopmentProfilePayload: true);

    public string RenderPaReviewPackage(GovernedRelayPackage package) =>
        _renderer.Render(package);

    public PaHandoverImportResult TryParsePaHandoverImport(string renderedText)
    {
        var import = _importer.Import(renderedText);
        if (import.Package is null)
        {
            return new PaHandoverImportResult(null, import.StructuralResult);
        }

        if (import.StructuralResult.State == RelayValidationState.RejectedMalformed)
        {
            return new PaHandoverImportResult(import.Package, import.StructuralResult);
        }

        var validation = _validator.Validate(import.Package);
        return new PaHandoverImportResult(import.Package, validation);
    }
}
