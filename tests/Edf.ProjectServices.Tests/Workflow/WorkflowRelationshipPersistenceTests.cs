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

public class WorkflowRelationshipPersistenceTests
{
    [Fact]
    public void Migration005_UpgradesSchemaVersionFourDatabase()
    {
        var path = CreateTempDatabasePath();
        SeedSchemaVersionFour(path);

        using (var store = new SqliteUserApplicationStateStore(path))
        {
        }

        Assert.Equal(5, ReadSchemaVersion(path));
        Assert.True(TableExists(path, "workflow_origin"));
        Assert.True(TableExists(path, "workflow_dependency"));
        TryDelete(path);
    }

    [Fact]
    public void OriginAndDependency_PersistAndReload()
    {
        var path = CreateTempDatabasePath();
        var persistence = UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var projectId = RegisterProject(persistence);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var a = services.WorkflowInstances.CreateGewInstance(projectId, GovernedCorrelationId.New(), null);
        var b = services.WorkflowInstances.CreateGewInstance(projectId, GovernedCorrelationId.New(), null);
        services.WorkflowRelationships.RecordGovernedWorkflowOrigin(a.InstanceId, b.InstanceId, WorkflowOriginKind.SpawnedFollowOn, authority);
        services.WorkflowRelationships.RecordGovernedBlockingDependency(a.InstanceId, b.InstanceId, authority);

        using var secondStore = new SqliteUserApplicationStateStore(path);
        using var secondPersistence = new SqliteUserApplicationStatePersistence(secondStore);
        var recovery = WorkflowApplicationServicesFactory.Create(secondPersistence).WorkStateRecovery.RecoverForProject(projectId);

        Assert.Single(recovery.Origins);
        Assert.Single(recovery.Dependencies);
        TryDelete(path);
    }

    private static void SeedSchemaVersionFour(string path)
    {
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE schema_info (version INTEGER NOT NULL PRIMARY KEY);
            INSERT INTO schema_info (version) VALUES (4);
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
            """;
        command.ExecuteNonQuery();
    }

    private static ProjectConcordProjectId RegisterProject(IUserApplicationStatePersistence persistence)
    {
        var dir = Directory.CreateTempSubdirectory("edf-wf-rel-sql-");
        return persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(dir.FullName),
            "Workflow Rel",
            DateTimeOffset.UtcNow).ProjectId;
    }

    private static string CreateTempDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"edf-wf-rel-{Guid.NewGuid():N}.db");

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

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
        }
    }
}
