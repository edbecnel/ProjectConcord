using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore
{
    public void InsertDevelopmentWorkAuthorization(DevelopmentWorkAuthorization authorization)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        EnsureProjectRegistered(authorization.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO development_work_authorization (
                development_work_authorization_id,
                project_id,
                workflow_instance_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                grant_topology_place_id,
                grant_traversal_occurrence_id,
                authorization_kind,
                normalized_tranche_key,
                authorized_tranche_id,
                authorized_scope_markers_json,
                authority_reference,
                disposition,
                grant_correlation_id,
                grant_package_id,
                grant_authority_reference,
                granted_utc,
                supersession_correlation_id,
                supersession_package_id,
                supersession_authority_reference,
                superseded_utc,
                resource_version,
                created_utc,
                updated_utc)
            VALUES (
                $development_work_authorization_id,
                $project_id,
                $workflow_instance_id,
                $prescribed_workflow_id,
                $definition_version,
                $profile_id,
                $grant_topology_place_id,
                $grant_traversal_occurrence_id,
                $authorization_kind,
                $normalized_tranche_key,
                $authorized_tranche_id,
                $authorized_scope_markers_json,
                $authority_reference,
                $disposition,
                $grant_correlation_id,
                $grant_package_id,
                $grant_authority_reference,
                $granted_utc,
                $supersession_correlation_id,
                $supersession_package_id,
                $supersession_authority_reference,
                $superseded_utc,
                $resource_version,
                $created_utc,
                $updated_utc);
            """);

        BindDevelopmentWorkAuthorizationParameters(command, authorization);
        command.ExecuteNonQuery();
    }

    public DevelopmentWorkAuthorization? GetDevelopmentWorkAuthorization(DevelopmentWorkAuthorizationId authorizationId)
    {
        using var command = CreateCommand(
            """
            SELECT
                development_work_authorization_id,
                project_id,
                workflow_instance_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                grant_topology_place_id,
                grant_traversal_occurrence_id,
                authorization_kind,
                normalized_tranche_key,
                authorized_tranche_id,
                authorized_scope_markers_json,
                authority_reference,
                disposition,
                grant_correlation_id,
                grant_package_id,
                grant_authority_reference,
                granted_utc,
                supersession_correlation_id,
                supersession_package_id,
                supersession_authority_reference,
                superseded_utc,
                resource_version,
                created_utc,
                updated_utc
            FROM development_work_authorization
            WHERE development_work_authorization_id = $development_work_authorization_id;
            """);
        command.Parameters.AddWithValue(
            "$development_work_authorization_id",
            authorizationId.Value.ToString("D"));

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadDevelopmentWorkAuthorization(reader) : null;
    }

    public IReadOnlyList<DevelopmentWorkAuthorization> ListDevelopmentWorkAuthorizations(
        ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand(
            """
            SELECT
                development_work_authorization_id,
                project_id,
                workflow_instance_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                grant_topology_place_id,
                grant_traversal_occurrence_id,
                authorization_kind,
                normalized_tranche_key,
                authorized_tranche_id,
                authorized_scope_markers_json,
                authority_reference,
                disposition,
                grant_correlation_id,
                grant_package_id,
                grant_authority_reference,
                granted_utc,
                supersession_correlation_id,
                supersession_package_id,
                supersession_authority_reference,
                superseded_utc,
                resource_version,
                created_utc,
                updated_utc
            FROM development_work_authorization
            WHERE project_id = $project_id
            ORDER BY created_utc ASC;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));

        var results = new List<DevelopmentWorkAuthorization>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadDevelopmentWorkAuthorization(reader));
        }

        return results;
    }

    public IReadOnlyList<DevelopmentWorkAuthorization> ListDevelopmentWorkAuthorizationsByInstance(
        WorkflowInstanceId workflowInstanceId)
    {
        using var command = CreateCommand(
            """
            SELECT
                development_work_authorization_id,
                project_id,
                workflow_instance_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                grant_topology_place_id,
                grant_traversal_occurrence_id,
                authorization_kind,
                normalized_tranche_key,
                authorized_tranche_id,
                authorized_scope_markers_json,
                authority_reference,
                disposition,
                grant_correlation_id,
                grant_package_id,
                grant_authority_reference,
                granted_utc,
                supersession_correlation_id,
                supersession_package_id,
                supersession_authority_reference,
                superseded_utc,
                resource_version,
                created_utc,
                updated_utc
            FROM development_work_authorization
            WHERE workflow_instance_id = $workflow_instance_id
            ORDER BY created_utc ASC;
            """);
        command.Parameters.AddWithValue("$workflow_instance_id", workflowInstanceId.Value.ToString("D"));

        var results = new List<DevelopmentWorkAuthorization>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadDevelopmentWorkAuthorization(reader));
        }

        return results;
    }

    public DevelopmentWorkAuthorization? TryGetActiveDevelopmentWorkAuthorizationByContext(
        WorkflowInstanceId workflowInstanceId,
        DevelopmentWorkAuthorizationKind authorizationKind,
        string normalizedTrancheKey)
    {
        using var command = CreateCommand(
            """
            SELECT
                development_work_authorization_id,
                project_id,
                workflow_instance_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                grant_topology_place_id,
                grant_traversal_occurrence_id,
                authorization_kind,
                normalized_tranche_key,
                authorized_tranche_id,
                authorized_scope_markers_json,
                authority_reference,
                disposition,
                grant_correlation_id,
                grant_package_id,
                grant_authority_reference,
                granted_utc,
                supersession_correlation_id,
                supersession_package_id,
                supersession_authority_reference,
                superseded_utc,
                resource_version,
                created_utc,
                updated_utc
            FROM development_work_authorization
            WHERE workflow_instance_id = $workflow_instance_id
              AND authorization_kind = $authorization_kind
              AND normalized_tranche_key = $normalized_tranche_key
              AND disposition = $disposition
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$workflow_instance_id", workflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$authorization_kind", (int)authorizationKind);
        command.Parameters.AddWithValue("$normalized_tranche_key", normalizedTrancheKey);
        command.Parameters.AddWithValue("$disposition", (int)DevelopmentWorkAuthorizationDisposition.Active);

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadDevelopmentWorkAuthorization(reader) : null;
    }

    public bool TryUpdateDevelopmentWorkAuthorizationWithExpectedVersion(
        DevelopmentWorkAuthorization authorization,
        long expectedResourceVersion)
    {
        ArgumentNullException.ThrowIfNull(authorization);
        EnsureProjectRegistered(authorization.ProjectId);

        using var command = CreateCommand(
            """
            UPDATE development_work_authorization SET
                project_id = $project_id,
                workflow_instance_id = $workflow_instance_id,
                prescribed_workflow_id = $prescribed_workflow_id,
                definition_version = $definition_version,
                profile_id = $profile_id,
                grant_topology_place_id = $grant_topology_place_id,
                grant_traversal_occurrence_id = $grant_traversal_occurrence_id,
                authorization_kind = $authorization_kind,
                normalized_tranche_key = $normalized_tranche_key,
                authorized_tranche_id = $authorized_tranche_id,
                authorized_scope_markers_json = $authorized_scope_markers_json,
                authority_reference = $authority_reference,
                disposition = $disposition,
                grant_correlation_id = $grant_correlation_id,
                grant_package_id = $grant_package_id,
                grant_authority_reference = $grant_authority_reference,
                granted_utc = $granted_utc,
                supersession_correlation_id = $supersession_correlation_id,
                supersession_package_id = $supersession_package_id,
                supersession_authority_reference = $supersession_authority_reference,
                superseded_utc = $superseded_utc,
                resource_version = $resource_version,
                created_utc = $created_utc,
                updated_utc = $updated_utc
            WHERE development_work_authorization_id = $development_work_authorization_id
              AND resource_version = $expected_resource_version;
            """);

        BindDevelopmentWorkAuthorizationParameters(command, authorization);
        command.Parameters.AddWithValue("$expected_resource_version", expectedResourceVersion);
        return command.ExecuteNonQuery() == 1;
    }

    public void InsertWorkflowInstanceStopSummary(WorkflowInstanceStopSummary summary)
    {
        ArgumentNullException.ThrowIfNull(summary);
        EnsureProjectRegistered(summary.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO workflow_instance_stop_summary (
                workflow_instance_id,
                project_id,
                is_stop_active,
                last_event_id,
                resource_version,
                updated_utc)
            VALUES (
                $workflow_instance_id,
                $project_id,
                $is_stop_active,
                $last_event_id,
                $resource_version,
                $updated_utc);
            """);

        command.Parameters.AddWithValue("$workflow_instance_id", summary.WorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", summary.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$is_stop_active", summary.IsStopActive ? 1 : 0);
        command.Parameters.AddWithValue("$last_event_id", summary.LastEventId.Value.ToString("D"));
        command.Parameters.AddWithValue("$resource_version", summary.ResourceVersion);
        command.Parameters.AddWithValue("$updated_utc", summary.UpdatedUtc.ToString("O"));
        command.ExecuteNonQuery();
    }

    public void InsertWorkflowInstanceStopEvent(WorkflowInstanceStopEvent stopEvent)
    {
        ArgumentNullException.ThrowIfNull(stopEvent);
        EnsureProjectRegistered(stopEvent.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO workflow_instance_stop_event (
                event_id,
                workflow_instance_id,
                project_id,
                event_kind,
                correlation_id,
                package_id,
                authority_reference,
                occurred_utc)
            VALUES (
                $event_id,
                $workflow_instance_id,
                $project_id,
                $event_kind,
                $correlation_id,
                $package_id,
                $authority_reference,
                $occurred_utc);
            """);

        command.Parameters.AddWithValue("$event_id", stopEvent.EventId.Value.ToString("D"));
        command.Parameters.AddWithValue("$workflow_instance_id", stopEvent.WorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", stopEvent.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$event_kind", (int)stopEvent.EventKind);
        command.Parameters.AddWithValue("$correlation_id", stopEvent.CorrelationId.Value.ToString("D"));
        command.Parameters.AddWithValue(
            "$package_id",
            stopEvent.PackageId is { IsEmpty: false } package ? package.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$authority_reference",
            (object?)stopEvent.AuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue("$occurred_utc", stopEvent.OccurredUtc.ToString("O"));
        command.ExecuteNonQuery();
    }

    public WorkflowInstanceStopSummary? GetWorkflowInstanceStopSummary(WorkflowInstanceId workflowInstanceId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_instance_id,
                project_id,
                is_stop_active,
                last_event_id,
                resource_version,
                updated_utc
            FROM workflow_instance_stop_summary
            WHERE workflow_instance_id = $workflow_instance_id;
            """);
        command.Parameters.AddWithValue("$workflow_instance_id", workflowInstanceId.Value.ToString("D"));

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadWorkflowInstanceStopSummary(reader) : null;
    }

    public IReadOnlyList<WorkflowInstanceStopSummary> ListWorkflowInstanceStopSummaries(
        ProjectConcordProjectId projectId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_instance_id,
                project_id,
                is_stop_active,
                last_event_id,
                resource_version,
                updated_utc
            FROM workflow_instance_stop_summary
            WHERE project_id = $project_id;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));

        var results = new List<WorkflowInstanceStopSummary>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowInstanceStopSummary(reader));
        }

        return results;
    }

    public IReadOnlyList<WorkflowInstanceStopEvent> ListWorkflowInstanceStopEvents(
        WorkflowInstanceId workflowInstanceId)
    {
        using var command = CreateCommand(
            """
            SELECT
                event_id,
                workflow_instance_id,
                project_id,
                event_kind,
                correlation_id,
                package_id,
                authority_reference,
                occurred_utc
            FROM workflow_instance_stop_event
            WHERE workflow_instance_id = $workflow_instance_id
            ORDER BY occurred_utc ASC;
            """);
        command.Parameters.AddWithValue("$workflow_instance_id", workflowInstanceId.Value.ToString("D"));

        var results = new List<WorkflowInstanceStopEvent>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowInstanceStopEvent(reader));
        }

        return results;
    }

    public bool TryUpdateWorkflowInstanceStopSummaryWithExpectedVersion(
        WorkflowInstanceStopSummary summary,
        long expectedResourceVersion)
    {
        ArgumentNullException.ThrowIfNull(summary);
        EnsureProjectRegistered(summary.ProjectId);

        using var command = CreateCommand(
            """
            UPDATE workflow_instance_stop_summary SET
                project_id = $project_id,
                is_stop_active = $is_stop_active,
                last_event_id = $last_event_id,
                resource_version = $resource_version,
                updated_utc = $updated_utc
            WHERE workflow_instance_id = $workflow_instance_id
              AND resource_version = $expected_resource_version;
            """);

        command.Parameters.AddWithValue("$workflow_instance_id", summary.WorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", summary.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$is_stop_active", summary.IsStopActive ? 1 : 0);
        command.Parameters.AddWithValue("$last_event_id", summary.LastEventId.Value.ToString("D"));
        command.Parameters.AddWithValue("$resource_version", summary.ResourceVersion);
        command.Parameters.AddWithValue("$updated_utc", summary.UpdatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$expected_resource_version", expectedResourceVersion);
        return command.ExecuteNonQuery() == 1;
    }

    private static void BindDevelopmentWorkAuthorizationParameters(
        SqliteCommand command,
        DevelopmentWorkAuthorization authorization)
    {
        command.Parameters.AddWithValue(
            "$development_work_authorization_id",
            authorization.AuthorizationId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", authorization.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$workflow_instance_id", authorization.WorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$prescribed_workflow_id", authorization.PrescribedWorkflowId.Value);
        command.Parameters.AddWithValue("$definition_version", authorization.DefinitionVersion.Value);
        command.Parameters.AddWithValue(
            "$profile_id",
            authorization.ProfileId is { IsEmpty: false } profile ? profile.Value : DBNull.Value);
        command.Parameters.AddWithValue("$grant_topology_place_id", authorization.GrantTopologyPlaceId.Value);
        command.Parameters.AddWithValue(
            "$grant_traversal_occurrence_id",
            authorization.GrantTraversalOccurrenceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$authorization_kind", (int)authorization.AuthorizationKind);
        command.Parameters.AddWithValue("$normalized_tranche_key", authorization.NormalizedTrancheKey);
        command.Parameters.AddWithValue(
            "$authorized_tranche_id",
            (object?)authorization.AuthorizedTrancheId ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$authorized_scope_markers_json",
            (object?)authorization.AuthorizedScopeMarkersJson ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$authority_reference",
            (object?)authorization.AuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue("$disposition", (int)authorization.Disposition);
        command.Parameters.AddWithValue("$grant_correlation_id", authorization.GrantCorrelationId.Value.ToString("D"));
        command.Parameters.AddWithValue(
            "$grant_package_id",
            authorization.GrantPackageId is { IsEmpty: false } package ? package.Value.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue(
            "$grant_authority_reference",
            (object?)authorization.GrantAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue("$granted_utc", authorization.GrantedUtc.ToString("O"));
        command.Parameters.AddWithValue(
            "$supersession_correlation_id",
            authorization.SupersessionCorrelationId is { IsEmpty: false } correlation
                ? correlation.Value.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue(
            "$supersession_package_id",
            authorization.SupersessionPackageId is { IsEmpty: false } supersessionPackage
                ? supersessionPackage.Value.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue(
            "$supersession_authority_reference",
            (object?)authorization.SupersessionAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$superseded_utc",
            authorization.SupersededUtc is { } superseded ? superseded.ToString("O") : DBNull.Value);
        command.Parameters.AddWithValue("$resource_version", authorization.ResourceVersion);
        command.Parameters.AddWithValue("$created_utc", authorization.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$updated_utc", authorization.UpdatedUtc.ToString("O"));
    }

    private static DevelopmentWorkAuthorization ReadDevelopmentWorkAuthorization(SqliteDataReader reader) =>
        new(
            DevelopmentWorkAuthorizationId.Parse(reader.GetString(0)),
            ProjectConcordProjectId.Parse(reader.GetString(1)),
            WorkflowInstanceId.Parse(reader.GetString(2)),
            PrescribedWorkflowId.Parse(reader.GetString(3)),
            new WorkflowDefinitionVersion(reader.GetInt32(4)),
            reader.IsDBNull(5) ? null : WorkflowProfileId.Parse(reader.GetString(5)),
            TopologyPlaceId.Parse(reader.GetString(6)),
            TraversalOccurrenceId.Parse(reader.GetString(7)),
            (DevelopmentWorkAuthorizationKind)reader.GetInt32(8),
            reader.GetString(9),
            reader.IsDBNull(10) ? null : reader.GetString(10),
            reader.IsDBNull(11) ? null : reader.GetString(11),
            reader.IsDBNull(12) ? null : reader.GetString(12),
            (DevelopmentWorkAuthorizationDisposition)reader.GetInt32(13),
            GovernedCorrelationId.Parse(reader.GetString(14)),
            reader.IsDBNull(15) ? null : GovernedPackageId.Parse(reader.GetString(15)),
            reader.IsDBNull(16) ? null : reader.GetString(16),
            DateTimeOffset.Parse(reader.GetString(17)),
            reader.IsDBNull(18) ? null : GovernedCorrelationId.Parse(reader.GetString(18)),
            reader.IsDBNull(19) ? null : GovernedPackageId.Parse(reader.GetString(19)),
            reader.IsDBNull(20) ? null : reader.GetString(20),
            reader.IsDBNull(21) ? null : DateTimeOffset.Parse(reader.GetString(21)),
            reader.GetInt64(22),
            DateTimeOffset.Parse(reader.GetString(23)),
            DateTimeOffset.Parse(reader.GetString(24)));

    private static WorkflowInstanceStopSummary ReadWorkflowInstanceStopSummary(SqliteDataReader reader) =>
        new(
            WorkflowInstanceId.Parse(reader.GetString(0)),
            ProjectConcordProjectId.Parse(reader.GetString(1)),
            reader.GetInt32(2) != 0,
            WorkflowInstanceStopEventId.Parse(reader.GetString(3)),
            reader.GetInt64(4),
            DateTimeOffset.Parse(reader.GetString(5)));

    private static WorkflowInstanceStopEvent ReadWorkflowInstanceStopEvent(SqliteDataReader reader) =>
        new(
            WorkflowInstanceStopEventId.Parse(reader.GetString(0)),
            WorkflowInstanceId.Parse(reader.GetString(1)),
            ProjectConcordProjectId.Parse(reader.GetString(2)),
            (WorkflowInstanceStopEventKind)reader.GetInt32(3),
            GovernedCorrelationId.Parse(reader.GetString(4)),
            reader.IsDBNull(5) ? null : GovernedPackageId.Parse(reader.GetString(5)),
            reader.IsDBNull(6) ? null : reader.GetString(6),
            DateTimeOffset.Parse(reader.GetString(7)));
}
