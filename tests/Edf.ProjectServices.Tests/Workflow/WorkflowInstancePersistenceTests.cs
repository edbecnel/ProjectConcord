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

public class WorkflowInstancePersistenceTests
{
    [Fact]
    public void Migration004_UpgradesSchemaVersionThreeDatabase()
    {
        var path = CreateTempDatabasePath();
        using (var connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE schema_info (version INTEGER NOT NULL PRIMARY KEY);
                INSERT INTO schema_info (version) VALUES (3);
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
                """;
            command.ExecuteNonQuery();
        }

        Assert.Equal(3, ReadSchemaVersion(path));

        using (var store = new SqliteUserApplicationStateStore(path))
        {
        }

        Assert.Equal(6, ReadSchemaVersion(path));
        Assert.True(TableExists(path, "workflow_instance"));
        TryDelete(path);
    }

    [Fact]
    public void WorkflowInstance_PersistsAndReloads_WithIdentityIntact()
    {
        var path = CreateTempDatabasePath();
        var persistence = UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        if (persistence is IDisposable disposable)
        {
            using (disposable)
            {
                RunPersistenceRoundTrip(persistence, path);
            }
        }
        else
        {
            RunPersistenceRoundTrip(persistence, path);
        }

        TryDelete(path);
    }

    private static void RunPersistenceRoundTrip(IUserApplicationStatePersistence persistence, string path)
    {
        var projectId = RegisterProject(persistence, path);
        var workflow = WorkflowApplicationServicesFactory.Create(persistence).WorkflowInstances;
        var creationCorrelation = GovernedCorrelationId.New();

        var created = workflow.CreateGewInstance(projectId, creationCorrelation, projectRootAbsolutePath: null);
        var reloaded = persistence.WorkflowInstances.GetById(created.InstanceId);

        Assert.NotNull(reloaded);
        Assert.Equal(created.InstanceId, reloaded!.InstanceId);
        Assert.Equal(GewV1TopologyPlaces.Intake, reloaded.TopologyPlaceId.Value);
        Assert.Equal(creationCorrelation, reloaded.CreationCorrelationId);
        Assert.False(reloaded.TraversalOccurrenceId.IsEmpty);
        Assert.Equal(1, reloaded.ResourceVersion);

        var recovery = WorkflowApplicationServicesFactory.Create(persistence).WorkStateRecovery;
        var snapshot = recovery.RecoverForProject(projectId);
        Assert.Single(snapshot.ActiveInstances);

        using var secondStore = new SqliteUserApplicationStateStore(path);
        using var secondPersistence = new SqliteUserApplicationStatePersistence(secondStore);
        var rehydrated = secondPersistence.WorkflowInstances.GetById(created.InstanceId);
        Assert.NotNull(rehydrated);
    }

    private static ProjectConcordProjectId RegisterProject(IUserApplicationStatePersistence persistence, string dbPath)
    {
        var dir = Directory.CreateTempSubdirectory("edf-wf-inst-");
        var locator = ProjectLocator.FromPath(dir.FullName);
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            locator,
            "Workflow Test",
            DateTimeOffset.UtcNow);
        return project.ProjectId;
    }

    private static string CreateTempDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"edf-wf-{Guid.NewGuid():N}.db");

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
            // Best-effort temp cleanup.
        }
    }
}
