namespace Edf.Application.Relay;

using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Application port for durable relay continuity, package records, and provenance (A2-T3).
/// </summary>
public interface IRelayOperationalStore
{
    RelaySessionContinuity GetSessionContinuity(ProjectConcordProjectId projectId);

    void SetProjectArchitectSessionIntent(
        ProjectConcordProjectId projectId,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc);

    void SetEngineeringAgentSessionIntent(
        ProjectConcordProjectId projectId,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc);

    void SetProjectArchitectSessionAdvisory(
        ProjectConcordProjectId projectId,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc);

    void SetEngineeringAgentSessionAdvisory(
        ProjectConcordProjectId projectId,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc);

    void SavePackage(PersistedGovernedRelayPackage persisted);

    PersistedGovernedRelayPackage? GetPackage(GovernedPackageId packageId);

    void AppendProvenanceEvent(RelayProvenanceEvent provenanceEvent);

    IReadOnlyList<RelayProvenanceEvent> ListProvenanceEvents(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId? correlationId = null);
}
