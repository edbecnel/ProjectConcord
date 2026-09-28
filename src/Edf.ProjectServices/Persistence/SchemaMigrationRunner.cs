using Edf.ProjectServices.Persistence.Migrations;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

internal static class SchemaMigrationRunner
{
    public static void EnsureCurrentSchema(SqliteConnection connection)
    {
        var version = ReadSchemaVersion(connection);
        if (version is null)
        {
            Migration001Initial.Apply(connection, null);
            return;
        }

        if (version > SchemaVersions.Current)
        {
            throw new UserApplicationStateSchemaException(
                $"User application state database schema version {version} is newer than supported version {SchemaVersions.Current}.");
        }

        if (version < SchemaVersions.Current)
        {
            ApplyForwardMigrations(connection, version.Value);
        }
    }

    private static void ApplyForwardMigrations(SqliteConnection connection, int fromVersion)
    {
        if (fromVersion < Migration001Initial.Version)
        {
            Migration001Initial.Apply(connection, null);
        }
    }

    private static int? ReadSchemaVersion(SqliteConnection connection)
    {
        if (!TableExists(connection, "schema_info"))
        {
            return null;
        }

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT version FROM schema_info ORDER BY version DESC LIMIT 1;";
        var result = command.ExecuteScalar();
        if (result is null or DBNull)
        {
            return null;
        }

        return Convert.ToInt32(result);
    }

    private static bool TableExists(SqliteConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = $name LIMIT 1;";
        command.Parameters.AddWithValue("$name", tableName);
        return command.ExecuteScalar() is not null;
    }
}
