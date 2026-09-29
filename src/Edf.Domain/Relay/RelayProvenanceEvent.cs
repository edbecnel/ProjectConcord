namespace Edf.Domain.Relay;

using Edf.Domain.Projects;

/// <summary>
/// Append-only relay provenance record (operational store; not canonical chat transcript).
/// </summary>
public sealed record RelayProvenanceEvent(
    RelayProvenanceEventId EventId,
    ProjectConcordProjectId ProjectId,
    GovernedPackageId? PackageId,
    GovernedCorrelationId? CorrelationId,
    RelayProvenanceEventType EventType,
    string PayloadJson,
    DateTimeOffset RecordedUtc);
