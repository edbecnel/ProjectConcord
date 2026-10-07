namespace Edf.Application.Relay;

using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// P0 manual governed relay workflow orchestration for Desktop (A2-T7). Presentation-free; no automated transport.
/// </summary>
public interface IGovernedRelayP0WorkflowService
{
    RelayP0SessionState GetSessionState(ProjectConcordProjectId projectId);

    void SetProjectArchitectSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent);

    void SetEngineeringAgentSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent);

    PaReviewExportOperationResult GeneratePaReviewExport(
        ProjectConcordProjectId projectId,
        ProjectRoot projectRoot,
        RelayPaReviewExportOptions options);

    PaHandoverImportOperationResult TryValidatePaHandoverImport(
        ProjectConcordProjectId projectId,
        string renderedText,
        GovernedCorrelationId? requiredReviewCorrelationId = null);

    PaHandoverImportOperationResult CommitConsumedPaHandoverImport(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage package,
        RelayValidationResult validation);

    PaHandoverImportOperationResult ImportPaHandover(ProjectConcordProjectId projectId, string renderedText);

    EngineeringAgentHandoverPreparationResult PrepareEngineeringAgentHandover(
        GovernedRelayPackage validatedImportPackage,
        RelayValidationResult importValidation);

    EngineeringResultImportOperationResult ImportEngineeringResult(ProjectConcordProjectId projectId, string renderedText);

    IReadOnlyList<RelayProvenanceEvent> ListRecentProvenance(ProjectConcordProjectId projectId, int maxEvents = 20);
}

public sealed record RelayP0SessionState(
    AgentSessionIntent? ProjectArchitectSessionIntent,
    AgentSessionIntent? EngineeringAgentSessionIntent);

public sealed record RelayPaReviewExportOptions(
    EngineeringAgentMode EngineeringAgentMode,
    EngineeringAgentMode? PriorEngineeringAgentMode,
    PaHandoverResponseProfile PaHandoverResponseProfile = PaHandoverResponseProfile.PlanningEntry);

public sealed record PaReviewExportOperationResult(
    string? RenderedPackage,
    RelayValidationResult Validation,
    GovernedPackageId? PackageId);

public sealed record PaHandoverImportOperationResult(
    PaHandoverImportResult Import,
    bool ProjectIdMatched,
    string? ProjectIdMismatchMessage);

public sealed record EngineeringResultImportOperationResult(
    EngineeringResultImportResult Import,
    bool ProjectIdMatched,
    string? ProjectIdMismatchMessage);
