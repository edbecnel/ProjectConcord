using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.ProjectServices.Persistence.Relay;
using Microsoft.Data.Sqlite;

namespace Edf.ProjectServices.Persistence;

public sealed partial class SqliteUserApplicationStateStore
{
    public RelaySessionContinuity GetRelaySessionContinuity(ProjectConcordProjectId projectId)
    {
        var pa = ReadContinuityRow(projectId, RelayAgentRole.ProjectArchitect);
        var ea = ReadContinuityRow(projectId, RelayAgentRole.EngineeringAgent);

        return new RelaySessionContinuity(
            pa.UserIntent,
            pa.Advisory,
            ea.UserIntent,
            ea.Advisory);
    }

    public void SetRelaySessionIntent(
        ProjectConcordProjectId projectId,
        RelayAgentRole agentRole,
        AgentSessionIntent? userIntent,
        DateTimeOffset updatedUtc)
    {
        EnsureProjectRegistered(projectId);
        var existing = ReadContinuityRow(projectId, agentRole);
        UpsertContinuityRow(projectId, agentRole, userIntent, existing.Advisory, updatedUtc);
    }

    public void SetRelaySessionAdvisory(
        ProjectConcordProjectId projectId,
        RelayAgentRole agentRole,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc)
    {
        EnsureProjectRegistered(projectId);
        var existing = ReadContinuityRow(projectId, agentRole);
        UpsertContinuityRow(projectId, agentRole, existing.UserIntent, advisory, updatedUtc);
    }

    public void UpsertRelayPackage(PersistedGovernedRelayPackage persisted)
    {
        ArgumentNullException.ThrowIfNull(persisted);
        EnsureProjectRegistered(persisted.Package.ProjectId);

        var package = persisted.Package;
        using var command = CreateCommand(
            """
            INSERT INTO relay_package (
                package_id, project_id, correlation_id, package_kind,
                schema_major, schema_minor, render_major, render_minor,
                created_utc, updated_utc, governance_critical_json, tier0_json,
                profile_payload, validation_state, validation_diagnostics_json,
                structural_agreement_json, rendered_body_hash)
            VALUES (
                $package_id, $project_id, $correlation_id, $package_kind,
                $schema_major, $schema_minor, $render_major, $render_minor,
                $created_utc, $updated_utc, $governance_critical_json, $tier0_json,
                $profile_payload, $validation_state, $validation_diagnostics_json,
                $structural_agreement_json, $rendered_body_hash)
            ON CONFLICT(package_id) DO UPDATE SET
                project_id = excluded.project_id,
                correlation_id = excluded.correlation_id,
                package_kind = excluded.package_kind,
                schema_major = excluded.schema_major,
                schema_minor = excluded.schema_minor,
                render_major = excluded.render_major,
                render_minor = excluded.render_minor,
                created_utc = excluded.created_utc,
                updated_utc = excluded.updated_utc,
                governance_critical_json = excluded.governance_critical_json,
                tier0_json = excluded.tier0_json,
                profile_payload = excluded.profile_payload,
                validation_state = excluded.validation_state,
                validation_diagnostics_json = excluded.validation_diagnostics_json,
                structural_agreement_json = excluded.structural_agreement_json,
                rendered_body_hash = excluded.rendered_body_hash;
            """);

        command.Parameters.AddWithValue("$package_id", package.PackageId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", package.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$correlation_id", package.CorrelationId.Value.ToString("D"));
        command.Parameters.AddWithValue("$package_kind", (int)package.Kind);
        command.Parameters.AddWithValue("$schema_major", package.SchemaVersion.Major);
        command.Parameters.AddWithValue("$schema_minor", package.SchemaVersion.Minor);
        if (package.RenderVersion is { } renderVersion)
        {
            command.Parameters.AddWithValue("$render_major", renderVersion.Major);
            command.Parameters.AddWithValue("$render_minor", renderVersion.Minor);
        }
        else
        {
            command.Parameters.AddWithValue("$render_major", DBNull.Value);
            command.Parameters.AddWithValue("$render_minor", DBNull.Value);
        }

        command.Parameters.AddWithValue("$created_utc", package.CreatedUtc.ToString("O"));
        command.Parameters.AddWithValue("$updated_utc", package.UpdatedUtc.ToString("O"));
        command.Parameters.AddWithValue(
            "$governance_critical_json",
            RelayPersistenceJson.SerializeGovernanceCritical(package.GovernanceCritical));
        command.Parameters.AddWithValue(
            "$tier0_json",
            (object?)RelayPersistenceJson.SerializeTier0(package.Tier0Snapshot) ?? DBNull.Value);
        command.Parameters.AddWithValue("$profile_payload", package.ProfilePayload.ToArray());
        command.Parameters.AddWithValue("$validation_state", (int)persisted.ValidationState);
        command.Parameters.AddWithValue(
            "$validation_diagnostics_json",
            RelayPersistenceJson.SerializeDiagnostics(persisted.ValidationDiagnostics));
        command.Parameters.AddWithValue(
            "$structural_agreement_json",
            RelayPersistenceJson.SerializeStructuralAgreement(package.StructuralAgreement));
        command.Parameters.AddWithValue(
            "$rendered_body_hash",
            (object?)persisted.RenderedBodyHash ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public PersistedGovernedRelayPackage? GetRelayPackage(GovernedPackageId packageId)
    {
        using var command = CreateCommand(
            """
            SELECT package_id, project_id, correlation_id, package_kind,
                   schema_major, schema_minor, render_major, render_minor,
                   created_utc, updated_utc, governance_critical_json, tier0_json,
                   profile_payload, validation_state, validation_diagnostics_json,
                   structural_agreement_json, rendered_body_hash
            FROM relay_package
            WHERE package_id = $package_id
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$package_id", packageId.Value.ToString("D"));
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadPersistedPackage(reader) : null;
    }

    public void AppendRelayProvenanceEvent(RelayProvenanceEvent provenanceEvent)
    {
        ArgumentNullException.ThrowIfNull(provenanceEvent);
        EnsureProjectRegistered(provenanceEvent.ProjectId);

        using var command = CreateCommand(
            """
            INSERT INTO relay_provenance_event (
                event_id, project_id, package_id, correlation_id,
                event_type, payload_json, recorded_utc)
            VALUES (
                $event_id, $project_id, $package_id, $correlation_id,
                $event_type, $payload_json, $recorded_utc);
            """);
        command.Parameters.AddWithValue("$event_id", provenanceEvent.EventId.Value.ToString("D"));
        command.Parameters.AddWithValue("$project_id", provenanceEvent.ProjectId.Value.ToString("D"));
        command.Parameters.AddWithValue(
            "$package_id",
            provenanceEvent.PackageId is { } packageId
                ? packageId.Value.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue(
            "$correlation_id",
            provenanceEvent.CorrelationId is { } correlationId
                ? correlationId.Value.ToString("D")
                : DBNull.Value);
        command.Parameters.AddWithValue("$event_type", (int)provenanceEvent.EventType);
        command.Parameters.AddWithValue("$payload_json", provenanceEvent.PayloadJson);
        command.Parameters.AddWithValue("$recorded_utc", provenanceEvent.RecordedUtc.ToString("O"));
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<RelayProvenanceEvent> ListRelayProvenanceEvents(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId? correlationId = null)
    {
        using var command = CreateCommand(
            correlationId is null
                ? """
                  SELECT event_id, project_id, package_id, correlation_id,
                         event_type, payload_json, recorded_utc
                  FROM relay_provenance_event
                  WHERE project_id = $project_id
                  ORDER BY recorded_utc ASC, event_id ASC;
                  """
                : """
                  SELECT event_id, project_id, package_id, correlation_id,
                         event_type, payload_json, recorded_utc
                  FROM relay_provenance_event
                  WHERE project_id = $project_id AND correlation_id = $correlation_id
                  ORDER BY recorded_utc ASC, event_id ASC;
                  """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        if (correlationId is not null)
        {
            command.Parameters.AddWithValue("$correlation_id", correlationId.Value.Value.ToString("D"));
        }

        using var reader = command.ExecuteReader();
        var events = new List<RelayProvenanceEvent>();
        while (reader.Read())
        {
            events.Add(ReadProvenanceEvent(reader));
        }

        return events;
    }

    private void EnsureProjectRegistered(ProjectConcordProjectId projectId)
    {
        if (GetById(projectId) is null)
        {
            throw new InvalidOperationException("Project is not registered.");
        }
    }

    private (AgentSessionIntent? UserIntent, AgentSessionAdvisory Advisory) ReadContinuityRow(
        ProjectConcordProjectId projectId,
        RelayAgentRole agentRole)
    {
        using var command = CreateCommand(
            """
            SELECT user_intent, advisory_json
            FROM relay_continuity
            WHERE project_id = $project_id AND agent_role = $agent_role
            LIMIT 1;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$agent_role", (int)agentRole);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return (null, AgentSessionAdvisory.None);
        }

        AgentSessionIntent? intent = reader.IsDBNull(0)
            ? null
            : (AgentSessionIntent)reader.GetInt32(0);
        var advisory = RelayPersistenceJson.DeserializeAdvisory(reader.GetString(1));
        return (intent, advisory);
    }

    private void UpsertContinuityRow(
        ProjectConcordProjectId projectId,
        RelayAgentRole agentRole,
        AgentSessionIntent? userIntent,
        AgentSessionAdvisory advisory,
        DateTimeOffset updatedUtc)
    {
        using var command = CreateCommand(
            """
            INSERT INTO relay_continuity (project_id, agent_role, user_intent, advisory_json, updated_utc)
            VALUES ($project_id, $agent_role, $user_intent, $advisory_json, $updated_utc)
            ON CONFLICT(project_id, agent_role) DO UPDATE SET
                user_intent = excluded.user_intent,
                advisory_json = excluded.advisory_json,
                updated_utc = excluded.updated_utc;
            """);
        command.Parameters.AddWithValue("$project_id", projectId.Value.ToString("D"));
        command.Parameters.AddWithValue("$agent_role", (int)agentRole);
        command.Parameters.AddWithValue(
            "$user_intent",
            userIntent is null ? DBNull.Value : (int)userIntent.Value);
        command.Parameters.AddWithValue("$advisory_json", RelayPersistenceJson.SerializeAdvisory(advisory));
        command.Parameters.AddWithValue("$updated_utc", updatedUtc.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static PersistedGovernedRelayPackage ReadPersistedPackage(SqliteDataReader reader)
    {
        var packageId = GovernedPackageId.Parse(reader.GetString(0));
        var projectId = ProjectConcordProjectId.Parse(reader.GetString(1));
        var correlationId = GovernedCorrelationId.Parse(reader.GetString(2));
        var kind = (GovernedPackageKind)reader.GetInt32(3);
        var schemaVersion = new RelaySchemaVersion(reader.GetInt32(4), reader.GetInt32(5));
        RelayRenderVersion? renderVersion = reader.IsDBNull(6) || reader.IsDBNull(7)
            ? null
            : new RelayRenderVersion(reader.GetInt32(6), reader.GetInt32(7));
        var createdUtc = DateTimeOffset.Parse(reader.GetString(8));
        var updatedUtc = DateTimeOffset.Parse(reader.GetString(9));
        var governance = RelayPersistenceJson.DeserializeGovernanceCritical(reader.GetString(10));
        var tier0 = RelayPersistenceJson.DeserializeTier0(reader.IsDBNull(11) ? null : reader.GetString(11));
        var profilePayload = reader.GetFieldValue<byte[]>(12);
        var validationState = (RelayValidationState)reader.GetInt32(13);
        var diagnostics = RelayPersistenceJson.DeserializeDiagnostics(reader.GetString(14));
        var structuralAgreement = RelayPersistenceJson.DeserializeStructuralAgreement(reader.GetString(15));
        var renderedBodyHash = reader.IsDBNull(16) ? null : reader.GetString(16);

        var package = new GovernedRelayPackage(
            packageId,
            correlationId,
            kind,
            schemaVersion,
            renderVersion,
            projectId,
            createdUtc,
            updatedUtc,
            governance,
            tier0,
            profilePayload,
            structuralAgreement);

        return new PersistedGovernedRelayPackage(package, validationState, diagnostics, renderedBodyHash);
    }

    private static RelayProvenanceEvent ReadProvenanceEvent(SqliteDataReader reader)
    {
        var eventId = RelayProvenanceEventId.Parse(reader.GetString(0));
        var projectId = ProjectConcordProjectId.Parse(reader.GetString(1));
        GovernedPackageId? packageId = reader.IsDBNull(2)
            ? null
            : GovernedPackageId.Parse(reader.GetString(2));
        GovernedCorrelationId? correlationId = reader.IsDBNull(3)
            ? null
            : GovernedCorrelationId.Parse(reader.GetString(3));
        var eventType = (RelayProvenanceEventType)reader.GetInt32(4);
        var payloadJson = reader.GetString(5);
        var recordedUtc = DateTimeOffset.Parse(reader.GetString(6));
        return new RelayProvenanceEvent(
            eventId,
            projectId,
            packageId,
            correlationId,
            eventType,
            payloadJson,
            recordedUtc);
    }
}
