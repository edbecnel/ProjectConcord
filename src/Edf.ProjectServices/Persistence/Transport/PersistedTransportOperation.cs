namespace Edf.ProjectServices.Persistence.Transport;

/// <summary>
/// SQLite row model for engineering-agent transport operations (ADR-0022). Not governed package content.
/// </summary>
public sealed record PersistedTransportOperation(
    Guid TransportOperationId,
    Guid SourcePackageId,
    Guid CorrelationId,
    string ProviderPluginId,
    int Attempt,
    int LifecycleState,
    string? ProviderSessionHintOpaque,
    Guid? ResultImportPackageId,
    DateTimeOffset CreatedUtc,
    DateTimeOffset UpdatedUtc);
