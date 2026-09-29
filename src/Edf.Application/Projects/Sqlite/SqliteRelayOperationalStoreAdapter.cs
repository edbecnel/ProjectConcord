using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteRelayOperationalStoreAdapter : IRelayOperationalStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteRelayOperationalStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public RelaySessionContinuity GetSessionContinuity(ProjectConcordProjectId projectId) =>
        _store.GetRelaySessionContinuity(projectId);

    public void SetProjectArchitectSessionIntent(
        ProjectConcordProjectId projectId,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc) =>
        _store.SetRelaySessionIntent(projectId, RelayAgentRole.ProjectArchitect, userIntent, updatedUtc);

    public void SetEngineeringAgentSessionIntent(
        ProjectConcordProjectId projectId,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc) =>
        _store.SetRelaySessionIntent(projectId, RelayAgentRole.EngineeringAgent, userIntent, updatedUtc);

    public void SetProjectArchitectSessionAdvisory(
        ProjectConcordProjectId projectId,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc) =>
        _store.SetRelaySessionAdvisory(projectId, RelayAgentRole.ProjectArchitect, advisory, updatedUtc);

    public void SetEngineeringAgentSessionAdvisory(
        ProjectConcordProjectId projectId,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc) =>
        _store.SetRelaySessionAdvisory(projectId, RelayAgentRole.EngineeringAgent, advisory, updatedUtc);

    public void SavePackage(PersistedGovernedRelayPackage persisted) =>
        _store.UpsertRelayPackage(persisted);

    public PersistedGovernedRelayPackage? GetPackage(GovernedPackageId packageId) =>
        _store.GetRelayPackage(packageId);

    public void AppendProvenanceEvent(RelayProvenanceEvent provenanceEvent) =>
        _store.AppendRelayProvenanceEvent(provenanceEvent);

    public IReadOnlyList<RelayProvenanceEvent> ListProvenanceEvents(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId? correlationId = null) =>
        _store.ListRelayProvenanceEvents(projectId, correlationId);
}
