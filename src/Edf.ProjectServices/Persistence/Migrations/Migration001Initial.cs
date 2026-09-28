using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration001Initial
{
    public const int Version = 1;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS schema_info (
                version INTEGER NOT NULL PRIMARY KEY
            );
            """,
            """
            CREATE TABLE IF NOT EXISTS managed_projects (
                project_id TEXT NOT NULL PRIMARY KEY,
                display_name TEXT NOT NULL,
                locator_path TEXT NOT NULL UNIQUE,
                created_utc TEXT NOT NULL,
                last_opened_utc TEXT NOT NULL
            );
            """,
            """
            CREATE TABLE IF NOT EXISTS recent_projects (
                project_id TEXT NOT NULL PRIMARY KEY,
                sort_order INTEGER NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id)
            );
            """,
            """
            CREATE TABLE IF NOT EXISTS user_preferences (
                key TEXT NOT NULL PRIMARY KEY,
                value TEXT
            );
            """,
            $"INSERT OR REPLACE INTO schema_info (version) VALUES ({Version});",
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
