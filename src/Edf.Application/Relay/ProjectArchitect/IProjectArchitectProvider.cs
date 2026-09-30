namespace Edf.Application.Relay.ProjectArchitect;

using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral Project Architect transport boundary (PC-PAR-016). P0 manual paste only.
/// </summary>
public interface IProjectArchitectProvider
{
    ProjectArchitectProviderCapabilities DeclareCapabilities();

    string RenderPaReviewPackage(GovernedRelayPackage package);

    PaHandoverImportResult TryParsePaHandoverImport(string renderedText);
}
