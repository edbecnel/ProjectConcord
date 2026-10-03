using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration004WorkflowInstances
{
    public const int Version = 4;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS workflow_instance (
                workflow_instance_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                prescribed_workflow_id TEXT NOT NULL,
                definition_version INTEGER NOT NULL,
                profile_id TEXT NULL,
                lifecycle_state INTEGER NOT NULL,
                topology_place_id TEXT NOT NULL,
                traversal_occurrence_id TEXT NOT NULL,
                governed_baseline_kind INTEGER NOT NULL,
                governed_baseline_value TEXT NULL,
                creation_correlation_id TEXT NOT NULL,
                last_governed_transition_correlation_id TEXT NULL,
                last_governed_transition_package_id TEXT NULL,
                last_governed_transition_authority_reference TEXT NULL,
                resource_version INTEGER NOT NULL,
                created_utc TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_instance_project_lifecycle
                ON workflow_instance(project_id, lifecycle_state);
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
