using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration006WorkflowAuthorizationAndStop
{
    public const int Version = 6;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS development_work_authorization (
                development_work_authorization_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                workflow_instance_id TEXT NOT NULL,
                prescribed_workflow_id TEXT NOT NULL,
                definition_version INTEGER NOT NULL,
                profile_id TEXT NULL,
                grant_topology_place_id TEXT NOT NULL,
                grant_traversal_occurrence_id TEXT NOT NULL,
                authorization_kind INTEGER NOT NULL,
                normalized_tranche_key TEXT NOT NULL,
                authorized_tranche_id TEXT NULL,
                authorized_scope_markers_json TEXT NULL,
                authority_reference TEXT NULL,
                disposition INTEGER NOT NULL,
                grant_correlation_id TEXT NOT NULL,
                grant_package_id TEXT NULL,
                grant_authority_reference TEXT NULL,
                granted_utc TEXT NOT NULL,
                supersession_correlation_id TEXT NULL,
                supersession_package_id TEXT NULL,
                supersession_authority_reference TEXT NULL,
                superseded_utc TEXT NULL,
                resource_version INTEGER NOT NULL,
                created_utc TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id),
                FOREIGN KEY (workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_development_work_authorization_project
                ON development_work_authorization(project_id);
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_development_work_authorization_instance
                ON development_work_authorization(workflow_instance_id);
            """,
            """
            CREATE UNIQUE INDEX IF NOT EXISTS ux_development_work_authorization_active_context
                ON development_work_authorization(workflow_instance_id, authorization_kind, normalized_tranche_key)
                WHERE disposition = 0;
            """,
            """
            CREATE TABLE IF NOT EXISTS workflow_instance_stop_summary (
                workflow_instance_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                is_stop_active INTEGER NOT NULL,
                last_event_id TEXT NOT NULL,
                resource_version INTEGER NOT NULL,
                updated_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id),
                FOREIGN KEY (workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id)
            );
            """,
            """
            CREATE TABLE IF NOT EXISTS workflow_instance_stop_event (
                event_id TEXT NOT NULL PRIMARY KEY,
                workflow_instance_id TEXT NOT NULL,
                project_id TEXT NOT NULL,
                event_kind INTEGER NOT NULL,
                correlation_id TEXT NOT NULL,
                package_id TEXT NULL,
                authority_reference TEXT NULL,
                occurred_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id),
                FOREIGN KEY (workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_instance_stop_event_instance
                ON workflow_instance_stop_event(workflow_instance_id, occurred_utc);
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
