using Edf.Application.Composition;
using Edf.Application.Projects.Sqlite;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.ProjectServices.Persistence;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Tests.Transport;

public class TransportOperationPersistenceTests
{
    [Fact]
    public void Migration003_UpgradesSchemaVersionTwoDatabase()
    {
        var path = CreateTempDatabasePath();
        SeedSchemaVersionTwoDatabase(path);
        Assert.Equal(2, ReadSchemaVersion(path));

        using (var store = new SqliteUserApplicationStateStore(path))
        {
        }

        Assert.Equal(4, ReadSchemaVersion(path));
        Assert.True(TableExists(path, "transport_operation"));
        TryDelete(path);
    }

    [Fact]
    public void Migration003_ExistingUserPreferencesSurvive()
    {
        var path = CreateTempDatabasePath();
        SeedSchemaVersionTwoDatabase(path);
        using (var connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO user_preferences (key, value) VALUES ('test.pref.key', 'kept-value');";
            command.ExecuteNonQuery();
        }

        using var persistence = (SqliteUserApplicationStatePersistence)
            UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        Assert.Equal(4, ReadSchemaVersion(path));
        using (var connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT value FROM user_preferences WHERE key = 'test.pref.key';";
            Assert.Equal("kept-value", command.ExecuteScalar()?.ToString());
        }

        TryDelete(path);
    }

    [Fact]
    public void Get_MissingOperation_ReturnsNull()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        Assert.Null(persistence.TransportOperations.Get(TransportOperationId.New()));
        TryDelete(path);
    }

    [Fact]
    public void SaveAndGet_RoundTrip_AllFields()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var original = BuildSampleOperation(
            TransportOperationLifecycleState.ForwardAcknowledged,
            withSession: true,
            withResultImport: true);

        persistence.TransportOperations.Save(original);
        var loaded = persistence.TransportOperations.Get(original.OperationId);

        Assert.NotNull(loaded);
        Assert.Equal(original, loaded);
        TryDelete(path);
    }

    [Fact]
    public void Save_UpdatesSameTransportOperationId()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var id = TransportOperationId.New();
        var created = DateTimeOffset.Parse("2026-10-01T08:00:00Z");
        var first = BuildSampleOperation(
            id,
            TransportOperationLifecycleState.CreatedNotForwarded,
            attempt: 1,
            createdUtc: created,
            updatedUtc: created,
            withSession: false,
            withResultImport: false);
        persistence.TransportOperations.Save(first);

        var updated = first with
        {
            Attempt = 2,
            LifecycleState = TransportOperationLifecycleState.AwaitingResult,
            UpdatedUtc = created.AddMinutes(5),
        };
        persistence.TransportOperations.Save(updated);

        var loaded = persistence.TransportOperations.Get(id);
        Assert.NotNull(loaded);
        Assert.Equal(2, loaded!.Attempt);
        Assert.Equal(TransportOperationLifecycleState.AwaitingResult, loaded.LifecycleState);
        TryDelete(path);
    }

    [Fact]
    public void RepeatedSave_DoesNotCreateDuplicateRows()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var operation = BuildSampleOperation(TransportOperationLifecycleState.ForwardInProgress);
        persistence.TransportOperations.Save(operation);
        persistence.TransportOperations.Save(operation);

        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM transport_operation;";
        Assert.Equal(1L, command.ExecuteScalar());
        TryDelete(path);
    }

    [Fact]
    public void LifecycleState_Ambiguous_RoundTrips()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var operation = BuildSampleOperation(TransportOperationLifecycleState.Ambiguous);
        persistence.TransportOperations.Save(operation);
        var loaded = persistence.TransportOperations.Get(operation.OperationId);
        Assert.Equal(TransportOperationLifecycleState.Ambiguous, loaded!.LifecycleState);
        TryDelete(path);
    }

    [Fact]
    public void Identities_RemainDistinctOnRoundTrip()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var operation = BuildSampleOperation(TransportOperationLifecycleState.ResultCandidateReceived);
        persistence.TransportOperations.Save(operation);
        var loaded = persistence.TransportOperations.Get(operation.OperationId)!;

        Assert.False(TransportOperationIdentityRules.SharesSameGuidValue(
            loaded.OperationId,
            loaded.SourcePackageId));
        Assert.False(TransportOperationIdentityRules.SharesSameGuidValue(
            loaded.OperationId,
            loaded.CorrelationId));
        Assert.NotEqual(loaded.SourcePackageId.Value, loaded.CorrelationId.Value);
        TryDelete(path);
    }

    [Fact]
    public void ProviderSessionHandle_PersistsOpaqueValue_NotRedactedToString()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var opaque = "provider-internal-session-xyz";
        var operation = BuildSampleOperation(
            TransportOperationLifecycleState.ForwardAcknowledged,
            withSession: true,
            sessionOpaque: opaque);
        persistence.TransportOperations.Save(operation);

        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT provider_session_hint FROM transport_operation WHERE transport_operation_id = $id;";
        command.Parameters.AddWithValue("$id", operation.OperationId.Value.ToString("D"));
        Assert.Equal(opaque, command.ExecuteScalar()?.ToString());

        var loaded = persistence.TransportOperations.Get(operation.OperationId);
        Assert.Equal(opaque, loaded!.ProviderSessionHint!.Value.Value);
        Assert.Equal("(opaque provider session handle)", loaded.ProviderSessionHint.Value.ToString());
        TryDelete(path);
    }

    [Fact]
    public void ProviderSessionHandle_Null_RoundTrips()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var operation = BuildSampleOperation(
            TransportOperationLifecycleState.CreatedNotForwarded,
            withSession: false);
        persistence.TransportOperations.Save(operation);
        Assert.Null(persistence.TransportOperations.Get(operation.OperationId)!.ProviderSessionHint);
        TryDelete(path);
    }

    [Fact]
    public void Restart_Readback_ReconstructsOperation()
    {
        var path = CreateTempDatabasePath();
        var operationId = TransportOperationId.New();
        var expected = BuildSampleOperation(
            operationId,
            TransportOperationLifecycleState.ImportRejected,
            withSession: true,
            withResultImport: true);

        using (var persistence = CreatePersistence(path))
        {
            persistence.TransportOperations.Save(expected);
        }

        using (var reloadedPersistence = CreatePersistence(path))
        {
            var loaded = reloadedPersistence.TransportOperations.Get(operationId);
            Assert.NotNull(loaded);
            Assert.Equal(expected, loaded);
        }

        TryDelete(path);
    }

    [Fact]
    public void SqlitePersistence_ExposesTransportOperationStore()
    {
        var path = CreateTempDatabasePath();
        using var persistence = CreatePersistence(path);
        var operation = BuildSampleOperation(TransportOperationLifecycleState.Cancelled);
        persistence.TransportOperations.Save(operation);
        Assert.NotNull(persistence.TransportOperations.Get(operation.OperationId));
        TryDelete(path);
    }

    [Fact]
    public void DesktopComposition_ExposesTransportOperationStore()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var operation = BuildSampleOperation(TransportOperationLifecycleState.TimedOut);
        services.TransportOperations.Save(operation);
        Assert.NotNull(services.TransportOperations.Get(operation.OperationId));
    }

    private static SqliteUserApplicationStatePersistence CreatePersistence(string path) =>
        (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);

    private static TransportOperation BuildSampleOperation(
        TransportOperationLifecycleState lifecycle,
        bool withSession = true,
        bool withResultImport = false,
        string? sessionOpaque = "session-opaque-1") =>
        BuildSampleOperation(
            TransportOperationId.New(),
            lifecycle,
            attempt: 1,
            createdUtc: DateTimeOffset.Parse("2026-10-01T09:00:00Z"),
            updatedUtc: DateTimeOffset.Parse("2026-10-01T09:05:00Z"),
            withSession,
            withResultImport,
            sessionOpaque);

    private static TransportOperation BuildSampleOperation(
        TransportOperationId operationId,
        TransportOperationLifecycleState lifecycle,
        int attempt = 1,
        DateTimeOffset? createdUtc = null,
        DateTimeOffset? updatedUtc = null,
        bool withSession = true,
        bool withResultImport = false,
        string? sessionOpaque = "session-opaque-1")
    {
        var sourcePackageId = GovernedPackageId.New();
        var correlationId = GovernedCorrelationId.New();
        while (TransportOperationIdentityRules.SharesSameGuidValue(operationId, sourcePackageId))
        {
            sourcePackageId = GovernedPackageId.New();
        }

        return new TransportOperation(
            operationId,
            sourcePackageId,
            correlationId,
            EngineeringAgentProviderPluginId.Parse("test.provider.t2"),
            attempt,
            lifecycle,
            withSession && sessionOpaque is not null
                ? EngineeringAgentProviderSessionHandle.FromOpaque(sessionOpaque)
                : null,
            withResultImport ? GovernedPackageId.New() : null,
            createdUtc ?? DateTimeOffset.UtcNow,
            updatedUtc ?? DateTimeOffset.UtcNow);
    }

    private static void SeedSchemaVersionTwoDatabase(string path)
    {
        using var connection = new SqliteConnection($"Data Source={path}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE schema_info (version INTEGER NOT NULL PRIMARY KEY);
            INSERT INTO schema_info (version) VALUES (2);
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
            CREATE TABLE relay_continuity (
                project_id TEXT NOT NULL,
                agent_role INTEGER NOT NULL,
                user_intent INTEGER NULL,
                advisory_json TEXT NOT NULL,
                updated_utc TEXT NOT NULL,
                PRIMARY KEY (project_id, agent_role));
            CREATE TABLE relay_package (
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
                rendered_body_hash TEXT NULL);
            CREATE TABLE relay_provenance_event (
                event_id TEXT NOT NULL PRIMARY KEY,
                project_id TEXT NOT NULL,
                package_id TEXT NULL,
                correlation_id TEXT NULL,
                event_type INTEGER NOT NULL,
                payload_json TEXT NOT NULL,
                recorded_utc TEXT NOT NULL);
            """;
        command.ExecuteNonQuery();
    }

    private static string CreateTempDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"edf-t2-transport-{Guid.NewGuid():N}.db");

    private static int ReadSchemaVersion(string databasePath)
    {
        using var connection = new SqliteConnection($"Data Source={databasePath}");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT version FROM schema_info LIMIT 1;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static bool TableExists(string databasePath, string tableName)
    {
        using var connection = new SqliteConnection($"Data Source={databasePath}");
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
