using Edf.Application.Composition;
using Edf.Application.Projects;
using Edf.Application.Projects.Sqlite;
using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.ProjectServices.Persistence;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Tests.Relay;

public class RelayOperationalPersistenceTests
{
    [Fact]
    public void Migration002_UpgradesSchemaVersionOneDatabase()
    {
        var path = CreateTempDatabasePath();
        using (var connection = new SqliteConnection($"Data Source={path}"))
        {
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE schema_info (version INTEGER NOT NULL PRIMARY KEY);
                INSERT INTO schema_info (version) VALUES (1);
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

        Assert.Equal(1, ReadSchemaVersion(path));

        using (var store = new SqliteUserApplicationStateStore(path))
        {
        }

        Assert.Equal(6, ReadSchemaVersion(path));
        Assert.True(TableExists(path, "relay_continuity"));
        Assert.True(TableExists(path, "relay_package"));
        Assert.True(TableExists(path, "relay_provenance_event"));
        TryDelete(path);
    }

    [Fact]
    public void Package_PersistsAndReloads_WithIdentityIntact()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var packageId = GovernedPackageId.New();
        var correlationId = GovernedCorrelationId.New();
        var original = BuildPackage(projectId, packageId, correlationId);

        relay.SavePackage(
            new PersistedGovernedRelayPackage(
                original,
                RelayValidationState.Valid,
                Array.Empty<RelayValidationDiagnostic>()));

        var reloaded = relay.GetPackage(packageId);
        Assert.NotNull(reloaded);
        Assert.Equal(original.PackageId, reloaded!.Package.PackageId);
        Assert.Equal(original.CorrelationId, reloaded.Package.CorrelationId);
        Assert.Equal(original.ProjectId, reloaded.Package.ProjectId);

        TryDelete(path);
    }

    [Fact]
    public void RelayState_IsPartitionedByProjectId()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var relay = persistence.RelayOperational;
        var dirA = Directory.CreateTempSubdirectory("edf-t3-a-");
        var dirB = Directory.CreateTempSubdirectory("edf-t3-b-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var projectA = persistence.ProjectRegistry.RegisterNewProjectAtLocator(ProjectLocator.FromPath(dirA.FullName), "a", utc);
            var projectB = persistence.ProjectRegistry.RegisterNewProjectAtLocator(ProjectLocator.FromPath(dirB.FullName), "b", utc);

            relay.SetProjectArchitectSessionIntent(projectA.ProjectId, AgentSessionIntent.New, utc);
            relay.SetProjectArchitectSessionIntent(projectB.ProjectId, AgentSessionIntent.Continue, utc);

            var continuityA = relay.GetSessionContinuity(projectA.ProjectId);
            var continuityB = relay.GetSessionContinuity(projectB.ProjectId);

            Assert.Equal(AgentSessionIntent.New, continuityA.ProjectArchitectSessionIntent);
            Assert.Equal(AgentSessionIntent.Continue, continuityB.ProjectArchitectSessionIntent);
        }
        finally
        {
            dirA.Delete();
            dirB.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void DistinctProjectIds_AtSameLocatorPath_DoNotCollapseRelayState()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var relay = persistence.RelayOperational;
        var dir = Directory.CreateTempSubdirectory("edf-t3-same-path-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var locator = ProjectLocator.FromPath(dir.FullName);
            var projectA = persistence.ProjectRegistry.RegisterNewProjectAtLocator(locator, "first", utc);
            persistence.ProjectRegistry.ReconcileProjectLocator(
                projectA.ProjectId,
                ProjectLocator.FromPath(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"))),
                "moved",
                utc);

            var projectB = persistence.ProjectRegistry.RegisterNewProjectAtLocator(locator, "second", utc.AddMinutes(1));

            relay.SetEngineeringAgentSessionIntent(projectA.ProjectId, AgentSessionIntent.New, utc);
            relay.SetEngineeringAgentSessionIntent(projectB.ProjectId, AgentSessionIntent.Continue, utc.AddMinutes(1));

            Assert.Equal(AgentSessionIntent.New, relay.GetSessionContinuity(projectA.ProjectId).EngineeringAgentSessionIntent);
            Assert.Equal(AgentSessionIntent.Continue, relay.GetSessionContinuity(projectB.ProjectId).EngineeringAgentSessionIntent);
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void LocatorRelocation_DoesNotRewriteProjectId_OnPersistedPackage()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var relay = persistence.RelayOperational;
        var originalDir = Directory.CreateTempSubdirectory("edf-t3-old-");
        var movedDir = Directory.CreateTempSubdirectory("edf-t3-new-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(ProjectLocator.FromPath(originalDir.FullName), "demo", utc);
            var packageId = GovernedPackageId.New();
            var package = BuildPackage(project.ProjectId, packageId, GovernedCorrelationId.New());
            relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, []));

            originalDir.Delete();
            persistence.ProjectRegistry.ReconcileProjectLocator(
                project.ProjectId,
                ProjectLocator.FromPath(movedDir.FullName),
                "moved",
                utc.AddHours(1));

            var reloaded = relay.GetPackage(packageId);
            Assert.NotNull(reloaded);
            Assert.Equal(project.ProjectId, reloaded!.Package.ProjectId);
        }
        finally
        {
            movedDir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void ValidationState_RoundTrips_WithoutReinterpretation()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var package = BuildPackage(projectId, GovernedPackageId.New(), GovernedCorrelationId.New());
        var diagnostics = new[]
        {
            new RelayValidationDiagnostic("test.incomplete", "missing", RelayValidationDiagnosticSeverity.Incomplete),
        };

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Incomplete, diagnostics));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.NotNull(reloaded);
        Assert.Equal(RelayValidationState.Incomplete, reloaded!.ValidationState);
        Assert.NotEqual(RelayValidationState.Valid, reloaded.ValidationState);
        Assert.NotEqual(RelayValidationState.RejectedMalformed, reloaded.ValidationState);
        Assert.Single(reloaded.ValidationDiagnostics);

        TryDelete(path);
    }

    [Fact]
    public void MissingGovernanceFields_RemainMissing_AfterRoundTrip()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode: null,
            PriorEngineeringAgentMode: null,
            ModeTransition: null,
            SessionContinuity: new RelaySessionContinuity(null, AgentSessionAdvisory.None, null, AgentSessionAdvisory.None),
            Stop: RelayStopMetadata.None,
            AuthorizationDispositionPresent: false,
            WorkContextPresent: false,
            DirectiveFlags: new RelayGovernanceDirectiveFlags(false, false),
            EdfCorrelation: null);
        var package = BuildPackage(projectId, GovernedPackageId.New(), GovernedCorrelationId.New(), governance);

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Incomplete, []));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.NotNull(reloaded);
        Assert.Null(reloaded!.Package.GovernanceCritical.EngineeringAgentMode);
        Assert.Null(reloaded.Package.GovernanceCritical.SessionContinuity.ProjectArchitectSessionIntent);
        Assert.False(reloaded.Package.GovernanceCritical.AuthorizationDispositionPresent);

        TryDelete(path);
    }

    [Fact]
    public void SessionIntentAndAdvisory_RoundTrip_ExplicitValues()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var utc = DateTimeOffset.UtcNow;

        relay.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.Continue, utc);
        relay.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.New, utc);
        relay.SetProjectArchitectSessionAdvisory(projectId, new AgentSessionAdvisory(AgentSessionAdvisoryKind.RecommendNew), utc);
        relay.SetEngineeringAgentSessionAdvisory(projectId, new AgentSessionAdvisory(AgentSessionAdvisoryKind.RecommendContinue), utc);

        var continuity = relay.GetSessionContinuity(projectId);
        Assert.Equal(AgentSessionIntent.Continue, continuity.ProjectArchitectSessionIntent);
        Assert.Equal(AgentSessionIntent.New, continuity.EngineeringAgentSessionIntent);
        Assert.Equal(AgentSessionAdvisoryKind.RecommendNew, continuity.ProjectArchitectSessionAdvisory.Kind);
        Assert.Equal(AgentSessionAdvisoryKind.RecommendContinue, continuity.EngineeringAgentSessionAdvisory.Kind);

        TryDelete(path);
    }

    [Fact]
    public void Advisory_DoesNotBecomeSessionIntent()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var utc = DateTimeOffset.UtcNow;

        relay.SetProjectArchitectSessionAdvisory(projectId, new AgentSessionAdvisory(AgentSessionAdvisoryKind.RecommendContinue), utc);

        var continuity = relay.GetSessionContinuity(projectId);
        Assert.Null(continuity.ProjectArchitectSessionIntent);
        Assert.Equal(AgentSessionAdvisoryKind.RecommendContinue, continuity.ProjectArchitectSessionAdvisory.Kind);

        TryDelete(path);
    }

    [Fact]
    public void StopState_RoundTrips_WithoutBecomingAuthorization()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var stop = new RelayStopMetadata(RelayStopState.Active, "STOP");
        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode.Plan,
            null,
            null,
            new RelaySessionContinuity(AgentSessionIntent.New, AgentSessionAdvisory.None, AgentSessionIntent.New, AgentSessionAdvisory.None),
            stop,
            AuthorizationDispositionPresent: false,
            WorkContextPresent: false,
            new RelayGovernanceDirectiveFlags(false, false),
            null);
        var package = BuildPackage(projectId, GovernedPackageId.New(), GovernedCorrelationId.New(), governance);

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Incomplete, []));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.NotNull(reloaded);
        Assert.Equal(RelayStopState.Active, reloaded!.Package.GovernanceCritical.Stop.State);
        Assert.False(reloaded.Package.GovernanceCritical.AuthorizationDispositionPresent);

        TryDelete(path);
    }

    [Fact]
    public void SoftwareDevelopmentProfilePayload_RoundTrips_OnPackage()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                SoftwareDevelopmentAuthorizationKind.Implementation,
                "A2-T4",
                ["scope"],
                "ref",
                false),
        };
        var bytes = SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload);
        var package = BuildPackage(projectId, GovernedPackageId.New(), GovernedCorrelationId.New())
            with { ProfilePayload = bytes };

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, []));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.NotNull(reloaded);
        Assert.True(
            SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                reloaded!.Package.ProfilePayload,
                out var roundTrip,
                out _));
        Assert.Equal("A2-T4", roundTrip!.DevelopmentWorkAuthorization!.AuthorizedTrancheId);

        TryDelete(path);
    }

    [Fact]
    public void CorrelationId_RoundTrips_OnPackage()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var correlationId = GovernedCorrelationId.New();
        var package = BuildPackage(projectId, GovernedPackageId.New(), correlationId);

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, []));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.Equal(correlationId, reloaded!.Package.CorrelationId);

        TryDelete(path);
    }

    [Fact]
    public void ProvenanceEvents_OrderDeterministically_ByRecordedUtcThenEventId()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var correlationId = GovernedCorrelationId.New();
        var packageId = GovernedPackageId.New();
        var t0 = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var t1 = t0.AddSeconds(1);
        relay.SavePackage(
            new PersistedGovernedRelayPackage(
                BuildPackage(projectId, packageId, correlationId),
                RelayValidationState.Valid,
                []));

        relay.AppendProvenanceEvent(new RelayProvenanceEvent(
            new RelayProvenanceEventId(Guid.Parse("00000000-0000-0000-0000-000000000002")),
            projectId,
            packageId,
            correlationId,
            RelayProvenanceEventType.ObservedContext,
            "{}",
            t1));
        relay.AppendProvenanceEvent(new RelayProvenanceEvent(
            new RelayProvenanceEventId(Guid.Parse("00000000-0000-0000-0000-000000000001")),
            projectId,
            packageId,
            correlationId,
            RelayProvenanceEventType.Advisory,
            "{}",
            t1));
        relay.AppendProvenanceEvent(new RelayProvenanceEvent(
            RelayProvenanceEventId.New(),
            projectId,
            packageId,
            correlationId,
            RelayProvenanceEventType.PackageProduced,
            "{}",
            t0));

        var events = relay.ListProvenanceEvents(projectId, correlationId);
        Assert.Equal(3, events.Count);
        Assert.Equal(RelayProvenanceEventType.PackageProduced, events[0].EventType);
        Assert.Equal(RelayProvenanceEventType.Advisory, events[1].EventType);
        Assert.Equal(RelayProvenanceEventType.ObservedContext, events[2].EventType);

        TryDelete(path);
    }

    [Fact]
    public void ProvenanceEvents_RemainAssociatedWithProjectPackageAndCorrelation()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var otherProjectDir = Directory.CreateTempSubdirectory("edf-t3-other-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
            var otherProject = persistence.ProjectRegistry.RegisterNewProjectAtLocator(ProjectLocator.FromPath(otherProjectDir.FullName), "other", utc);
            var packageId = GovernedPackageId.New();
            var correlationId = GovernedCorrelationId.New();
            relay.SavePackage(
                new PersistedGovernedRelayPackage(
                    BuildPackage(projectId, packageId, correlationId),
                    RelayValidationState.Valid,
                    []));

            relay.AppendProvenanceEvent(new RelayProvenanceEvent(
                RelayProvenanceEventId.New(),
                projectId,
                packageId,
                correlationId,
                RelayProvenanceEventType.PackageConsumed,
                "{}",
                utc));

            var otherPackageId = GovernedPackageId.New();
            var otherCorrelationId = GovernedCorrelationId.New();
            persistence.RelayOperational.SavePackage(
                new PersistedGovernedRelayPackage(
                    BuildPackage(otherProject.ProjectId, otherPackageId, otherCorrelationId),
                    RelayValidationState.Valid,
                    []));
            relay.AppendProvenanceEvent(new RelayProvenanceEvent(
                RelayProvenanceEventId.New(),
                otherProject.ProjectId,
                otherPackageId,
                otherCorrelationId,
                RelayProvenanceEventType.PackageConsumed,
                "{}",
                utc));

            var filtered = relay.ListProvenanceEvents(projectId, correlationId);
            Assert.Single(filtered);
            Assert.Equal(projectId, filtered[0].ProjectId);
            Assert.Equal(packageId, filtered[0].PackageId);
            Assert.Equal(correlationId, filtered[0].CorrelationId);
        }
        finally
        {
            otherProjectDir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void Tier0Snapshot_RoundTrips_WhenPresent()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var tier0 = new Tier0RelaySnapshot(
            "abc123",
            new[] { "PROJECT_INDEX.md" },
            new Dictionary<string, string> { ["status"] = "In progress" });
        var package = BuildPackage(projectId, GovernedPackageId.New(), GovernedCorrelationId.New()) with
        {
            Tier0Snapshot = tier0,
        };

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, []));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.NotNull(reloaded?.Package.Tier0Snapshot);
        Assert.Equal("abc123", reloaded!.Package.Tier0Snapshot!.GitHeadCommit);
        Assert.Contains("PROJECT_INDEX.md", reloaded.Package.Tier0Snapshot.KnownPathsPresent);
        Assert.Equal("In progress", reloaded.Package.Tier0Snapshot.NarrowMetadata["status"]);

        TryDelete(path);
    }

    [Fact]
    public void NonGitTier0State_CanPersist_EmptyGitHead()
    {
        var (path, projectId, relay) = CreateRelayHarness();
        var tier0 = new Tier0RelaySnapshot(null, Array.Empty<string>(), new Dictionary<string, string>());
        var package = BuildPackage(projectId, GovernedPackageId.New(), GovernedCorrelationId.New()) with
        {
            Tier0Snapshot = tier0,
        };

        relay.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, []));

        var reloaded = relay.GetPackage(package.PackageId);
        Assert.NotNull(reloaded?.Package.Tier0Snapshot);
        Assert.Null(reloaded!.Package.Tier0Snapshot!.GitHeadCommit);

        TryDelete(path);
    }

    [Fact]
    public void GovernedInteractionRelayService_RecordProducedPackage_CapturesTier0_AndEvents()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var dir = Directory.CreateTempSubdirectory("edf-t3-prod-");
        var utc = DateTimeOffset.Parse("2026-09-30T00:00:00Z");
        var clock = new TestClock(utc);

        try
        {
            var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
                ProjectLocator.FromPath(dir.FullName),
                "demo",
                utc);
            var relay = persistence.RelayOperational;
            var service = new GovernedInteractionRelayService(
                relay,
                new Tier0RelaySnapshotProvider(),
                clock);

            var package = BuildPackage(project.ProjectId, GovernedPackageId.New(), GovernedCorrelationId.New());
            var recorded = service.RecordProducedPackage(
                package,
                RelayValidationResult.Valid(),
                ProjectRoot.Create(dir.FullName));

            Assert.NotNull(recorded.Package.Tier0Snapshot);
            var events = service.ListProvenanceEvents(project.ProjectId, package.CorrelationId);
            Assert.Contains(events, e => e.EventType == RelayProvenanceEventType.ObservedContext);
            Assert.Contains(events, e => e.EventType == RelayProvenanceEventType.PackageProduced);
            Assert.DoesNotContain(events, e => e.PayloadJson.Contains("conversation", StringComparison.OrdinalIgnoreCase));
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    [Fact]
    public void Persistence_DoesNotCreateProjectConcordFolder_InProjectRoot()
    {
        var path = CreateTempDatabasePath();
        using var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var dir = Directory.CreateTempSubdirectory("edf-t3-nopc-");
        var utc = DateTimeOffset.UtcNow;

        try
        {
            var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
                ProjectLocator.FromPath(dir.FullName),
                "demo",
                utc);
            var package = BuildPackage(project.ProjectId, GovernedPackageId.New(), GovernedCorrelationId.New());
            persistence.RelayOperational.SavePackage(new PersistedGovernedRelayPackage(package, RelayValidationState.Valid, []));

            Assert.False(Directory.Exists(Path.Combine(dir.FullName, ".projectconcord")));
        }
        finally
        {
            dir.Delete();
            TryDelete(path);
        }
    }

    private static (string Path, ProjectConcordProjectId ProjectId, IRelayOperationalStore Relay) CreateRelayHarness()
    {
        var path = CreateTempDatabasePath();
        var persistence = (SqliteUserApplicationStatePersistence)UserApplicationStatePersistenceFactory.CreateSqliteAtPath(path);
        var dir = Directory.CreateTempSubdirectory("edf-t3-harness-");
        var utc = DateTimeOffset.UtcNow;
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(ProjectLocator.FromPath(dir.FullName), "demo", utc);
        return (path, project.ProjectId, persistence.RelayOperational);
    }

    private static GovernedRelayPackage BuildPackage(
        ProjectConcordProjectId projectId,
        GovernedPackageId packageId,
        GovernedCorrelationId correlationId,
        RelayGovernanceCriticalState? governance = null)
    {
        var utc = DateTimeOffset.UtcNow;
        governance ??= new RelayGovernanceCriticalState(
            EngineeringAgentMode.Plan,
            null,
            null,
            new RelaySessionContinuity(AgentSessionIntent.Continue, AgentSessionAdvisory.None, AgentSessionIntent.New, AgentSessionAdvisory.None),
            RelayStopMetadata.None,
            false,
            false,
            new RelayGovernanceDirectiveFlags(false, false),
            null);

        return new GovernedRelayPackage(
            packageId,
            correlationId,
            GovernedPackageKind.PaReviewExport,
            RelaySchemaVersion.Current,
            RelayRenderVersion.V1,
            projectId,
            utc,
            utc,
            governance,
            null,
            ReadOnlyMemory<byte>.Empty,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }

    private static string CreateTempDatabasePath() =>
        Path.Combine(Path.GetTempPath(), $"edf-t3-{Guid.NewGuid():N}.db");

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

    private sealed class TestClock(DateTimeOffset start) : TimeProvider
    {
        private DateTimeOffset _current = start;

        public override DateTimeOffset GetUtcNow() => _current;

        public void Advance(TimeSpan delta) => _current = _current.Add(delta);
    }
}
