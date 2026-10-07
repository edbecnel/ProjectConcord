using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration007OperatorWorkFocus
{
    public const int Version = 7;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS operator_work_focus (
                project_id TEXT NOT NULL PRIMARY KEY,
                focus_id TEXT NOT NULL,
                workflow_instance_id TEXT NOT NULL,
                subject_label TEXT NOT NULL,
                subject_provenance INTEGER NOT NULL,
                governed_reference_key TEXT NULL,
                resumption_target INTEGER NOT NULL,
                last_continuity_summary TEXT NULL,
                last_continuity_package_id TEXT NULL,
                resource_version INTEGER NOT NULL,
                updated_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_operator_work_focus_instance
                ON operator_work_focus(workflow_instance_id);
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
