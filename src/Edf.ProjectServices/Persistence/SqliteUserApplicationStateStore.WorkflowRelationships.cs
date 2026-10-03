using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore
{
    public void InsertWorkflowOrigin(WorkflowOrigin origin)
    {
        ArgumentNullException.ThrowIfNull(origin);
        EnsureProjectRegistered(origin.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO workflow_origin (
                workflow_origin_id,
                project_id,
                source_workflow_instance_id,
                derived_workflow_instance_id,
                origin_kind,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                created_utc)
            VALUES (
                $workflow_origin_id,
                $project_id,
                $source_workflow_instance_id,
                $derived_workflow_instance_id,
                $origin_kind,
                $creation_correlation_id,
                $creation_package_id,
                $creation_authority_reference,
                $created_utc);
            """);

        command.Parameters.AddWithValue("$workflow_origin_id", origin.OriginId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", origin.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$source_workflow_instance_id", origin.SourceWorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$derived_workflow_instance_id", origin.DerivedWorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$origin_kind", (int)origin.OriginKind);
        command.Parameters.AddWithValue("$creation_correlation_id", origin.CreationCorrelationId.Value.ToString("D"));
        command.Parameters.AddWithValue(
            "$creation_package_id",
            origin.CreationPackageId is { IsEmpty: false } package ? package.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$creation_authority_reference",
            (object?)origin.CreationAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue("$created_utc", origin.CreatedUtc.ToString("O"));
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<WorkflowOrigin> ListWorkflowOrigins(ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_origin_id,
                project_id,
                source_workflow_instance_id,
                derived_workflow_instance_id,
                origin_kind,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                created_utc
            FROM workflow_origin
            WHERE project_id = $project_id
            ORDER BY created_utc ASC;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));

        var results = new List<WorkflowOrigin>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowOrigin(reader));
        }

        return results;
    }

    public void InsertWorkflowDependency(WorkflowDependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        EnsureProjectRegistered(dependency.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO workflow_dependency (
                workflow_dependency_id,
                project_id,
                blocked_workflow_instance_id,
                required_workflow_instance_id,
                dependency_kind,
                satisfaction_condition,
                status,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                satisfaction_correlation_id,
                satisfaction_package_id,
                satisfaction_authority_reference,
                release_correlation_id,
                release_package_id,
                release_authority_reference,
                resource_version,
                created_utc,
                updated_utc,
                satisfied_utc,
                released_utc)
            VALUES (
                $workflow_dependency_id,
                $project_id,
                $blocked_workflow_instance_id,
                $required_workflow_instance_id,
                $dependency_kind,
                $satisfaction_condition,
                $status,
                $creation_correlation_id,
                $creation_package_id,
                $creation_authority_reference,
                $satisfaction_correlation_id,
                $satisfaction_package_id,
                $satisfaction_authority_reference,
                $release_correlation_id,
                $release_package_id,
                $release_authority_reference,
                $resource_version,
                $created_utc,
                $updated_utc,
                $satisfied_utc,
                $released_utc);
            """);

        BindWorkflowDependencyParameters(command, dependency);
        command.ExecuteNonQuery();
    }

    public WorkflowDependency? GetWorkflowDependency(WorkflowDependencyId dependencyId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_dependency_id,
                project_id,
                blocked_workflow_instance_id,
                required_workflow_instance_id,
                dependency_kind,
                satisfaction_condition,
                status,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                satisfaction_correlation_id,
                satisfaction_package_id,
                satisfaction_authority_reference,
                release_correlation_id,
                release_package_id,
                release_authority_reference,
                resource_version,
                created_utc,
                updated_utc,
                satisfied_utc,
                released_utc
            FROM workflow_dependency
            WHERE workflow_dependency_id = $workflow_dependency_id
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$workflow_dependency_id", dependencyId.Value.ToString("D"));

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadWorkflowDependency(reader) : null;
    }

    public IReadOnlyList<WorkflowDependency> ListWorkflowDependencies(ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_dependency_id,
                project_id,
                blocked_workflow_instance_id,
                required_workflow_instance_id,
                dependency_kind,
                satisfaction_condition,
                status,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                satisfaction_correlation_id,
                satisfaction_package_id,
                satisfaction_authority_reference,
                release_correlation_id,
                release_package_id,
                release_authority_reference,
                resource_version,
                created_utc,
                updated_utc,
                satisfied_utc,
                released_utc
            FROM workflow_dependency
            WHERE project_id = $project_id
            ORDER BY created_utc ASC;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));

        var results = new List<WorkflowDependency>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowDependency(reader));
        }

        return results;
    }

    public IReadOnlyList<WorkflowDependency> ListPendingWorkflowDependencies(ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_dependency_id,
                project_id,
                blocked_workflow_instance_id,
                required_workflow_instance_id,
                dependency_kind,
                satisfaction_condition,
                status,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                satisfaction_correlation_id,
                satisfaction_package_id,
                satisfaction_authority_reference,
                release_correlation_id,
                release_package_id,
                release_authority_reference,
                resource_version,
                created_utc,
                updated_utc,
                satisfied_utc,
                released_utc
            FROM workflow_dependency
            WHERE project_id = $project_id AND status = $status
            ORDER BY created_utc ASC;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$status", (int)WorkflowDependencyStatus.Pending);

        var results = new List<WorkflowDependency>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowDependency(reader));
        }

        return results;
    }

    public IReadOnlyList<WorkflowDependency> ListPendingWorkflowDependenciesByBlocked(
        WorkflowInstanceId blockedInstanceId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_dependency_id,
                project_id,
                blocked_workflow_instance_id,
                required_workflow_instance_id,
                dependency_kind,
                satisfaction_condition,
                status,
                creation_correlation_id,
                creation_package_id,
                creation_authority_reference,
                satisfaction_correlation_id,
                satisfaction_package_id,
                satisfaction_authority_reference,
                release_correlation_id,
                release_package_id,
                release_authority_reference,
                resource_version,
                created_utc,
                updated_utc,
                satisfied_utc,
                released_utc
            FROM workflow_dependency
            WHERE blocked_workflow_instance_id = $blocked_workflow_instance_id
              AND status = $status
            ORDER BY created_utc ASC;
            """);
        command.Parameters.AddWithValue("$blocked_workflow_instance_id", blockedInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$status", (int)WorkflowDependencyStatus.Pending);

        var results = new List<WorkflowDependency>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowDependency(reader));
        }

        return results;
    }

    public bool TryUpdateWorkflowDependencyWithExpectedVersion(
        WorkflowDependency dependency,
        long expectedResourceVersion)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        EnsureProjectRegistered(dependency.ProjectId);

        using var command = CreateCommand(
            """
            UPDATE workflow_dependency SET
                project_id = $project_id,
                blocked_workflow_instance_id = $blocked_workflow_instance_id,
                required_workflow_instance_id = $required_workflow_instance_id,
                dependency_kind = $dependency_kind,
                satisfaction_condition = $satisfaction_condition,
                status = $status,
                creation_correlation_id = $creation_correlation_id,
                creation_package_id = $creation_package_id,
                creation_authority_reference = $creation_authority_reference,
                satisfaction_correlation_id = $satisfaction_correlation_id,
                satisfaction_package_id = $satisfaction_package_id,
                satisfaction_authority_reference = $satisfaction_authority_reference,
                release_correlation_id = $release_correlation_id,
                release_package_id = $release_package_id,
                release_authority_reference = $release_authority_reference,
                resource_version = $resource_version,
                created_utc = $created_utc,
                updated_utc = $updated_utc,
                satisfied_utc = $satisfied_utc,
                released_utc = $released_utc
            WHERE workflow_dependency_id = $workflow_dependency_id
              AND resource_version = $expected_resource_version;
            """);

        BindWorkflowDependencyParameters(command, dependency);
        command.Parameters.AddWithValue("$expected_resource_version", expectedResourceVersion);
        return command.ExecuteNonQuery() == 1;
    }

    private static void BindWorkflowDependencyParameters(SqliteCommand command, WorkflowDependency dependency)
    {
        command.Parameters.AddWithValue("$workflow_dependency_id", dependency.DependencyId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", dependency.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$blocked_workflow_instance_id", dependency.BlockedWorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$required_workflow_instance_id", dependency.RequiredWorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$dependency_kind", (int)dependency.DependencyKind);
        command.Parameters.AddWithValue("$satisfaction_condition", (int)dependency.SatisfactionCondition);
        command.Parameters.AddWithValue("$status", (int)dependency.Status);
        command.Parameters.AddWithValue("$creation_correlation_id", dependency.CreationCorrelationId.Value.ToString("D"));
        command.Parameters.AddWithValue(
            "$creation_package_id",
            dependency.CreationPackageId is { IsEmpty: false } cp ? cp.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$creation_authority_reference",
            (object?)dependency.CreationAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$satisfaction_correlation_id",
            dependency.SatisfactionCorrelationId is { } sc ? sc.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$satisfaction_package_id",
            dependency.SatisfactionPackageId is { IsEmpty: false } sp ? sp.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$satisfaction_authority_reference",
            (object?)dependency.SatisfactionAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$release_correlation_id",
            dependency.ReleaseCorrelationId is { } rc ? rc.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$release_package_id",
            dependency.ReleasePackageId is { IsEmpty: false } rp ? rp.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$release_authority_reference",
            (object?)dependency.ReleaseAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue("$resource_version", dependency.ResourceVersion);
        command.Parameters.AddWithValue("$created_utc", dependency.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$updated_utc", dependency.UpdatedUtc.ToString("O"));
        command.Parameters.AddWithValue(
            "$satisfied_utc",
            dependency.SatisfiedUtc is { } satisfied ? satisfied.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$released_utc",
            dependency.ReleasedUtc is { } released ? released.ToString("O") : DBNull.Value);
    }

    private static WorkflowOrigin ReadWorkflowOrigin(SqliteDataReader reader)
    {
        var packageText = reader.IsDBNull(6) ? null : reader.GetString(6);
        GovernedPackageId? packageId = string.IsNullOrWhiteSpace(packageText)
            ? null
            : GovernedPackageId.Parse(packageText);

        return new WorkflowOrigin(
            WorkflowOriginId.Parse(reader.GetString(0)),
            ProjectConcordProjectId.Parse(reader.GetString(1)),
            WorkflowInstanceId.Parse(reader.GetString(2)),
            WorkflowInstanceId.Parse(reader.GetString(3)),
            (WorkflowOriginKind)reader.GetInt32(4),
            GovernedCorrelationId.Parse(reader.GetString(5)),
            packageId,
            reader.IsDBNull(7) ? null : reader.GetString(7),
            DateTimeOffset.Parse(reader.GetString(8)));
    }

    private static WorkflowDependency ReadWorkflowDependency(SqliteDataReader reader)
    {
        GovernedCorrelationId? satisfactionCorrelation = ReadOptionalCorrelation(reader, 10);
        GovernedPackageId? satisfactionPackage = ReadOptionalPackage(reader, 11);
        GovernedCorrelationId? releaseCorrelation = ReadOptionalCorrelation(reader, 13);
        GovernedPackageId? releasePackage = ReadOptionalPackage(reader, 14);

        return new WorkflowDependency(
            WorkflowDependencyId.Parse(reader.GetString(0)),
            ProjectConcordProjectId.Parse(reader.GetString(1)),
            WorkflowInstanceId.Parse(reader.GetString(2)),
            WorkflowInstanceId.Parse(reader.GetString(3)),
            (WorkflowDependencyKind)reader.GetInt32(4),
            (WorkflowDependencySatisfactionCondition)reader.GetInt32(5),
            (WorkflowDependencyStatus)reader.GetInt32(6),
            GovernedCorrelationId.Parse(reader.GetString(7)),
            ReadOptionalPackage(reader, 8),
            reader.IsDBNull(9) ? null : reader.GetString(9),
            satisfactionCorrelation,
            satisfactionPackage,
            reader.IsDBNull(12) ? null : reader.GetString(12),
            releaseCorrelation,
            releasePackage,
            reader.IsDBNull(15) ? null : reader.GetString(15),
            reader.GetInt64(16),
            DateTimeOffset.Parse(reader.GetString(17)),
            DateTimeOffset.Parse(reader.GetString(18)),
            reader.IsDBNull(19) ? null : DateTimeOffset.Parse(reader.GetString(19)),
            reader.IsDBNull(20) ? null : DateTimeOffset.Parse(reader.GetString(20)));
    }

    private static GovernedCorrelationId? ReadOptionalCorrelation(SqliteDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
        {
            return null;
        }

        var text = reader.GetString(index);
        return string.IsNullOrWhiteSpace(text) ? null : GovernedCorrelationId.Parse(text);
    }

    private static GovernedPackageId? ReadOptionalPackage(SqliteDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
        {
            return null;
        }

        var text = reader.GetString(index);
        return string.IsNullOrWhiteSpace(text) ? null : GovernedPackageId.Parse(text);
    }
}
