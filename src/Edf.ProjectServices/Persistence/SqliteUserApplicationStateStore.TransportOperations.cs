using Edf.ProjectServices.Persistence.Transport;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore
{
    public void UpsertTransportOperation(PersistedTransportOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        using var command = CreateCommand(
            """
            INSERT INTO transport_operation (
                transport_operation_id,
                source_package_id,
                correlation_id,
                provider_plugin_id,
                attempt,
                lifecycle_state,
                provider_session_hint,
                result_import_package_id,
                created_utc,
                updated_utc)
            VALUES (
                $transport_operation_id,
                $source_package_id,
                $correlation_id,
                $provider_plugin_id,
                $attempt,
                $lifecycle_state,
                $provider_session_hint,
                $result_import_package_id,
                $created_utc,
                $updated_utc)
            ON CONFLICT(transport_operation_id) DO UPDATE SET
                source_package_id = excluded.source_package_id,
                correlation_id = excluded.correlation_id,
                provider_plugin_id = excluded.provider_plugin_id,
                attempt = excluded.attempt,
                lifecycle_state = excluded.lifecycle_state,
                provider_session_hint = excluded.provider_session_hint,
                result_import_package_id = excluded.result_import_package_id,
                created_utc = excluded.created_utc,
                updated_utc = excluded.updated_utc;
            """);

        command.Parameters.AddWithValue("$transport_operation_id", operation.TransportOperationId.ToString("D"));
        command.Parameters.AddWithValue("$source_package_id", operation.SourcePackageId.ToString("D"));
        command.Parameters.AddWithValue("$correlation_id", operation.CorrelationId.ToString("D"));
        command.Parameters.AddWithValue("$provider_plugin_id", operation.ProviderPluginId);
        command.Parameters.AddWithValue("$attempt", operation.Attempt);
        command.Parameters.AddWithValue("$lifecycle_state", operation.LifecycleState);
        command.Parameters.AddWithValue(
            "$provider_session_hint",
            (object?)operation.ProviderSessionHintOpaque ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$result_import_package_id",
            operation.ResultImportPackageId is { } resultId
                ? resultId.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue("$created_utc", operation.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$updated_utc", operation.UpdatedUtc.ToString("O"));
        command.ExecuteNonQuery();
    }

    public PersistedTransportOperation? GetTransportOperation(Guid transportOperationId)
    {
        using var command = CreateCommand(
            """
            SELECT
                transport_operation_id,
                source_package_id,
                correlation_id,
                provider_plugin_id,
                attempt,
                lifecycle_state,
                provider_session_hint,
                result_import_package_id,
                created_utc,
                updated_utc
            FROM transport_operation
            WHERE transport_operation_id = $transport_operation_id
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$transport_operation_id", transportOperationId.ToString("D"));
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadPersistedTransportOperation(reader) : null;
    }

    public IReadOnlyList<PersistedTransportOperation> ListRecoverableTransportOperations(Guid projectId)
    {
        using var command = CreateCommand(
            """
            SELECT
                t.transport_operation_id,
                t.source_package_id,
                t.correlation_id,
                t.provider_plugin_id,
                t.attempt,
                t.lifecycle_state,
                t.provider_session_hint,
                t.result_import_package_id,
                t.created_utc,
                t.updated_utc
            FROM transport_operation t
            INNER JOIN relay_package p ON p.package_id = t.source_package_id
            WHERE p.project_id = $project_id
              AND t.lifecycle_state IN ($s0, $s1, $s2, $s3, $s4, $s5, $s6)
            ORDER BY t.updated_utc DESC;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.ToString("D"));
        command.Parameters.AddWithValue("$s0", 0); // CreatedNotForwarded
        command.Parameters.AddWithValue("$s1", 1); // ForwardInProgress
        command.Parameters.AddWithValue("$s2", 2); // ForwardAcknowledged
        command.Parameters.AddWithValue("$s3", 4); // AwaitingResult
        command.Parameters.AddWithValue("$s4", 5); // ResultCandidateReceived
        command.Parameters.AddWithValue("$s5", 10); // Ambiguous
        command.Parameters.AddWithValue("$s6", 9); // TimedOut

        var results = new List<PersistedTransportOperation>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadPersistedTransportOperation(reader));
        }

        return results;
    }

    private static PersistedTransportOperation ReadPersistedTransportOperation(SqliteDataReader reader)
    {
        var operationId = Guid.Parse(reader.GetString(0));
        var sourcePackageId = Guid.Parse(reader.GetString(1));
        var correlationId = Guid.Parse(reader.GetString(2));
        var providerPluginId = reader.GetString(3);
        var attempt = reader.GetInt32(4);
        var lifecycleState = reader.GetInt32(5);
        var sessionHint = reader.IsDBNull(6) ? null : reader.GetString(6);
        Guid? resultImportPackageId = reader.IsDBNull(7) ? null : Guid.Parse(reader.GetString(7));
        var createdUtc = DateTimeOffset.Parse(reader.GetString(8));
        var updatedUtc = DateTimeOffset.Parse(reader.GetString(9));

        return new PersistedTransportOperation(
            operationId,
            sourcePackageId,
            correlationId,
            providerPluginId,
            attempt,
            lifecycleState,
            sessionHint,
            resultImportPackageId,
            createdUtc,
            updatedUtc);
    }
}
