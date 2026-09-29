namespace Edf.Application.Relay;

using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// T3 relay orchestration: continuity, package persistence, and provenance (no render/parse/UI).
/// </summary>
public interface IGovernedInteractionRelayService
{
    RelaySessionContinuity GetSessionContinuity(ProjectConcordProjectId projectId);

    void SetProjectArchitectSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent? userIntent);

    void SetEngineeringAgentSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent? userIntent);

    void SetProjectArchitectSessionAdvisory(ProjectConcordProjectId projectId, AgentSessionAdvisory advisory);

    void SetEngineeringAgentSessionAdvisory(ProjectConcordProjectId projectId, AgentSessionAdvisory advisory);

    /// <summary>
    /// Records a produced package (export path) with Tier-0 capture and provenance events; no rendering.
    /// </summary>
    PersistedGovernedRelayPackage RecordProducedPackage(
        GovernedRelayPackage package,
        RelayValidationResult validation,
        ProjectRoot? projectRootForTier0 = null,
        string? renderedBodyHash = null);

    /// <summary>
    /// Records a consumed package (import path) with validation disposition and provenance events.
    /// </summary>
    PersistedGovernedRelayPackage RecordConsumedPackage(
        GovernedRelayPackage package,
        RelayValidationResult validation);

    IReadOnlyList<RelayProvenanceEvent> ListProvenanceEvents(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId? correlationId = null);
}
