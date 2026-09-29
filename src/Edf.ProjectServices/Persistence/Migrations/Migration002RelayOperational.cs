using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence.Migrations;

internal static class Migration002RelayOperational
{
    public const int Version = 2;

    public static void Apply(SqliteConnection connection, SqliteTransaction? transaction)
    {
        var commands = new[]
        {
            """
            CREATE TABLE IF NOT EXISTS relay_continuity (
                project_id TEXT NOT NULL,
                agent_role INTEGER NOT NULL,
                user_intent INTEGER NULL,
                advisory_json TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                PRIMARY KEY (project_id, agent_role),
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id)
            );
            """,
            """
            CREATE TABLE IF NOT EXISTS relay_package (
                package_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                correlation_id TEXT NOT NULL,
                package_kind INTEGER NOT NULL,
                schema_major INTEGER NOT NULL,
                schema_minor INTEGER NOT NULL,
                render_major INTEGER NULL,
                render_minor INTEGER NULL,
                created_utc TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                governance_critical_json TEXT NOT NULL,
                tier0_json TEXT NULL,
                profile_payload BLOB NOT NULL,
                validation_state INTEGER NOT NULL,
                validation_diagnostics_json TEXT NOT NULL,
                structural_agreement_json TEXT NOT NULL,
                rendered_body_hash TEXT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_relay_package_project_id ON relay_package(project_id);
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_relay_package_correlation_id ON relay_package(correlation_id);
            """,
            """
            CREATE TABLE IF NOT EXISTS relay_provenance_event (
                event_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                package_id TEXT NULL,
                correlation_id TEXT NULL,
                event_type INTEGER NOT NULL,
                payload_json TEXT NOT NULL,
                recorded_utc TEXT NOT NULL,
                FOREIGN KEY (project_id) REFERENCES managed_projects(project_id),
                FOREIGN KEY (package_id) REFERENCES relay_package(package_id)
            );
            """,
            """
            CREATE INDEX IF NOT EXISTS ix_relay_provenance_project_recorded
                ON relay_provenance_event(project_id, recorded_utc, event_id);
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
