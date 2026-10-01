using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration003TransportOperations
{
    public const int Version = 3;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS transport_operation (
                transport_operation_id TEXT NOT NULL PRIMARY KEY,
                source_package_id TEXT NOT NULL,
                correlation_id TEXT NOT NULL,
                provider_plugin_id TEXT NOT NULL,
                attempt INTEGER NOT NULL,
                lifecycle_state INTEGER NOT NULL,
                provider_session_hint TEXT NULL,
                result_import_package_id TEXT NULL,
                created_utc TEXT NOT NULL,
                updated_utc TEXT NOT NULL
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_transport_operation_source_package_id
                ON transport_operation(source_package_id);
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_transport_operation_correlation_id
                ON transport_operation(correlation_id);
            """,
            $"UPDATE schema_info SET version = {Version};",
        };

        foreach (var sql in commands)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
    }
}
