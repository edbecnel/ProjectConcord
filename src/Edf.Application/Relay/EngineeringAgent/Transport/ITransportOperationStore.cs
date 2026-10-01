namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Operational persistence port for transport operations (ADR-0022 §4–5). SQLite implementation in A4-T2.
/// </summary>
public interface ITransportOperationStore
{
    void Save(TransportOperation operation);

    TransportOperation? Get(TransportOperationId operationId);
}
