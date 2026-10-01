namespace Edf.Application.Projects.Sqlite;

using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;
using Edf.ProjectServices.Persistence;
using Edf.ProjectServices.Persistence.Transport;

internal sealed class SqliteTransportOperationStoreAdapter : ITransportOperationStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteTransportOperationStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public void Save(TransportOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        _store.UpsertTransportOperation(ToPersisted(operation));
    }

    public TransportOperation? Get(TransportOperationId operationId)
    {
        var persisted = _store.GetTransportOperation(operationId.Value);
        return persisted is null ? null : FromPersisted(persisted);
    }

    internal static PersistedTransportOperation ToPersisted(TransportOperation operation) =>
        new(
            operation.OperationId.Value,
            operation.SourcePackageId.Value,
            operation.CorrelationId.Value,
            operation.ProviderPluginId.Value,
            operation.Attempt,
            (int)operation.LifecycleState,
            operation.ProviderSessionHint?.Value,
            operation.ResultImportPackageId?.Value,
            operation.CreatedUtc,
            operation.UpdatedUtc);

    internal static TransportOperation FromPersisted(PersistedTransportOperation persisted) =>
        new(
            new TransportOperationId(persisted.TransportOperationId),
            new GovernedPackageId(persisted.SourcePackageId),
            new GovernedCorrelationId(persisted.CorrelationId),
            EngineeringAgentProviderPluginId.Parse(persisted.ProviderPluginId),
            persisted.Attempt,
            (TransportOperationLifecycleState)persisted.LifecycleState,
            persisted.ProviderSessionHintOpaque is null
                ? null
                : EngineeringAgentProviderSessionHandle.FromOpaque(persisted.ProviderSessionHintOpaque),
            persisted.ResultImportPackageId is null
                ? null
                : new GovernedPackageId(persisted.ResultImportPackageId.Value),
            persisted.CreatedUtc,
            persisted.UpdatedUtc);
}
