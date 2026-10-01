namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Domain.Projects;

/// <summary>
/// Operational persistence port for transport operations (ADR-0022 §4–5). SQLite implementation in A4-T2.
/// </summary>
public interface ITransportOperationStore
{
    void Save(TransportOperation operation);

    TransportOperation? Get(TransportOperationId operationId);

    /// <summary>
    /// Returns persisted operations requiring recovery consideration for the Project (via source package association). Not a generalized search/history API (A4-T4).
    /// </summary>
    IReadOnlyList<TransportOperation> GetRecoverableOperations(ProjectConcordProjectId projectId);
}
