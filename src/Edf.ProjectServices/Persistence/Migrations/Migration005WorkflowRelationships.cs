using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration005WorkflowRelationships
{
    public const int Version = 5;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS workflow_origin (
                workflow_origin_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                source_workflow_instance_id TEXT NOT NULL,
                derived_workflow_instance_id TEXT NOT NULL,
                origin_kind INTEGER NOT NULL,
                creation_correlation_id TEXT NOT NULL,
                creation_package_id TEXT NULL,
                creation_authority_reference TEXT NULL,
                created_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id),
                FOREIGN KEY (source_workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id),
                FOREIGN KEY (derived_workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id),
                UNIQUE (source_workflow_instance_id, derived_workflow_instance_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_origin_project_id
                ON workflow_origin(project_id);
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_origin_source
                ON workflow_origin(source_workflow_instance_id);
            """,
            """
            CREATE TABLE IF NOT EXISTS workflow_dependency (
                workflow_dependency_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                blocked_workflow_instance_id TEXT NOT NULL,
                required_workflow_instance_id TEXT NOT NULL,
                dependency_kind INTEGER NOT NULL,
                satisfaction_condition INTEGER NOT NULL,
                status INTEGER NOT NULL,
                creation_correlation_id TEXT NOT NULL,
                creation_package_id TEXT NULL,
                creation_authority_reference TEXT NULL,
                satisfaction_correlation_id TEXT NULL,
                satisfaction_package_id TEXT NULL,
                satisfaction_authority_reference TEXT NULL,
                release_correlation_id TEXT NULL,
                release_package_id TEXT NULL,
                release_authority_reference TEXT NULL,
                resource_version INTEGER NOT NULL,
                created_utc TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                satisfied_utc TEXT NULL,
                released_utc TEXT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id),
                FOREIGN KEY (blocked_workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id),
                FOREIGN KEY (required_workflow_instance_id) REFERENCES workflow_instance(workflow_instance_id)
            );
            """,
            """
            CREATE UNIQUE INDEX IF NOT EXISTS ux_workflow_dependency_pending_pair
                ON workflow_dependency(blocked_workflow_instance_id, required_workflow_instance_id)
                WHERE status = 0;
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_dependency_project_status
                ON workflow_dependency(project_id, status);
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_dependency_blocked
                ON workflow_dependency(blocked_workflow_instance_id);
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_workflow_dependency_required
                ON workflow_dependency(required_workflow_instance_id);
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
