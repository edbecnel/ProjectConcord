using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryRelayOperationalStore : IRelayOperationalStore
{
    private readonly object _sync = new();
    private readonly Dictionary<ProjectConcordProjectId, RelaySessionContinuity> _continuity = new();
    private readonly Dictionary<GovernedPackageId, PersistedGovernedRelayPackage> _packages = new();
    private readonly List<RelayProvenanceEvent> _events = [];

    public RelaySessionContinuity GetSessionContinuity(ProjectConcordProjectId projectId)
    {
        lock (_sync)
        {
            return _continuity.GetValueOrDefault(projectId)
                ?? new RelaySessionContinuity(null, AgentSessionAdvisory.None, null, AgentSessionAdvisory.None);
        }
    }

    public void SetProjectArchitectSessionIntent(
        ProjectConcordProjectId projectId,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc) =>
        UpdateContinuity(
            projectId,
            current => current with { ProjectArchitectSessionIntent = userIntent });

    public void SetEngineeringAgentSessionIntent(
        ProjectConcordProjectId projectId,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc) =>
        UpdateContinuity(
            projectId,
            current => current with { EngineeringAgentSessionIntent = userIntent });

    public void SetProjectArchitectSessionAdvisory(
        ProjectConcordProjectId projectId,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc) =>
        UpdateContinuity(
            projectId,
            current => current with { ProjectArchitectSessionAdvisory = advisory });

    public void SetEngineeringAgentSessionAdvisory(
        ProjectConcordProjectId projectId,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc) =>
        UpdateContinuity(
            projectId,
            current => current with { EngineeringAgentSessionAdvisory = advisory });

    public void SavePackage(PersistedGovernedRelayPackage persisted)
    {
        lock (_sync)
        {
            _packages[persisted.Package.PackageId] = persisted;
        }
    }

    public PersistedGovernedRelayPackage? GetPackage(GovernedPackageId packageId)
    {
        lock (_sync)
        {
            return _packages.GetValueOrDefault(packageId);
        }
    }

    public void AppendProvenanceEvent(RelayProvenanceEvent provenanceEvent)
    {
        lock (_sync)
        {
            _events.Add(provenanceEvent);
        }
    }

    public IReadOnlyList<RelayProvenanceEvent> ListProvenanceEvents(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId? correlationId = null)
    {
        lock (_sync)
        {
            IEnumerable<RelayProvenanceEvent> query = _events.Where(e => e.ProjectId.Value == projectId.Value);
            if (correlationId is not null)
            {
                query = query.Where(e => e.CorrelationId?.Value == correlationId.Value.Value);
            }

            return query
                .OrderBy(e => e.RecordedUtc)
                .ThenBy(e => e.EventId.Value)
                .ToList();
        }
    }

    private void UpdateContinuity(
        ProjectConcordProjectId projectId,
        Func<RelaySessionContinuity, RelaySessionContinuity> update)
    {
        lock (_sync)
        {
            var current = GetSessionContinuity(projectId);
            _continuity[projectId] = update(current);
        }
    }
}
