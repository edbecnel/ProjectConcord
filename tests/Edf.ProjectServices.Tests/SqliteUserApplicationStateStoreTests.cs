using Edf.Application.Composition;
using Edf.Application.Projects;
using Edf.Application.Projects.Sqlite;
using Edf.Domain.Projects;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;
using Edf.ProjectServices.Persistence;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Tests;

public class SqliteUserApplicationStateStoreTests
{
    [Fact]
    public void FreshDatabase_CreatesCurrentSchemaVersion()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);

        Assert.Equal(2, ReadSchemaVersion(path));
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void RegisterAndResolve_PersistsManagedProject()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);
        var dir = Directory.CreateTempSubdirectory("edf-a1b-reg-");
        var locator = ProjectLocator.FromPath(dir.FullName);
        var openedUtc = DateTimeOffset.UtcNow;

        try
        {
            var registered = store.RegisterNewProjectAtLocator(locator, "demo", openedUtc);
            var resolved = store.ResolveByRegisteredLocator(locator);

            Assert.NotNull(resolved);
            Assert.Equal(registered.ProjectId, resolved!.ProjectId);
            Assert.Equal("demo", resolved.DisplayName);
            Assert.Equal(registered.CreatedUtc, resolved.CreatedUtc);
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void RecordSuccessfulOpen_UpdatesRecentOrderingAndLastOpened()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);
        var dirA = Directory.CreateTempSubdirectory("edf-a1b-a-");
        var dirB = Directory.CreateTempSubdirectory("edf-a1b-b-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var projectA = store.RegisterNewProjectAtLocator(ProjectLocator.FromPath(dirA.FullName), "a", utc);
            var projectB = store.RegisterNewProjectAtLocator(ProjectLocator.FromPath(dirB.FullName), "b", utc);
            var later = utc.AddMinutes(5);
            store.RecordSuccessfulOpen(projectA.ProjectId, later);

            var recent = store.ListRecent(null);
            Assert.Equal(2, recent.Count);
            Assert.Equal(projectA.ProjectId, recent[0].ProjectId);
            Assert.Equal(later, recent[0].LastOpenedUtc);
            Assert.Equal(projectB.ProjectId, recent[1].ProjectId);
        }
        finally
        {
            dirA.Delete();
            dirB.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void RemoveFromRecent_KeepsManagedProject()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);
        var dir = Directory.CreateTempSubdirectory("edf-a1b-remove-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var project = store.RegisterNewProjectAtLocator(ProjectLocator.FromPath(dir.FullName), "demo", utc);
            store.RemoveFromRecent(project.ProjectId);

            Assert.Empty(store.ListRecent(null));
            Assert.NotNull(store.GetById(project.ProjectId));
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void LastActivePreference_PersistsAcrossStoreInstances()
    {
        var path = CreateTempDatabasePath();
        var projectId = ProjectConcordProjectId.New();

        using (var store = new SqliteUserApplicationStateStore(path))
        {
            store.SetLastActiveProjectId(projectId);
        }

        using (var store = new SqliteUserApplicationStateStore(path))
        {
            Assert.Equal(projectId, store.GetLastActiveProjectId());
        }

        TryDelete(path);
    }

    [Fact]
    public void ReconcileLocator_PreservesProjectId()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);
        var original = Directory.CreateTempSubdirectory("edf-a1b-old-");
        var moved = Directory.CreateTempSubdirectory("edf-a1b-new-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var project = store.RegisterNewProjectAtLocator(ProjectLocator.FromPath(original.FullName), "demo", utc);
            original.Delete();

            var result = store.ReconcileProjectLocator(
                project.ProjectId,
                ProjectLocator.FromPath(moved.FullName),
                "moved",
                utc.AddHours(1));

            Assert.True(result.Success);
            Assert.Equal(project.ProjectId, result.Project!.ProjectId);
            Assert.Equal(moved.FullName, Path.GetFullPath(result.Project.RegisteredLocator.NormalizedAbsolutePath));
        }
        finally
        {
            moved.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void DuplicateRegisteredLocator_IsRejected()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);
        var dirA = Directory.CreateTempSubdirectory("edf-a1b-dup-a-");
        var dirB = Directory.CreateTempSubdirectory("edf-a1b-dup-b-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var locator = ProjectLocator.FromPath(dirA.FullName);
            store.RegisterNewProjectAtLocator(locator, "a", utc);
            var secondProject = ProjectConcordProjectId.New();

            var reconcile = store.ReconcileProjectLocator(secondProject, locator, "b", utc);
            Assert.False(reconcile.Success);

            Assert.Throws<InvalidOperationException>(() =>
                store.RegisterNewProjectAtLocator(locator, "c", utc));
        }
        finally
        {
            dirA.Delete();
            dirB.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void UnknownNewerSchemaVersion_Throws()
    {
        var path = CreateTempDatabasePath();
        using (var store = new SqliteUserApplicationStateStore(path))
        {
        }

        using (var connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();
            using var update = connection.CreateCommand();
            update.CommandText = "UPDATE schema_info SET version = 99;";
            update.ExecuteNonQuery();
        }

        Assert.Throws<UserApplicationStateSchemaException>(() => new SqliteUserApplicationStateStore(path));
        TryDelete(path);
    }

    [Fact]
    public void OpenProject_UsesTransactionalPersistence()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var service = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            new DegenerateAdministratorActor("test"),
            persistence,
            new LocalProjectRuntime());
        var dir = Directory.CreateTempSubdirectory("edf-a1b-open-");

        try
        {
            var result = service.OpenProjectRoot(dir.FullName);
            Assert.True(result.Success);

            using var store = new SqliteUserApplicationStateStore(path);
            Assert.NotNull(store.GetById(result.ProjectId!.Value));
            Assert.Equal(result.ProjectId, store.GetLastActiveProjectId());
            Assert.Single(store.ListRecent(store.GetLastActiveProjectId()));
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void DatabasePath_IsOutsideOpenedProjectDirectory()
    {
        var projectDir = Directory.CreateTempSubdirectory("edf-a1b-proj-");
        var dbPath = CreateTempDatabasePath();

        try
        {
            Assert.DoesNotContain(projectDir.FullName, dbPath, StringComparison.Ordinal);
            Assert.False(dbPath.StartsWith(projectDir.FullName, StringComparison.Ordinal));
        }
        finally
        {
            projectDir.Delete();
            TryDelete(dbPath);
        }
    }

    [Fact]
    public void SchemaV1_DoesNotPersistLocatorStatusOrRepositoryIdentity()
    {
        var path = CreateTempDatabasePath();
        using var store = new SqliteUserApplicationStateStore(path);

        var managedColumns = GetTableColumns(path, "managed_projects");
        Assert.DoesNotContain(managedColumns, c => c.Contains("locator_status", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(managedColumns, c => c.Contains("repository", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(managedColumns, c => c.Contains("fingerprint", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(managedColumns, c => c.Contains("hint", StringComparison.OrdinalIgnoreCase));

        TryDelete(path);
    }

    [Fact]
    public void OpenProject_DoesNotCreateProjectConcordFolder()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var service = new ProjectWorkspaceService(
            new ProjectRootResolver(),
            new DegenerateAdministratorActor("test"),
            persistence,
            new LocalProjectRuntime());
        var dir = Directory.CreateTempSubdirectory("edf-a1b-nopc-");

        try
        {
            var result = service.OpenProjectRoot(dir.FullName);
            Assert.True(result.Success);
            Assert.False(Directory.Exists(Path.Combine(dir.FullName, ".projectconcord")));
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void ApplicationAndDomain_Assemblies_DoNotReferenceSqlite()
    {
        var applicationReferences = typeof(ProjectWorkspaceService).Assembly
            .GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToHashSet();
        var domainReferences = typeof(ProjectConcordProjectId).Assembly
            .GetReferencedAssemblies()
            .Select(a => a.Name)
            .ToHashSet();

        Assert.DoesNotContain("Microsoft.Data.Sqlite", applicationReferences);
        Assert.DoesNotContain("Microsoft.Data.Sqlite", domainReferences);
    }

    private static string CreateTempDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"edf-a1b-{Guid.NewGuid():N}.db");

    private static int ReadSchemaVersion(string databasePath)
    {
        using var connection = new SqliteConnection($"Data Source={databasePath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT version FROM schema_info LIMIT 1;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static IReadOnlyList<string> GetTableColumns(string databasePath, string tableName)
    {
        using var connection = new SqliteConnection($"Data Source={databasePath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";
        using var reader = command.ExecuteReader();
        var columns = new List<string>();
        while (reader.Read())
        {
            columns.Add(reader.GetString(1));
        }

        return columns;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch (IOException)
        {
        }
    }
}
