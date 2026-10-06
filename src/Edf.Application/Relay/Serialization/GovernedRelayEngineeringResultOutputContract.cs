namespace Edf.Application.Relay.Serialization;

using System.Text;
using System.Text.Json;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral, complete Engineering Result output contract for automated execution (A4-T6 remediation #3B).
/// Structural shape is generated from <see cref="GovernedRelayV1Renderer"/> and canonical relay v1 models.
/// </summary>
public static class GovernedRelayEngineeringResultOutputContract
{
    public const string SectionHeading = "## ProjectConcord Engineering Result Output Contract";

    /// <summary>Instructional placeholder — must be replaced by the Engineering Agent; invalid for import.</summary>
    public const string PlaceholderPackageId = "__REPLACE_WITH_NEW_PACKAGE_ID_UUID__";

    /// <summary>Instructional placeholder — must be replaced by the Engineering Agent; invalid for import.</summary>
    public const string PlaceholderUtcTimestamp = "__REPLACE_WITH_UTC_ISO8601_TIMESTAMP__";

    private static readonly JsonSerializerOptions TimestampFormatOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Renders field semantics, structural template (with placeholders), machine/projection agreement, output isolation,
    /// and authorization safety rules.
    /// </summary>
    public static string RenderCompleteContract(GovernedRelayPackage handoverExportPackage)
    {
        ArgumentNullException.ThrowIfNull(handoverExportPackage);

        var reporting = GovernedRelayEngineeringResultProfileDerivation.DeriveReportingRequirements(handoverExportPackage);
        var template = RenderStructuralTemplateWithPlaceholders(handoverExportPackage, reporting);
        var builder = new StringBuilder();
        builder.AppendLine(SectionHeading);
        builder.AppendLine();
        builder.AppendLine(
            "When you finish work for this handover, your **final message** for ProjectConcord MUST be **only** "
            + "one outer plain-text copy surface containing the **complete** governed relay document described below — not ordinary prose alone. "
            + "ProjectConcord validates all provider output as **untrusted** until `TryParseEngineeringResult` succeeds.");
        builder.AppendLine();
        AppendFieldSemantics(builder, handoverExportPackage, reporting);
        builder.AppendLine();
        builder.AppendLine("### Structural template (replace every placeholder)");
        builder.AppendLine();
        builder.AppendLine(
            "The block below shows the **exact single-copy shape** you must return (outer plain-text fence wrapping the full canonical relay document). "
            + "Values marked ProjectConcord-fixed must be copied exactly. "
            + "Replace every `__REPLACE_…__` placeholder with a real value. **Do not return placeholder text.**");
        builder.AppendLine();
        builder.AppendLine(template);
        builder.AppendLine();
        AppendMachineProjectionAgreement(builder);
        builder.AppendLine();
        AppendAuthorizationSafety(builder);
        builder.AppendLine();
        AppendOutputIsolation(builder);

        return builder.ToString().TrimEnd();
    }

    /// <summary>
    /// Canonical relay-v1 document shape with instructional placeholders (not importable as Valid).
    /// </summary>
    public static string RenderStructuralTemplateWithPlaceholders(GovernedRelayPackage handoverExportPackage)
    {
        var reporting = GovernedRelayEngineeringResultProfileDerivation.DeriveReportingRequirements(handoverExportPackage);
        return RenderStructuralTemplateWithPlaceholders(handoverExportPackage, reporting);
    }

    internal static string RenderStructuralTemplateWithPlaceholders(
        GovernedRelayPackage handoverExportPackage,
        GovernedRelayEngineeringResultReportingRequirements reporting)
    {
        var skeleton = CreateResultImportSkeleton(handoverExportPackage, useFreshIdentity: true, reporting);
        var rendered = new GovernedRelayV1Renderer().Render(skeleton);
        var withPlaceholders = ApplyInstructionalPlaceholders(rendered, skeleton);
        return GovernedRelayManualPasteCopyFence.WrapForManualCopy(withPlaceholders);
    }

    /// <summary>
    /// Fully completed example using the same skeleton rules as the instructional template (for automated validation).
    /// </summary>
    public static string RenderCompletedValidExample(GovernedRelayPackage handoverExportPackage)
    {
        var reporting = GovernedRelayEngineeringResultProfileDerivation.DeriveReportingRequirements(handoverExportPackage);
        var package = CreateResultImportSkeleton(handoverExportPackage, useFreshIdentity: true, reporting);
        return new GovernedRelayV1Renderer().Render(package);
    }

    internal static GovernedRelayPackage CreateResultImportSkeleton(
        GovernedRelayPackage handoverExportPackage,
        bool useFreshIdentity,
        GovernedRelayEngineeringResultReportingRequirements? reportingRequirements = null)
    {
        var reporting = reportingRequirements
            ?? GovernedRelayEngineeringResultProfileDerivation.DeriveReportingRequirements(handoverExportPackage);
        var gc = handoverExportPackage.GovernanceCritical;
        var resultGovernance = new RelayGovernanceCriticalState(
            gc.EngineeringAgentMode ?? EngineeringAgentMode.Plan,
            gc.PriorEngineeringAgentMode,
            gc.ModeTransition,
            gc.SessionContinuity,
            gc.Stop,
            reporting.AuthorizationDispositionPresent,
            reporting.WorkContextPresent,
            new RelayGovernanceDirectiveFlags(
                reporting.DirectsImplementationWork,
                reporting.DirectsTrancheWork),
            gc.EdfCorrelation);

        var profileBytes = BuildResultProfilePayload(handoverExportPackage, reporting);

        var now = DateTimeOffset.UtcNow;
        return new GovernedRelayPackage(
            useFreshIdentity ? GovernedPackageId.New() : handoverExportPackage.PackageId,
            handoverExportPackage.CorrelationId,
            GovernedPackageKind.EngineeringResultImport,
            handoverExportPackage.SchemaVersion,
            handoverExportPackage.RenderVersion ?? RelayRenderVersion.V1,
            handoverExportPackage.ProjectId,
            now,
            now,
            resultGovernance,
            Tier0RelaySnapshot.Empty,
            profileBytes,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }

    private static byte[] BuildResultProfilePayload(
        GovernedRelayPackage handoverExportPackage,
        GovernedRelayEngineeringResultReportingRequirements reporting)
    {
        if (!reporting.IsRich)
        {
            return SoftwareDevelopmentProfilePayloadSerializer.Serialize(SoftwareDevelopmentProfilePayloadSerializer.Empty);
        }

        SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
            handoverExportPackage.ProfilePayload,
            out var handoverPayload,
            out _);

        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty;
        if (reporting.AuthorizationDispositionPresent)
        {
            payload = payload with
            {
                AuthorizationDisposition = new AuthorizationDispositionProjection(
                    PlaceholderDispositionSummary,
                    false,
                    false,
                    null),
            };
        }

        if (reporting.WorkContextPresent)
        {
            var tranche = handoverPayload?.WorkContext?.RequestedTrancheId;
            payload = payload with
            {
                WorkContext = new WorkContextProjection(
                    tranche,
                    PlaceholderWorkContextSummary),
            };
        }

        return SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload);
    }

    /// <summary>Instructional placeholder for rich authorization disposition reporting.</summary>
    public const string PlaceholderDispositionSummary = "__REPLACE_AUTHORIZATION_DISPOSITION_SUMMARY__";

    /// <summary>Instructional placeholder for rich work-context reporting.</summary>
    public const string PlaceholderWorkContextSummary = "__REPLACE_WORK_CONTEXT_SUMMARY__";

    private static string ApplyInstructionalPlaceholders(string rendered, GovernedRelayPackage skeleton)
    {
        var packageId = skeleton.PackageId.Value.ToString("D");
        rendered = rendered.Replace(packageId, PlaceholderPackageId, StringComparison.OrdinalIgnoreCase);

        var created = FormatJsonTimestamp(skeleton.CreatedUtc);
        var updated = FormatJsonTimestamp(skeleton.UpdatedUtc);
        rendered = rendered.Replace(created, PlaceholderUtcTimestamp, StringComparison.Ordinal);
        if (!string.Equals(created, updated, StringComparison.Ordinal))
        {
            rendered = rendered.Replace(updated, PlaceholderUtcTimestamp, StringComparison.Ordinal);
        }

        return rendered.TrimEnd() + Environment.NewLine;
    }

    private static string FormatJsonTimestamp(DateTimeOffset value) =>
        JsonSerializer.Serialize(value, TimestampFormatOptions).Trim('"');

    private static void AppendFieldSemantics(
        StringBuilder builder,
        GovernedRelayPackage handover,
        GovernedRelayEngineeringResultReportingRequirements reporting)
    {
        var projectId = handover.ProjectId.Value;
        var correlationId = handover.CorrelationId.Value;
        var mode = handover.GovernanceCritical.EngineeringAgentMode ?? EngineeringAgentMode.Plan;
        var fence = GovernedRelayV1Format.MachineBlockFenceLanguage;

        builder.AppendLine("### Field semantics");
        builder.AppendLine();
        builder.AppendLine("| Field | Source | Required | Rules |");
        builder.AppendLine("| --- | --- | --- | --- |");
        builder.AppendLine(
            $"| Document first line `{GovernedRelayV1Format.RenderVersionLinePrefix} 1` | ProjectConcord-fixed | Yes | Exact line; required render version **1**. |");
        builder.AppendLine("| Blank line after render header | ProjectConcord-fixed | Yes | One empty line before the machine fence. |");
        builder.AppendLine(
            $"| Machine fence opening | ProjectConcord-fixed | Yes | Line must be exactly ` ```{fence}` . |");
        builder.AppendLine(
            $"| Machine fence language | ProjectConcord-fixed | Yes | Fence language token: `{fence}`. |");
        builder.AppendLine("| Machine JSON root object | EA-completed template | Yes | Complete JSON matching the structural template. |");
        builder.AppendLine(
            $"| `kind` | ProjectConcord-fixed | Yes | Must be `engineeringResultImport` (`{nameof(GovernedPackageKind.EngineeringResultImport)}`). |");
        builder.AppendLine(
            $"| `schemaVersionMajor` / `schemaVersionMinor` | ProjectConcord-fixed | Yes | Must be `{handover.SchemaVersion.Major}` / `{handover.SchemaVersion.Minor}`. |");
        builder.AppendLine(
            $"| `projectId` | ProjectConcord-fixed | Yes | Must be `{projectId}`. |");
        builder.AppendLine(
            $"| `correlationId` | ProjectConcord-fixed | Yes | Must be `{correlationId}`. |");
        builder.AppendLine(
            $"| `packageId` | EA-generated | Yes | New unique package id (UUID). Replace `{PlaceholderPackageId}`. |");
        builder.AppendLine(
            $"| `createdUtc` / `updatedUtc` | EA-generated | Yes | UTC ISO-8601 timestamps. Replace `{PlaceholderUtcTimestamp}` (both fields). |");
        builder.AppendLine(
            reporting.IsRich
                ? "| `governanceCritical` | Governance-constrained | Yes | Copy inherited session/mode/stop/EDF correlation from template; set disposition/work/directive flags exactly as the structural template requires for this reporting profile. |"
                : "| `governanceCritical` | Governance-constrained | Yes | Copy inherited session/mode/stop/EDF correlation from template; do **not** set authorization disposition or work context present to true; directive flags must remain false. |");
        builder.AppendLine(
            $"| `governanceCritical.engineeringAgentMode` | Governance-constrained | Yes | Must remain `{mode.ToString().ToLowerInvariant()}` unless reporting an actual mode change with transition fields. |");
        builder.AppendLine(
            "| `governanceCritical.sessionContinuity` | Governance-constrained | Yes | Reproduce template session intents exactly in machine JSON and projections. |");
        builder.AppendLine(
            "| `governanceCritical.stop` | Result-derived / constrained | Yes | Use template stop state unless work truly changes STOP; never remove an inherited STOP to pass validation. |");
        if (reporting.IsRich)
        {
            builder.AppendLine(
                $"| `governanceCritical.authorizationDispositionPresent` | ProjectConcord-fixed | Yes | Must be `{reporting.AuthorizationDispositionPresent.ToString().ToLowerInvariant()}`. |");
            builder.AppendLine(
                $"| `governanceCritical.workContextPresent` | ProjectConcord-fixed | Yes | Must be `{reporting.WorkContextPresent.ToString().ToLowerInvariant()}`. |");
            builder.AppendLine(
                $"| `governanceCritical.directiveFlags.directsImplementationWork` | ProjectConcord-fixed | Yes | Must be `{reporting.DirectsImplementationWork.ToString().ToLowerInvariant()}`. |");
            builder.AppendLine(
                $"| `governanceCritical.directiveFlags.directsTrancheWork` | ProjectConcord-fixed | Yes | Must be `{reporting.DirectsTrancheWork.ToString().ToLowerInvariant()}`. |");
        }
        else
        {
            builder.AppendLine(
                "| `governanceCritical.authorizationDispositionPresent` | ProjectConcord-fixed | Yes | Must be `false` for this contract (no new authorization). |");
            builder.AppendLine(
                "| `governanceCritical.workContextPresent` | ProjectConcord-fixed | Yes | Must be `false` unless substantive work context is present in profile (thin result uses false). |");
            builder.AppendLine(
                "| `governanceCritical.directiveFlags` | ProjectConcord-fixed | Yes | Both `directsImplementationWork` and `directsTrancheWork` must be `false`. |");
        }
        builder.AppendLine(
            "| `tier0Snapshot` | ProjectConcord-fixed | Yes | Use template empty Tier-0 snapshot (`knownPathsPresent`: [], `narrowMetadata`: {}). |");
        builder.AppendLine(
            reporting.IsRich
                ? "| `softwareDevelopmentProfile` | EA-completed template | Yes | Complete required profile sections shown in the structural template; do not add DevelopmentWorkAuthorization unless explicitly required by the template. |"
                : "| `softwareDevelopmentProfile` | ProjectConcord-fixed | Yes | `payloadVersion`: 1 only for thin engineering result (no authorization disposition / work context blocks). |");
        builder.AppendLine(
            "| Governance projection headings and lines | EA-completed template | Yes | Must match machine JSON (see agreement section). |");
        builder.AppendLine(
            "| Trailing PA reminder line | ProjectConcord-fixed | Yes | Include exactly as shown in template when present. |");
        builder.AppendLine();
        builder.AppendLine(
            "**Enumerated — Engineering Agent mode (machine JSON):** `plan`, `agent` (camelCase). "
            + "**Projections:** `PLAN`, `AGENT`.");
        builder.AppendLine(
            "**Enumerated — session intent (machine JSON):** `new`, `continue`. "
            + "**Projections:** `NEW`, `CONTINUE`.");
        builder.AppendLine(
            "**Enumerated — stop state (machine JSON):** `none`, `stop`, `ambiguous`. "
            + "**Projections:** `None`, `Stop`, `Ambiguous` (see template).");
    }

    private static void AppendMachineProjectionAgreement(StringBuilder builder)
    {
        builder.AppendLine("### Machine / projection agreement");
        builder.AppendLine();
        builder.AppendLine("Human-readable governance projections are **not** independent commentary. "
            + "Where the template lists projection lines, values MUST match the machine JSON:");
        builder.AppendLine("- Engineering-Agent-Mode ↔ `governanceCritical.engineeringAgentMode`");
        builder.AppendLine("- Engineering-Agent-Chat ↔ `governanceCritical.sessionContinuity.engineeringAgentSessionIntent`");
        builder.AppendLine("- ChatGPT-Chat ↔ `governanceCritical.sessionContinuity.projectArchitectSessionIntent`");
        builder.AppendLine("- Engineering-Agent-Mode-Transition (if present) ↔ `governanceCritical.modeTransition`");
        builder.AppendLine("- Authorization-Disposition-Present / Work-Context-Present / directive flags ↔ same fields in machine JSON");
        builder.AppendLine("- STOP State (and Label if present) ↔ `governanceCritical.stop`");
        builder.AppendLine("- Authorization / Work-Context / EDF sections ↔ `softwareDevelopmentProfile` when present");
    }

    private static void AppendAuthorizationSafety(StringBuilder builder)
    {
        builder.AppendLine("### Authorization safety");
        builder.AppendLine();
        builder.AppendLine("You may report work you performed. You may **not**:");
        builder.AppendLine("- Grant new implementation or tranche authorization");
        builder.AppendLine("- Set `authorizationDispositionPresent` true or add authorization disposition content to bypass governance");
        builder.AppendLine("- Set directive flags to direct implementation or tranche work");
        builder.AppendLine("- Remove inherited STOP or alter PA authority");
        builder.AppendLine("- Invent governance grants or change fixed ProjectConcord identifiers");
    }

    private static void AppendOutputIsolation(StringBuilder builder)
    {
        builder.AppendLine("### Final response (output isolation)");
        builder.AppendLine();
        builder.AppendLine("**RETURN ONLY ONE OUTER PLAIN-TEXT COPY SURFACE.**");
        builder.AppendLine("- Return **exactly one** outer Markdown code fence labeled `text` (or `plaintext`) whose **literal copied bytes** contain the **complete** canonical engineering-result relay document.");
        builder.AppendLine("- The copied payload MUST include `ProjectConcord-Relay-Render: 1`, the literal ` ```projectconcord-relay-v1` machine fence and JSON, and all required governance projections — not JSON alone.");
        builder.AppendLine("- Do **not** include a preamble, explanation, or Markdown introduction outside that single outer fence.");
        builder.AppendLine("- Do **not** return multiple relay documents, multiple render markers, or nested competing outer fences.");
        builder.AppendLine("- **Preserve** the inner ` ```projectconcord-relay-v1` fence and projection sections exactly inside the outer fence.");
        builder.AppendLine("- **Replace every required placeholder.** Do **not** return the instructional template unchanged.");
    }
}
