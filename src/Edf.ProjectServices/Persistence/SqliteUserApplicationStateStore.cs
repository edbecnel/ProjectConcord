using Edf.Domain.Projects;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore : IDisposable
{
    public const int DefaultMaxRecentProjects = 10;
    private const string LastActivePreferenceKey = "last_active_project_id";

    private readonly string _databasePath;
    private readonly int _maxRecentProjects;
    private readonly object _sync = new();
    private SqliteConnection? _connection;
    private SqliteTransaction? _activeTransaction;

    public SqliteUserApplicationStateStore(string databasePath, int maxRecentProjects = DefaultMaxRecentProjects)
    {
        if (maxRecentProjects < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxRecentProjects));
        }

        _databasePath = databasePath ?? throw new ArgumentNullException(nameof(databasePath));
        _maxRecentProjects = maxRecentProjects;
        EnsureSchema();
    }

    public string DatabasePath => _databasePath;

    public int MaxRecentProjects => _maxRecentProjects;

    public void ExecuteInTransaction(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        lock (_sync)
        {
            var connection = GetConnection();
            using var transaction = connection.BeginTransaction();
            _activeTransaction = transaction;
            try
            {
                work();
                transaction.Commit();
            }
            finally
            {
                _activeTransaction = null;
            }
        }
    }

    public ManagedProject RegisterNewProjectAtLocator(
        ProjectLocator locator,
        string displayName,
        DateTimeOffset openedUtc)
    {
        if (ResolveByRegisteredLocator(locator) is not null)
        {
            throw new InvalidOperationException("Locator is already registered to a project.");
        }

        var projectId = ProjectConcordProjectId.New();
        InsertManagedProject(projectId, displayName, locator, openedUtc, openedUtc);
        TouchRecent(projectId);
        return new ManagedProject(projectId, displayName, locator, openedUtc, openedUtc);
    }

    public ManagedProject? ResolveByRegisteredLocator(ProjectLocator locator)
    {
        using var command = CreateCommand(
            """
            SELECT project_id, display_name, locator_path, created_utc, last_opened_utc
            FROM managed_projects
            WHERE locator_path = $locator_path
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$locator_path", locator.NormalizedAbsolutePath);
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadManagedProject(reader) : null;
    }

    public ManagedProject? GetById(ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand(
            """
            SELECT project_id, display_name, locator_path, created_utc, last_opened_utc
            FROM managed_projects
            WHERE project_id = $project_id
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadManagedProject(reader) : null;
    }

    public IReadOnlyList<RecentProjectEntry> ListRecent(ProjectConcordProjectId? lastActiveProjectId)
    {
        using var command = CreateCommand(
            """
            SELECT mp.project_id, mp.display_name, mp.locator_path, mp.created_utc, mp.last_opened_utc
            FROM recent_projects rp
            INNER JOIN managed_projects mp ON mp.project_id = rp.project_id
            ORDER BY rp.sort_order ASC;
            """);
        using var reader = command.ExecuteReader();
        var entries = new List<RecentProjectEntry>();
        while (reader.Read())
        {
            var project = ReadManagedProject(reader);
            entries.Add(ToRecentEntry(project, lastActiveProjectId));
        }

        return entries;
    }

    public void RecordSuccessfulOpen(ProjectConcordProjectId projectId, DateTimeOffset openedUtc)
    {
        if (GetById(projectId) is null)
        {
            throw new InvalidOperationException("Project is not registered.");
        }

        using var command = CreateCommand(
            """
            UPDATE managed_projects
            SET last_opened_utc = $last_opened_utc
            WHERE project_id = $project_id;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$last_opened_utc", openedUtc.ToString("O"));
        if (command.ExecuteNonQuery() != 1)
        {
            throw new InvalidOperationException("Project is not registered.");
        }

        TouchRecent(projectId);
    }

    public void RemoveFromRecent(ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand("DELETE FROM recent_projects WHERE project_id = $project_id;");
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.ExecuteNonQuery();
    }

    public LocatorReconcileResult ReconcileProjectLocator(
        ProjectConcordProjectId projectId,
        ProjectLocator newLocator,
        string displayName,
        DateTimeOffset reconciledUtc)
    {
        var existing = GetById(projectId);
        if (existing is null)
        {
            return LocatorReconcileResult.Failed("Project was not found.");
        }

        var owner = ResolveByRegisteredLocator(newLocator);
        if (owner is not null && owner.ProjectId.Value != projectId.Value)
        {
            return LocatorReconcileResult.Failed("Locator is already registered to another project.");
        }

        using var command = CreateCommand(
            """
            UPDATE managed_projects
            SET display_name = $display_name,
                locator_path = $locator_path,
                last_opened_utc = $last_opened_utc
            WHERE project_id = $project_id;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$display_name", displayName);
        command.Parameters.AddWithValue("$locator_path", newLocator.NormalizedAbsolutePath);
        command.Parameters.AddWithValue("$last_opened_utc", reconciledUtc.ToString("O"));
        if (command.ExecuteNonQuery() != 1)
        {
            return LocatorReconcileResult.Failed("Project was not found.");
        }

        TouchRecent(projectId);
        var updated = new ManagedProject(projectId, displayName, newLocator, existing.CreatedUtc, reconciledUtc);
        return LocatorReconcileResult.Succeeded(updated);
    }

    public ProjectConcordProjectId? GetLastActiveProjectId()
    {
        using var command = CreateCommand(
            "SELECT value FROM user_preferences WHERE key = $key LIMIT 1;");
        command.Parameters.AddWithValue("$key", LastActivePreferenceKey);
        var value = command.ExecuteScalar() as string;
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return ProjectConcordProjectId.Parse(value);
    }

    public void SetLastActiveProjectId(ProjectConcordProjectId? projectId)
    {
        if (projectId is null)
        {
            using var delete = CreateCommand("DELETE FROM user_preferences WHERE key = $key;");
            delete.Parameters.AddWithValue("$key", LastActivePreferenceKey);
            delete.ExecuteNonQuery();
            return;
        }

        using var command = CreateCommand(
            """
            INSERT INTO user_preferences (key, value)
            VALUES ($key, $value)
            ON CONFLICT(key) DO UPDATE SET value = excluded.value;
            """);
        command.Parameters.AddWithValue("$key", LastActivePreferenceKey);
        command.Parameters.AddWithValue("$value", projectId!.Value.ToString());
        command.ExecuteNonQuery();
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _activeTransaction?.Dispose();
            _activeTransaction = null;
            _connection?.Dispose();
            _connection = null;
        }
    }

    private void EnsureSchema()
    {
        lock (_sync)
        {
            var connection = GetConnection();
            SchemaMigrationRunner.EnsureCurrentSchema(connection);
        }
    }

    private SqliteConnection GetConnection()
    {
        if (_connection is null)
        {
            var connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _databasePath,
            }.ToString();
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        return _connection;
    }

    private SqliteCommand CreateCommand(string sql)
    {
        var command = GetConnection().CreateCommand();
        command.CommandText = sql;
        command.Transaction = _activeTransaction;
        return command;
    }

    private void InsertManagedProject(
        ProjectConcordProjectId projectId,
        string displayName,
        ProjectLocator locator,
        DateTimeOffset createdUtc,
        DateTimeOffset lastOpenedUtc)
    {
        using var command = CreateCommand(
            """
            INSERT INTO managed_projects (project_id, display_name, locator_path, created_utc, last_opened_utc)
            VALUES ($project_id, $display_name, $locator_path, $created_utc, $last_opened_utc);
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$display_name", displayName);
        command.Parameters.AddWithValue("$locator_path", locator.NormalizedAbsolutePath);
        command.Parameters.AddWithValue("$created_utc", createdUtc.ToString("O"));
        command.Parameters.AddWithValue("$last_opened_utc", lastOpenedUtc.ToString("O"));
        try
        {
            command.ExecuteNonQuery();
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
        {
            throw new InvalidOperationException("Locator is already registered to a project.", ex);
        }
    }

    private void TouchRecent(ProjectConcordProjectId projectId)
    {
        using (var delete = CreateCommand("DELETE FROM recent_projects WHERE project_id = $project_id;"))
        {
            delete.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
            delete.ExecuteNonQuery();
        }

        using (var shift = CreateCommand("UPDATE recent_projects SET sort_order = sort_order + 1;"))
        {
            shift.ExecuteNonQuery();
        }

        using (var insert = CreateCommand(
                   """
                   INSERT INTO recent_projects (project_id, sort_order)
                   VALUES ($project_id, 0);
                   """))
        {
            insert.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
            insert.ExecuteNonQuery();
        }

        using var trim = CreateCommand("DELETE FROM recent_projects WHERE sort_order >= $max_recent;");
        trim.Parameters.AddWithValue("$max_recent", _maxRecentProjects);
        trim.ExecuteNonQuery();
    }

    private static ManagedProject ReadManagedProject(SqliteDataReader reader)
    {
        var projectId = ProjectConcordProjectId.Parse(reader.GetString(0));
        var displayName = reader.GetString(1);
        var locator = ProjectLocator.FromPath(reader.GetString(2));
        var createdUtc = DateTimeOffset.Parse(reader.GetString(3));
        var lastOpenedUtc = DateTimeOffset.Parse(reader.GetString(4));
        return new ManagedProject(projectId, displayName, locator, createdUtc, lastOpenedUtc);
    }

    private static RecentProjectEntry ToRecentEntry(ManagedProject project, ProjectConcordProjectId? lastActiveProjectId)
    {
        var availability = LocatorAvailabilityEvaluator.Evaluate(project.RegisteredLocator);
        var isLastActive = lastActiveProjectId is { } lastActive && lastActive.Value == project.ProjectId.Value;
        return new RecentProjectEntry(
            project.ProjectId,
            project.DisplayName,
            project.RegisteredLocator,
            availability,
            project.LastOpenedUtc,
            isLastActive);
    }
}
