using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore
{
    public OperatorActiveWorkFocus? GetOperatorWorkFocus(ProjectConcordProjectId projectId)
    {
        lock (_sync)
        {
            var connection = GetConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT focus_id, project_id, workflow_instance_id, subject_label, subject_provenance,
                       governed_reference_key, resumption_target, last_continuity_summary,
                       last_continuity_package_id, resource_version, updated_utc
                FROM operator_work_focus
                WHERE project_id = $project_id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return ReadOperatorWorkFocusRecord(reader);
        }
    }

    public void UpsertOperatorWorkFocus(OperatorActiveWorkFocus record)
    {
        ArgumentNullException.ThrowIfNull(record);
        lock (_sync)
        {
            var connection = GetConnection();
            using var command = connection.CreateCommand();
            command.Transaction = _activeTransaction;
            command.CommandText = """
                INSERT INTO operator_work_focus (
                    project_id, focus_id, workflow_instance_id, subject_label, subject_provenance,
                    governed_reference_key, resumption_target, last_continuity_summary,
                    last_continuity_package_id, resource_version, updated_utc)
                VALUES (
                    $project_id, $focus_id, $workflow_instance_id, $subject_label, $subject_provenance,
                    $governed_reference_key, $resumption_target, $last_continuity_summary,
                    $last_continuity_package_id, $resource_version, $updated_utc)
                ON CONFLICT(project_id) DO UPDATE SET
                    focus_id = excluded.focus_id,
                    workflow_instance_id = excluded.workflow_instance_id,
                    subject_label = excluded.subject_label,
                    subject_provenance = excluded.subject_provenance,
                    governed_reference_key = excluded.governed_reference_key,
                    resumption_target = excluded.resumption_target,
                    last_continuity_summary = excluded.last_continuity_summary,
                    last_continuity_package_id = excluded.last_continuity_package_id,
                    resource_version = excluded.resource_version,
                    updated_utc = excluded.updated_utc;
                """;
            BindOperatorWorkFocus(command, record);
            command.ExecuteNonQuery();
        }
    }

    public bool TryUpdateOperatorWorkFocusWithExpectedVersion(
        OperatorActiveWorkFocus record,
        long expectedResourceVersion)
    {
        ArgumentNullException.ThrowIfNull(record);
        lock (_sync)
        {
            var connection = GetConnection();
            using var command = connection.CreateCommand();
            command.Transaction = _activeTransaction;
            command.CommandText = """
                UPDATE operator_work_focus SET
                    focus_id = $focus_id,
                    workflow_instance_id = $workflow_instance_id,
                    subject_label = $subject_label,
                    subject_provenance = $subject_provenance,
                    governed_reference_key = $governed_reference_key,
                    resumption_target = $resumption_target,
                    last_continuity_summary = $last_continuity_summary,
                    last_continuity_package_id = $last_continuity_package_id,
                    resource_version = $resource_version,
                    updated_utc = $updated_utc
                WHERE project_id = $project_id AND resource_version = $expected_version;
                """;
            BindOperatorWorkFocus(command, record);
            command.Parameters.AddWithValue("$expected_version", expectedResourceVersion);
            return command.ExecuteNonQuery() == 1;
        }
    }

    private static void BindOperatorWorkFocus(SqliteCommand command, OperatorActiveWorkFocus record)
    {
        command.Parameters.AddWithValue("$project_id", record.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$focus_id", record.FocusId.ToString("D"));
        command.Parameters.AddWithValue("$workflow_instance_id", record.WorkflowInstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$subject_label", record.SubjectLabel);
        command.Parameters.AddWithValue("$subject_provenance", (int)record.SubjectProvenance);
        command.Parameters.AddWithValue(
            "$governed_reference_key",
            (object?)record.GovernedReferenceKey ?? DBNull.Value);
        command.Parameters.AddWithValue("$resumption_target", (int)record.ResumptionTarget);
        command.Parameters.AddWithValue(
            "$last_continuity_summary",
            (object?)record.LastContinuitySummary ?? DBNull.Value);
        command.Parameters.AddWithValue(
            "$last_continuity_package_id",
            record.LastContinuityPackageId is { } packageId ? packageId.ToString("D") : DBNull.Value);
        command.Parameters.AddWithValue("$resource_version", record.ResourceVersion);
        command.Parameters.AddWithValue("$updated_utc", record.UpdatedUtc.UtcDateTime.ToString("O"));
    }

    private static OperatorActiveWorkFocus ReadOperatorWorkFocusRecord(SqliteDataReader reader)
    {
        var projectId = ProjectConcordProjectId.Parse(reader.GetString(1));
        var focusId = Guid.Parse(reader.GetString(0));
        var instanceId = WorkflowInstanceId.Parse(reader.GetString(2));
        var subjectLabel = reader.GetString(3);
        var provenance = (OperatorWorkFocusSubjectProvenance)reader.GetInt32(4);
        var governedKey = reader.IsDBNull(5) ? null : reader.GetString(5);
        var resumption = (OperatorWorkFocusResumptionTarget)reader.GetInt32(6);
        var continuitySummary = reader.IsDBNull(7) ? null : reader.GetString(7);
        Guid? continuityPackageId = reader.IsDBNull(8) ? null : Guid.Parse(reader.GetString(8));
        var resourceVersion = reader.GetInt64(9);
        var updatedUtc = DateTimeOffset.Parse(reader.GetString(10));
        return new OperatorActiveWorkFocus(
            focusId,
            projectId,
            instanceId,
            subjectLabel,
            provenance,
            governedKey,
            resumption,
            continuitySummary,
            continuityPackageId,
            resourceVersion,
            updatedUtc);
    }
}
