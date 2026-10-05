using Edf.Application.Composition;
using Edf.Application.Projects;
using Edf.Application.Projects.Sqlite;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Tests.Workflow;

public class WorkflowAuthorizationAndStopPersistenceTests
{
    [Fact]
    public void Migration006_UpgradesSchemaVersionFiveDatabase()
    {
        var path = CreateTempDatabasePath();
        SeedSchemaVersionFive(path);

        using (var store = new SqliteUserApplicationStateStore(path))
        {
        }

        Assert.Equal(6, ReadSchemaVersion(path));
        Assert.True(TableExists(path, "development_work_authorization"));
        Assert.True(TableExists(path, "workflow_instance_stop_summary"));
        Assert.True(TableExists(path, "workflow_instance_stop_event"));
        TryDelete(path);
    }

    [Fact]
    public void AuthorizationAndStop_PersistAndReload()
    {
        var path = CreateTempDatabasePath();
        var persistence = UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var projectId = RegisterProject(persistence);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var instance = services.WorkflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            "t1",
            new[] { "scope" },
            "ref",
            authority);

        var stop = services.WorkflowInstanceStops.RecordGovernedStopSet(instance.InstanceId, null, authority);

        using var secondStore = new SqliteUserApplicationStateStore(path);
        using var secondPersistence = new SqliteUserApplicationStatePersistence(secondStore);
        var recovery = WorkflowApplicationServicesFactory.Create(secondPersistence).WorkStateRecovery.RecoverForProject(projectId);

        Assert.Single(recovery.DevelopmentWorkAuthorizations);
        Assert.Single(recovery.WorkflowInstanceStopSummaries);
        Assert.True(recovery.WorkflowInstanceStopSummaries[0].IsStopActive);
        Assert.Equal(stop.ResourceVersion, recovery.WorkflowInstanceStopSummaries[0].ResourceVersion);
        TryDelete(path);
    }

    private static ProjectConcordProjectId RegisterProject(IUserApplicationStatePersistence persistence)
    {
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-sqlite-").FullName),
            "sqlite",
            DateTimeOffset.UtcNow);
        return project.ProjectId;
    }

    private static void SeedSchemaVersionFive(string path)
    {
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        SeedSchemaVersionFiveInline(connection);
    }

    private static void SeedSchemaVersionFiveInline(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE schema_info (version INTEGER NOT NULL PRIMARY KEY);
            INSERT INTO schema_info (version) VALUES (5);
            CREATE TABLE managed_projects (
                project_id TEXT NOT NULL PRIMARY KEY,
                display_name TEXT NOT NULL,
                locator_path TEXT NOT NULL UNIQUE,
                created_utc TEXT NOT NULL,
                last_opened_utc TEXT NOT NULL);
            CREATE TABLE recent_projects (
                project_id TEXT NOT NULL PRIMARY KEY,
                sort_order INTEGER NOT NULL);
            CREATE TABLE user_preferences (
                key TEXT NOT NULL PRIMARY KEY,
                value TEXT);
            CREATE TABLE workflow_instance (
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
                updated_utc TEXT NOT NULL);
            CREATE TABLE workflow_origin (
                workflow_origin_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                source_workflow_instance_id TEXT NOT NULL,
                derived_workflow_instance_id TEXT NOT NULL,
                origin_kind INTEGER NOT NULL,
                creation_correlation_id TEXT NOT NULL,
                creation_package_id TEXT NULL,
                creation_authority_reference TEXT NULL,
                created_utc TEXT NOT NULL);
            CREATE TABLE workflow_dependency (
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
                released_utc TEXT NULL);
            """;
        command.ExecuteNonQuery();
    }

    private static int ReadSchemaVersion(string path)
    {
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT version FROM schema_info ORDER BY version DESC LIMIT 1;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static bool TableExists(string path, string tableName)
    {
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM sqlite_master WHERE type = 'table' AND name = $name LIMIT 1;";
        command.Parameters.AddWithValue("$name", tableName);
        return command.ExecuteScalar() is not null;
    }

    private static string CreateTempDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"edf-wf-auth-stop-{Guid.NewGuid():N}.db");

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
            // Best-effort cleanup for temp databases.
        }
    }
}
