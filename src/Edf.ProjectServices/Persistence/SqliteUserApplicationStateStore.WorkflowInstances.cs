using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore
{
    public void InsertWorkflowInstance(WorkflowInstance instance)
    {
        ArgumentNullException.ThrowIfNull(instance);
        EnsureProjectRegistered(instance.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO workflow_instance (
                workflow_instance_id,
                project_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                lifecycle_state,
                topology_place_id,
                traversal_occurrence_id,
                governed_baseline_kind,
                governed_baseline_value,
                creation_correlation_id,
                last_governed_transition_correlation_id,
                last_governed_transition_package_id,
                last_governed_transition_authority_reference,
                resource_version,
                created_utc,
                updated_utc)
            VALUES (
                $workflow_instance_id,
                $project_id,
                $prescribed_workflow_id,
                $definition_version,
                $profile_id,
                $lifecycle_state,
                $topology_place_id,
                $traversal_occurrence_id,
                $governed_baseline_kind,
                $governed_baseline_value,
                $creation_correlation_id,
                $last_governed_transition_correlation_id,
                $last_governed_transition_package_id,
                $last_governed_transition_authority_reference,
                $resource_version,
                $created_utc,
                $updated_utc);
            """);

        BindWorkflowInstanceParameters(command, instance);
        command.ExecuteNonQuery();
    }

    public WorkflowInstance? GetWorkflowInstance(WorkflowInstanceId instanceId)
    {
        using var command = CreateCommand(
            """
            SELECT
                workflow_instance_id,
                project_id,
                prescribed_workflow_id,
                definition_version,
                profile_id,
                lifecycle_state,
                topology_place_id,
                traversal_occurrence_id,
                governed_baseline_kind,
                governed_baseline_value,
                creation_correlation_id,
                last_governed_transition_correlation_id,
                last_governed_transition_package_id,
                last_governed_transition_authority_reference,
                resource_version,
                created_utc,
                updated_utc
            FROM workflow_instance
            WHERE workflow_instance_id = $workflow_instance_id
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$workflow_instance_id", instanceId.Value.ToString("D"));

        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadWorkflowInstance(reader) : null;
    }

    public IReadOnlyList<WorkflowInstance> ListWorkflowInstances(
        ProjectConcordProjectId projectId,
        WorkflowInstanceLifecycle? lifecycleFilter)
    {
        using var command = CreateCommand(
            lifecycleFilter is null
                ? """
                  SELECT
                      workflow_instance_id,
                      project_id,
                      prescribed_workflow_id,
                      definition_version,
                      profile_id,
                      lifecycle_state,
                      topology_place_id,
                      traversal_occurrence_id,
                      governed_baseline_kind,
                      governed_baseline_value,
                      creation_correlation_id,
                      last_governed_transition_correlation_id,
                      last_governed_transition_package_id,
                      last_governed_transition_authority_reference,
                      resource_version,
                      created_utc,
                      updated_utc
                  FROM workflow_instance
                  WHERE project_id = $project_id
                  ORDER BY created_utc ASC;
                  """
                : """
                  SELECT
                      workflow_instance_id,
                      project_id,
                      prescribed_workflow_id,
                      definition_version,
                      profile_id,
                      lifecycle_state,
                      topology_place_id,
                      traversal_occurrence_id,
                      governed_baseline_kind,
                      governed_baseline_value,
                      creation_correlation_id,
                      last_governed_transition_correlation_id,
                      last_governed_transition_package_id,
                      last_governed_transition_authority_reference,
                      resource_version,
                      created_utc,
                      updated_utc
                  FROM workflow_instance
                  WHERE project_id = $project_id AND lifecycle_state = $lifecycle_state
                  ORDER BY created_utc ASC;
                  """);

        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        if (lifecycleFilter is not null)
        {
            command.Parameters.AddWithValue("$lifecycle_state", (int)lifecycleFilter.Value);
        }

        var results = new List<WorkflowInstance>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadWorkflowInstance(reader));
        }

        return results;
    }

    public bool TryUpdateWorkflowInstanceWithExpectedVersion(
        WorkflowInstance instance,
        long expectedResourceVersion)
    {
        ArgumentNullException.ThrowIfNull(instance);
        EnsureProjectRegistered(instance.ProjectId);

        using var command = CreateCommand(
            """
            UPDATE workflow_instance SET
                project_id = $project_id,
                prescribed_workflow_id = $prescribed_workflow_id,
                definition_version = $definition_version,
                profile_id = $profile_id,
                lifecycle_state = $lifecycle_state,
                topology_place_id = $topology_place_id,
                traversal_occurrence_id = $traversal_occurrence_id,
                governed_baseline_kind = $governed_baseline_kind,
                governed_baseline_value = $governed_baseline_value,
                creation_correlation_id = $creation_correlation_id,
                last_governed_transition_correlation_id = $last_governed_transition_correlation_id,
                last_governed_transition_package_id = $last_governed_transition_package_id,
                last_governed_transition_authority_reference = $last_governed_transition_authority_reference,
                resource_version = $resource_version,
                created_utc = $created_utc,
                updated_utc = $updated_utc
            WHERE workflow_instance_id = $workflow_instance_id
              AND resource_version = $expected_resource_version;
            """);

        BindWorkflowInstanceParameters(command, instance);
        command.Parameters.AddWithValue("$expected_resource_version", expectedResourceVersion);
        return command.ExecuteNonQuery() == 1;
    }

    private static void BindWorkflowInstanceParameters(SqliteCommand command, WorkflowInstance instance)
    {
        command.Parameters.AddWithValue("$workflow_instance_id", instance.InstanceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", instance.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$prescribed_workflow_id", instance.WorkflowId.Value);
        command.Parameters.AddWithValue("$definition_version", instance.DefinitionVersion.Value);
        command.Parameters.AddWithValue(
            "$profile_id",
            instance.ProfileId is { IsEmpty: false } profile ? profile.Value : DBNull.Value);
        command.Parameters.AddWithValue("$lifecycle_state", (int)instance.Lifecycle);
        command.Parameters.AddWithValue("$topology_place_id", instance.TopologyPlaceId.Value);
        command.Parameters.AddWithValue("$traversal_occurrence_id", instance.TraversalOccurrenceId.Value.ToString("D"));
        command.Parameters.AddWithValue("$governed_baseline_kind", (int)instance.GovernedBaseline.Kind);
        command.Parameters.AddWithValue(
            "$governed_baseline_value",
            (object?)instance.GovernedBaseline.Value ?? DBNull.Value);
        command.Parameters.AddWithValue("$creation_correlation_id", instance.CreationCorrelationId.Value.ToString("D"));
        command.Parameters.AddWithValue(
            "$last_governed_transition_correlation_id",
            instance.LastGovernedTransitionCorrelationId is { } correlation
                ? correlation.Value.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue(
            "$last_governed_transition_package_id",
            instance.LastGovernedTransitionPackageId is { IsEmpty: false } package
                ? package.Value.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue(
            "$last_governed_transition_authority_reference",
            (object?)instance.LastGovernedTransitionAuthorityReference ?? DBNull.Value);
        command.Parameters.AddWithValue("$resource_version", instance.ResourceVersion);
        command.Parameters.AddWithValue("$created_utc", instance.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$updated_utc", instance.UpdatedUtc.ToString("O"));
    }

    private static WorkflowInstance ReadWorkflowInstance(SqliteDataReader reader)
    {
        var profileText = reader.IsDBNull(4) ? null : reader.GetString(4);
        WorkflowProfileId? profileId = string.IsNullOrWhiteSpace(profileText)
            ? null
            : WorkflowProfileId.Parse(profileText);

        var baselineKind = (GovernedBaselineKind)reader.GetInt32(8);
        var baselineValue = reader.IsDBNull(9) ? null : reader.GetString(9);

        var lastCorrelationText = reader.IsDBNull(11) ? null : reader.GetString(11);
        GovernedCorrelationId? lastCorrelation = string.IsNullOrWhiteSpace(lastCorrelationText)
            ? null
            : GovernedCorrelationId.Parse(lastCorrelationText);

        var lastPackageText = reader.IsDBNull(12) ? null : reader.GetString(12);
        GovernedPackageId? lastPackage = string.IsNullOrWhiteSpace(lastPackageText)
            ? null
            : GovernedPackageId.Parse(lastPackageText);

        var lastAuthorityReference = reader.IsDBNull(13) ? null : reader.GetString(13);

        return new WorkflowInstance(
            WorkflowInstanceId.Parse(reader.GetString(0)),
            ProjectConcordProjectId.Parse(reader.GetString(1)),
            PrescribedWorkflowId.Parse(reader.GetString(2)),
            WorkflowDefinitionVersion.Parse(reader.GetInt32(3)),
            profileId,
            (WorkflowInstanceLifecycle)reader.GetInt32(5),
            TopologyPlaceId.Parse(reader.GetString(6)),
            TraversalOccurrenceId.Parse(reader.GetString(7)),
            new GovernedBaselineReference(baselineKind, baselineValue),
            GovernedCorrelationId.Parse(reader.GetString(10)),
            lastCorrelation,
            lastPackage,
            lastAuthorityReference,
            reader.GetInt64(14),
            DateTimeOffset.Parse(reader.GetString(15)),
            DateTimeOffset.Parse(reader.GetString(16)));
    }
}
