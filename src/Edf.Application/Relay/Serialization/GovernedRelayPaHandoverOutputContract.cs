namespace Edf.Application.Relay.Serialization;

using System.Text;
using System.Text.Json;
using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral PA Handover output contract v1 for manual PA review export responses.
/// </summary>
public static class GovernedRelayPaHandoverOutputContract
{
    public const string SectionHeading = "## ProjectConcord PA Handover Output Contract";

    public const string ContractVersion = "pa-handover-response/v1";

    public const string PlaceholderPackageId = "__REPLACE_WITH_NEW_PACKAGE_ID_UUID__";

    public const string PlaceholderUtcTimestamp = "__REPLACE_WITH_UTC_ISO8601_TIMESTAMP__";

    public const string PlaceholderDispositionSummary = "__REPLACE_AUTHORIZATION_DISPOSITION_SUMMARY__";

    public const string PlaceholderAuthorizedTrancheId = "__REPLACE_AUTHORIZED_TRANCHE_ID__";

    public const string PlaceholderWorkContextSummary = "__REPLACE_WORK_CONTEXT_SUMMARY__";

    public const string PlaceholderDwaAuthorizationReference = "__REPLACE_DWA_AUTHORIZATION_REFERENCE__";

    private static readonly JsonSerializerOptions TimestampFormatOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static string RenderCompleteContract(
        GovernedRelayPackage reviewExportPackage,
        PaHandoverResponseProfile responseProfile)
    {
        ArgumentNullException.ThrowIfNull(reviewExportPackage);
        var reporting = GovernedRelayPaHandoverProfileDerivation.DeriveReportingRequirements(
            reviewExportPackage,
            responseProfile);
        var template = RenderStructuralTemplateWithPlaceholders(reviewExportPackage, reporting);

        var builder = new StringBuilder();
        builder.AppendLine(SectionHeading);
        builder.AppendLine();
        builder.AppendLine($"**Response contract:** `{ContractVersion}`");
        builder.AppendLine($"**Requested response profile:** {DescribeResponseProfile(reporting.ResponseProfile)}");
        builder.AppendLine();
        builder.AppendLine(
            "Your **final message** for ProjectConcord MUST be **only** one outer plain-text copy surface "
            + "containing the **complete** governed relay document — a `paHandoverImport` response to this review export. "
            + "ProjectConcord validates all provider output as **untrusted** until PA handover import succeeds.");
        builder.AppendLine();
        AppendProfileSemantics(builder, reporting);
        builder.AppendLine();
        AppendFieldSemantics(builder, reviewExportPackage, reporting);
        builder.AppendLine();
        builder.AppendLine("### Structural template (replace every placeholder)");
        builder.AppendLine();
        builder.AppendLine(
            "The block below shows the **exact single-copy shape** you must return (outer plain-text fence wrapping the full canonical relay document). "
            + "**PRESERVE / FIXED** values must be copied exactly from this template. "
            + "**REPLACE / DECIDE** placeholders must be completed per the field semantics and response profile. "
            + "**Do not return placeholder text.**");
        builder.AppendLine();
        builder.AppendLine(template);
        builder.AppendLine();
        AppendMachineProjectionAgreement(builder);
        builder.AppendLine();
        AppendAuthorizationSafety(builder, reporting);
        builder.AppendLine();
        AppendOutputIsolation(builder);

        return builder.ToString().TrimEnd();
    }

    public static string RenderStructuralTemplateWithPlaceholders(
        GovernedRelayPackage reviewExportPackage,
        PaHandoverResponseProfile responseProfile)
    {
        var reporting = GovernedRelayPaHandoverProfileDerivation.DeriveReportingRequirements(
            reviewExportPackage,
            responseProfile);
        return RenderStructuralTemplateWithPlaceholders(reviewExportPackage, reporting);
    }

    internal static string RenderStructuralTemplateWithPlaceholders(
        GovernedRelayPackage reviewExportPackage,
        GovernedRelayPaHandoverReportingRequirements reporting)
    {
        var skeleton = CreatePaHandoverImportSkeleton(
            reviewExportPackage,
            reporting,
            useFreshIdentity: true,
            planningAuthorized: false,
            implementationAuthorized: reporting.ImplementationAuthorizedFixedValue ?? false,
            dispositionSummary: PlaceholderDispositionSummary,
            stop: RelayStopMetadata.None);
        var rendered = new GovernedRelayV1Renderer().Render(skeleton);
        var withPlaceholders = ApplyInstructionalPlaceholders(rendered, skeleton);
        return GovernedRelayManualPasteCopyFence.WrapForManualCopy(withPlaceholders);
    }

    public static string RenderCompletedPlanningEntryHandover(
        GovernedRelayPackage reviewExportPackage,
        bool planningAuthorized,
        string dispositionSummary,
        RelayStopMetadata? stop = null)
    {
        var reporting = GovernedRelayPaHandoverReportingRequirements.PlanningEntry;
        var stopMeta = stop ?? RelayStopMetadata.None;
        var package = CreatePaHandoverImportSkeleton(
            reviewExportPackage,
            reporting,
            useFreshIdentity: true,
            planningAuthorized,
            implementationAuthorized: false,
            dispositionSummary,
            stopMeta);
        return new GovernedRelayV1Renderer().Render(package);
    }

    public static string RenderCompletedImplementationDirectedHandover(
        GovernedRelayPackage reviewExportPackage,
        string dispositionSummary,
        string authorizedTrancheId,
        string workSummary)
    {
        var reporting = GovernedRelayPaHandoverReportingRequirements.ImplementationDirected;
        var package = CreatePaHandoverImportSkeleton(
            reviewExportPackage,
            reporting,
            useFreshIdentity: true,
            planningAuthorized: false,
            implementationAuthorized: true,
            dispositionSummary,
            RelayStopMetadata.None,
            authorizedTrancheId,
            workSummary,
            dwaReference: "dwa-projection-ref");
        return new GovernedRelayV1Renderer().Render(package);
    }

    internal static GovernedRelayPackage CreatePaHandoverImportSkeleton(
        GovernedRelayPackage reviewExportPackage,
        GovernedRelayPaHandoverReportingRequirements reporting,
        bool useFreshIdentity,
        bool planningAuthorized,
        bool implementationAuthorized,
        string dispositionSummary,
        RelayStopMetadata stop,
        string? authorizedTrancheId = null,
        string? workSummary = null,
        string? dwaReference = null)
    {
        var gc = reviewExportPackage.GovernanceCritical;
        var handoverGovernance = new RelayGovernanceCriticalState(
            gc.EngineeringAgentMode ?? EngineeringAgentMode.Plan,
            gc.PriorEngineeringAgentMode,
            gc.ModeTransition,
            gc.SessionContinuity,
            stop,
            reporting.AuthorizationDispositionPresent,
            reporting.WorkContextPresent,
            new RelayGovernanceDirectiveFlags(
                reporting.DirectsImplementationWork,
                reporting.DirectsTrancheWork),
            gc.EdfCorrelation);

        var profileBytes = BuildProfilePayload(
            reporting,
            planningAuthorized,
            implementationAuthorized,
            dispositionSummary,
            authorizedTrancheId,
            workSummary,
            dwaReference);

        var now = DateTimeOffset.UtcNow;
        return new GovernedRelayPackage(
            useFreshIdentity ? GovernedPackageId.New() : reviewExportPackage.PackageId,
            reviewExportPackage.CorrelationId,
            GovernedPackageKind.PaHandoverImport,
            reviewExportPackage.SchemaVersion,
            reviewExportPackage.RenderVersion ?? RelayRenderVersion.V1,
            reviewExportPackage.ProjectId,
            now,
            now,
            handoverGovernance,
            Tier0RelaySnapshot.Empty,
            profileBytes,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }

    private static byte[] BuildProfilePayload(
        GovernedRelayPaHandoverReportingRequirements reporting,
        bool planningAuthorized,
        bool implementationAuthorized,
        string dispositionSummary,
        string? authorizedTrancheId,
        string? workSummary,
        string? dwaReference)
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty;

        if (reporting.AuthorizationDispositionPresent)
        {
            payload = payload with
            {
                AuthorizationDisposition = new AuthorizationDispositionProjection(
                    dispositionSummary,
                    planningAuthorized,
                    implementationAuthorized,
                    authorizedTrancheId),
            };
        }

        if (reporting.WorkContextPresent)
        {
            payload = payload with
            {
                WorkContext = new WorkContextProjection(
                    authorizedTrancheId,
                    workSummary ?? PlaceholderWorkContextSummary),
            };
        }

        if (reporting.RequiresDevelopmentWorkAuthorizationProjection && implementationAuthorized)
        {
            payload = payload with
            {
                DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                    SoftwareDevelopmentAuthorizationKind.Implementation,
                    authorizedTrancheId,
                    ["authorized-scope"],
                    dwaReference ?? PlaceholderDwaAuthorizationReference,
                    false),
            };
        }

        return SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload);
    }

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

        if (rendered.Contains(PlaceholderDispositionSummary, StringComparison.Ordinal))
        {
            return rendered.TrimEnd() + Environment.NewLine;
        }

        return rendered.TrimEnd() + Environment.NewLine;
    }

    private static string FormatJsonTimestamp(DateTimeOffset value) =>
        JsonSerializer.Serialize(value, TimestampFormatOptions).Trim('"');

    private static string DescribeResponseProfile(PaHandoverResponseProfile profile) =>
        profile switch
        {
            PaHandoverResponseProfile.PlanningEntry => "PLANNING ENTRY",
            PaHandoverResponseProfile.ImplementationDirected => "IMPLEMENTATION DIRECTED",
            _ => profile.ToString(),
        };

    private static void AppendProfileSemantics(
        StringBuilder builder,
        GovernedRelayPaHandoverReportingRequirements reporting)
    {
        builder.AppendLine("### Response profile semantics");
        builder.AppendLine();
        if (reporting.ResponseProfile == PaHandoverResponseProfile.PlanningEntry)
        {
            builder.AppendLine(
                "This review requests a **planning-entry** governed decision. You must decide whether "
                + "**planning entry is authorized** (`planningAuthorized` true or false). "
                + "You must **not** authorize implementation in this profile (`implementationAuthorized` must remain **false**). "
                + "Do **not** add a `developmentWorkAuthorization` projection. "
                + "Directive flags must remain false. STOP remains your governance judgment.");
        }
        else
        {
            builder.AppendLine(
                "This review requests an **implementation-directed** handover. You must decide implementation "
                + "authorization, tranche scope, and work context. When `implementationAuthorized` is true, "
                + "an explicit `developmentWorkAuthorization` projection is required.");
        }
    }

    private static void AppendFieldSemantics(
        StringBuilder builder,
        GovernedRelayPackage reviewExport,
        GovernedRelayPaHandoverReportingRequirements reporting)
    {
        var projectId = reviewExport.ProjectId.Value;
        var correlationId = reviewExport.CorrelationId.Value;
        var mode = reviewExport.GovernanceCritical.EngineeringAgentMode ?? EngineeringAgentMode.Plan;
        var fence = GovernedRelayV1Format.MachineBlockFenceLanguage;

        builder.AppendLine("### Field semantics");
        builder.AppendLine();
        builder.AppendLine("| Field | Source | Required | Rules |");
        builder.AppendLine("| --- | --- | --- | --- |");
        builder.AppendLine(
            $"| Document first line `{GovernedRelayV1Format.RenderVersionLinePrefix} 1` | PRESERVE | Yes | Exact line; render version **1**. |");
        builder.AppendLine(
            $"| Machine fence ` ```{fence}` | PRESERVE | Yes | Preserve the inner fence exactly; do not nest an outer fence around the whole response. |");
        builder.AppendLine(
            $"| `kind` | PRESERVE | Yes | Must be `paHandoverImport`. |");
        builder.AppendLine(
            $"| `schemaVersionMajor` / `schemaVersionMinor` | PRESERVE | Yes | `{reviewExport.SchemaVersion.Major}` / `{reviewExport.SchemaVersion.Minor}`. |");
        builder.AppendLine(
            $"| `projectId` | PRESERVE | Yes | `{projectId}`. |");
        builder.AppendLine(
            $"| `correlationId` | PRESERVE | Yes | `{correlationId}` (links this handover to the review export). |");
        builder.AppendLine(
            $"| `packageId` | REPLACE | Yes | New unique UUID. Replace `{PlaceholderPackageId}`. |");
        builder.AppendLine(
            $"| `createdUtc` / `updatedUtc` | REPLACE | Yes | UTC ISO-8601. Replace `{PlaceholderUtcTimestamp}`. |");
        builder.AppendLine(
            $"| `governanceCritical.engineeringAgentMode` | PRESERVE unless mode change | Yes | Template shows `{mode.ToString().ToLowerInvariant()}`; include mode transition metadata if you change mode. |");
        builder.AppendLine(
            "| `governanceCritical.sessionContinuity` | PRESERVE | Yes | Reproduce template session intents in machine JSON **and** projections. |");
        builder.AppendLine(
            "| `governanceCritical.stop` | DECIDE | Yes | Set STOP state in machine JSON and `## STOP` projection consistently. |");
        builder.AppendLine(
            $"| `governanceCritical.authorizationDispositionPresent` | PRESERVE | Yes | `{reporting.AuthorizationDispositionPresent.ToString().ToLowerInvariant()}`. |");
        builder.AppendLine(
            $"| `governanceCritical.workContextPresent` | PRESERVE | Yes | `{reporting.WorkContextPresent.ToString().ToLowerInvariant()}`. |");
        builder.AppendLine(
            $"| `governanceCritical.directiveFlags.directsImplementationWork` | PRESERVE | Yes | `{reporting.DirectsImplementationWork.ToString().ToLowerInvariant()}`. |");
        builder.AppendLine(
            $"| `governanceCritical.directiveFlags.directsTrancheWork` | PRESERVE | Yes | `{reporting.DirectsTrancheWork.ToString().ToLowerInvariant()}`. |");
        builder.AppendLine(
            "| `softwareDevelopmentProfile.payloadVersion` | PRESERVE | Yes | Must be `1`. |");
        builder.AppendLine(
            "| `softwareDevelopmentProfile.authorizationDisposition` | REQUIRED when disposition present | Yes | "
            + "Must exist when `authorizationDispositionPresent` is true. "
            + "Fields: `dispositionSummary` (REPLACE), `planningAuthorized` (DECIDE unless profile-fixed), "
            + "`implementationAuthorized` (profile constraints apply), optional `authorizedTrancheId`. |");
        builder.AppendLine(
            "| `## Authorization-Disposition` projection | DECIDE + PRESERVE agreement | Yes | "
            + "Must mirror `softwareDevelopmentProfile.authorizationDisposition` and governance-critical disposition flags. |");
        builder.AppendLine(
            "| `softwareDevelopmentProfile.developmentWorkAuthorization` | Profile-dependent | "
            + (reporting.RequiresDevelopmentWorkAuthorizationProjection ? "Yes when implementation authorized" : "No")
            + " | "
            + (reporting.RequiresDevelopmentWorkAuthorizationProjection
                ? "Required when directing implementation work with implementation authorization."
                : "Must be absent for Planning Entry profile."));
        builder.AppendLine();
        builder.AppendLine(
            "**Machine JSON booleans** and **human-readable projection booleans** (including `## Authorization-Disposition` "
            + "Planning-Authorized and Implementation-Authorized) use lowercase `true` or `false` only.");
    }

    private static void AppendMachineProjectionAgreement(StringBuilder builder)
    {
        builder.AppendLine("### Machine / projection agreement");
        builder.AppendLine();
        builder.AppendLine("Human-readable sections are **not** independent commentary. Values MUST match the machine JSON:");
        builder.AppendLine("- Engineering-Agent-Mode ↔ `governanceCritical.engineeringAgentMode`");
        builder.AppendLine("- Engineering-Agent-Chat ↔ `governanceCritical.sessionContinuity.engineeringAgentSessionIntent`");
        builder.AppendLine("- ChatGPT-Chat ↔ `governanceCritical.sessionContinuity.projectArchitectSessionIntent`");
        builder.AppendLine("- Engineering-Agent-Mode-Transition (if present) ↔ `governanceCritical.modeTransition`");
        builder.AppendLine("- Authorization-Disposition-Present / Work-Context-Present / directive flags ↔ same fields in machine JSON");
        builder.AppendLine("- STOP State (and Label if present) ↔ `governanceCritical.stop`");
        builder.AppendLine(
            "- `## Authorization-Disposition` lines (Planning-Authorized, Implementation-Authorized, Disposition-Summary, Authorized-Tranche-Id) "
            + "↔ `softwareDevelopmentProfile.authorizationDisposition`");
        builder.AppendLine("- `## Work-Context` ↔ `softwareDevelopmentProfile.workContext` when work context is present");
    }

    private static void AppendAuthorizationSafety(
        StringBuilder builder,
        GovernedRelayPaHandoverReportingRequirements reporting)
    {
        builder.AppendLine("### Authorization safety");
        builder.AppendLine();
        builder.AppendLine("You may record governed judgments. You may **not**:");
        builder.AppendLine("- Change fixed `projectId` or `correlationId`");
        builder.AppendLine("- Omit required `softwareDevelopmentProfile` blocks when governance-critical flags require them");
        if (reporting.ResponseProfile == PaHandoverResponseProfile.PlanningEntry)
        {
            builder.AppendLine("- Set `implementationAuthorized` true or add `developmentWorkAuthorization` in the Planning Entry profile");
            builder.AppendLine("- Set directive flags to direct implementation or tranche work in the Planning Entry profile");
        }

        builder.AppendLine("- Remove STOP without an explicit governed STOP decision reflected consistently");
    }

    private static void AppendOutputIsolation(StringBuilder builder)
    {
        builder.AppendLine("### Final response (output isolation)");
        builder.AppendLine();
        builder.AppendLine("**RETURN ONLY ONE OUTER PLAIN-TEXT COPY SURFACE.**");
        builder.AppendLine("- Return **exactly one** outer Markdown code fence labeled `text` (or `plaintext`) whose **literal copied bytes** contain the **complete** canonical `paHandoverImport` relay document.");
        builder.AppendLine("- The copied payload MUST include `ProjectConcord-Relay-Render: 1`, the literal ` ```projectconcord-relay-v1` machine fence and JSON, and all required governance projections — not JSON alone.");
        builder.AppendLine("- Do **not** include a preamble, explanation, or Markdown introduction outside that single outer fence.");
        builder.AppendLine("- Do **not** return multiple relay documents, multiple render markers, or nested competing outer fences.");
        builder.AppendLine("- **Preserve** the inner ` ```projectconcord-relay-v1` fence and projection sections exactly inside the outer fence.");
        builder.AppendLine("- **Replace every required placeholder.** Do **not** return the instructional template unchanged.");
    }
}
