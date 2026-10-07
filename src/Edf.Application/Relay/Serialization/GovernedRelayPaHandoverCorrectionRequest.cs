namespace Edf.Application.Relay.Serialization;

using System.Text;
using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// Presentation-only PA correction request (not a governed package kind).
/// </summary>
public static class GovernedRelayPaHandoverCorrectionRequest
{
    public const string SectionHeading = "## ProjectConcord PA Handover Correction Request";

    public const string ContractVersion = "pa-handover-correction-request/v1";

    public static string RenderCompleteCorrectionRequest(
        GovernedRelayPackage trustedReviewExport,
        PaHandoverResponseProfile responseProfile,
        PaHandoverCorrectionFailureClass failureClass)
    {
        ArgumentNullException.ThrowIfNull(trustedReviewExport);
        if (trustedReviewExport.Kind != GovernedPackageKind.PaReviewExport)
        {
            throw new ArgumentException("Correction requests require a trusted PaReviewExport package.", nameof(trustedReviewExport));
        }

        var responseContract = GovernedRelayPaHandoverOutputContract.RenderCompleteContract(
            trustedReviewExport,
            responseProfile);

        var builder = new StringBuilder();
        builder.AppendLine(SectionHeading);
        builder.AppendLine();
        builder.AppendLine($"**Correction contract:** `{ContractVersion}`");
        builder.AppendLine($"**Requested response profile:** {DescribeResponseProfile(responseProfile)}");
        builder.AppendLine();
        builder.AppendLine(
            "ProjectConcord could **not** safely use your previous `paHandoverImport` response for this review. "
            + "Return a **replacement** `paHandoverImport` for the **same** review leg below — not a new authorization episode.");
        builder.AppendLine();
        builder.AppendLine("### Trusted exchange identity (ProjectConcord-fixed)");
        builder.AppendLine();
        builder.AppendLine($"- **projectId:** `{trustedReviewExport.ProjectId.Value}`");
        builder.AppendLine($"- **correlationId:** `{trustedReviewExport.CorrelationId.Value}` (preserve in replacement handover)");
        builder.AppendLine($"- **review packageId:** `{trustedReviewExport.PackageId.Value}`");
        builder.AppendLine();
        builder.AppendLine("### Why the previous response could not be accepted");
        builder.AppendLine();
        builder.AppendLine(DescribeFailureClass(failureClass));
        builder.AppendLine();
        builder.AppendLine("### Required replacement response");
        builder.AppendLine();
        builder.AppendLine(
            "Return **one** normal copy surface containing the **complete** replacement `paHandoverImport` document: "
            + "`ProjectConcord-Relay-Render: 1`, the `projectconcord-relay-v1` machine JSON, **all** required governance projections, "
            + "and the PA reminder line — not machine JSON alone and not projections separated from the machine block.");
        builder.AppendLine();
        builder.AppendLine(
            "Do **not** change substantive governance judgments unless you are making a new governed decision. "
            + "This request is for a **complete, copyable replacement** of the prior unusable response.");
        builder.AppendLine();
        builder.AppendLine("---");
        builder.AppendLine();
        builder.AppendLine(responseContract);

        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(builder.ToString().TrimEnd());
        return wrapped;
    }

    private static string DescribeFailureClass(PaHandoverCorrectionFailureClass failureClass) =>
        failureClass switch
        {
            PaHandoverCorrectionFailureClass.Incomplete =>
                "The prior response appeared **incomplete** or missing required governed sections (for example governance projections).",
            PaHandoverCorrectionFailureClass.Ambiguous =>
                "The prior paste contained **more than one** candidate governed response or ambiguous framing.",
            PaHandoverCorrectionFailureClass.StructuralMalformed =>
                "The prior response contained a **malformed** machine payload or invalid JSON.",
            PaHandoverCorrectionFailureClass.GovernanceProjectionInconsistent =>
                "Machine JSON and human-readable governance projections **did not agree**. Return a replacement with consistent values.",
            PaHandoverCorrectionFailureClass.ProjectIdentityMismatch =>
                "The prior response did not match the active ProjectConcord project identity for this review.",
            PaHandoverCorrectionFailureClass.CorrelationMismatch =>
                "The prior response **correlationId** did not match this review export.",
            _ =>
                "ProjectConcord could **not** recognize one complete governed `paHandoverImport` response in what was pasted.",
        };

    private static string DescribeResponseProfile(PaHandoverResponseProfile profile) =>
        profile switch
        {
            PaHandoverResponseProfile.PlanningEntry => "PLANNING ENTRY",
            PaHandoverResponseProfile.ImplementationDirected => "IMPLEMENTATION-DIRECTED",
            PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization => "PLANNING DEVELOPMENT WORK AUTHORIZATION",
            _ => profile.ToString(),
        };
}
