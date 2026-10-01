namespace Edf.Application.Projects.InMemory;

using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Relay;

public sealed class InMemoryTransportOperationStore : ITransportOperationStore
{
    private readonly Dictionary<TransportOperationId, TransportOperation> _operations = new();

    public void Save(TransportOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations[operation.OperationId] = operation;
    }

    public TransportOperation? Get(TransportOperationId operationId) =>
        _operations.TryGetValue(operationId, out var operation) ? operation : null;
}
