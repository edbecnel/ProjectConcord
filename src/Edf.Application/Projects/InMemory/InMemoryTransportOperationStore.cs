namespace Edf.Application.Projects.InMemory;

using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Projects;

public sealed class InMemoryTransportOperationStore : ITransportOperationStore
{
    private readonly IRelayOperationalStore? _relayOperational;
    private readonly Dictionary<TransportOperationId, TransportOperation> _operations = new();

    public InMemoryTransportOperationStore()
        : this(relayOperational: null)
    {
    }

    public InMemoryTransportOperationStore(IRelayOperationalStore relayOperational)
    {
        _relayOperational = relayOperational;
    }

    public void Save(TransportOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _operations[operation.OperationId] = operation;
    }

    public TransportOperation? Get(TransportOperationId operationId) =>
        _operations.TryGetValue(operationId, out var operation) ? operation : null;

    public IReadOnlyList<TransportOperation> GetRecoverableOperations(ProjectConcordProjectId projectId)
    {
        if (_relayOperational is null)
        {
            return Array.Empty<TransportOperation>();
        }

        return _operations.Values
            .Where(op => TransportOperationRecoverySemantics.RequiresRecoveryConsideration(op.LifecycleState))
            .Where(op => BelongsToProject(op, projectId))
            .OrderByDescending(op => op.UpdatedUtc)
            .ToList();
    }

    private bool BelongsToProject(TransportOperation operation, ProjectConcordProjectId projectId)
    {
        var package = _relayOperational!.GetPackage(operation.SourcePackageId);
        return package is not null && package.Package.ProjectId == projectId;
    }
}
