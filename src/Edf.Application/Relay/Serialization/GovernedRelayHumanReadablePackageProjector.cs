namespace Edf.Application.Relay.Serialization;

using System.Text;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Deterministic, read-only operator projection of a governed relay package (ADR-0026 §8).
/// </summary>
public static class GovernedRelayHumanReadablePackageProjector
{
    public const string ViewPurposeFootnote =
        "This view is for your understanding. The copy action transfers the canonical governed package — not this human-readable presentation.";

    public const string EngineeringAgentReturnExpectation =
        "The Engineering Agent must return a governed Engineering Result response according to the relay contract.";

    public static string Project(
        GovernedRelayPackage package,
        GovernedRelayHumanReadablePackageOptions options)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(options);

        var envelope = GovernedRelayEnvelopeV1Mapper.ToDto(package);
        var profile = envelope.SoftwareDevelopmentProfile;
        var builder = new StringBuilder();

        builder.AppendLine($"To: {DescribeCounterparty(options.Counterparty)}");
        builder.AppendLine($"Package purpose: {DescribePackagePurpose(package.Kind)}");
        builder.AppendLine();

        if (!string.IsNullOrWhiteSpace(options.OperatorWorkSubject))
        {
            builder.AppendLine("Work subject (operator continuity — not a canonical package field):");
            builder.AppendLine(options.OperatorWorkSubject.Trim());
            builder.AppendLine();
        }

        AppendAuthorizationSection(builder, profile, envelope.GovernanceCritical);
        AppendStopSection(builder, envelope.GovernanceCritical.Stop);

        if (profile?.AuthorizationDisposition is { } disposition)
        {
            builder.AppendLine("Authorization disposition:");
            if (!string.IsNullOrWhiteSpace(disposition.DispositionSummary))
            {
                builder.AppendLine(disposition.DispositionSummary);
            }

            builder.AppendLine(
                $"Planning authorized: {FormatBool(disposition.PlanningAuthorized)}; "
                + $"Implementation authorized: {FormatBool(disposition.ImplementationAuthorized)}.");
            builder.AppendLine();
        }

        if (profile?.WorkContext is { } workContext)
        {
            builder.AppendLine("Requested work:");
            if (!string.IsNullOrWhiteSpace(workContext.RequestedTrancheId))
            {
                builder.AppendLine($"Tranche: {workContext.RequestedTrancheId}");
            }

            if (!string.IsNullOrWhiteSpace(workContext.RequestedWorkSummary))
            {
                builder.AppendLine(workContext.RequestedWorkSummary);
            }

            builder.AppendLine();
        }

        if (profile?.HandoverContext is { } handoverContext
            && !string.IsNullOrWhiteSpace(handoverContext.Summary))
        {
            builder.AppendLine(
                handoverContext.IsInheritedContextOnly
                    ? "Inherited handover context (not a new instruction):"
                    : "Handover context:");
            builder.AppendLine(handoverContext.Summary);
            builder.AppendLine();
        }

        builder.AppendLine("Session and mode (governed relay):");
        builder.Append(GovernedRelayGovernanceProjections.Render(envelope));
        builder.AppendLine();
        builder.AppendLine();

        if (options.Counterparty == GovernedRelayHumanReadableCounterparty.EngineeringAgent)
        {
            builder.AppendLine("Expected return:");
            builder.AppendLine(EngineeringAgentReturnExpectation);
            builder.AppendLine();
        }

        builder.AppendLine(ViewPurposeFootnote);

        return builder.ToString().TrimEnd();
    }

    private static void AppendAuthorizationSection(
        StringBuilder builder,
        SoftwareDevelopmentProfilePayload? profile,
        RelayGovernanceCriticalState governance)
    {
        builder.AppendLine("Authorization constraints:");
        var planningFromProfile = profile?.AuthorizationDisposition?.PlanningAuthorized == true
                                  || profile?.DevelopmentWorkAuthorization?.Kind
                                      == SoftwareDevelopmentAuthorizationKind.Planning;
        var implementationFromProfile = profile?.AuthorizationDisposition?.ImplementationAuthorized == true;

        if (planningFromProfile && !implementationFromProfile)
        {
            builder.AppendLine(
                "Planning development work authorization is reflected in this package. "
                + "Repository implementation is not authorized by that Planning development work authorization.");
        }
        else if (implementationFromProfile)
        {
            builder.AppendLine("Implementation development work authorization is indicated in this package.");
        }
        else if (!governance.AuthorizationDispositionPresent)
        {
            builder.AppendLine("No authorization disposition is present in this package.");
        }

        if (governance.DirectiveFlags.DirectsImplementationWork)
        {
            builder.AppendLine("This package directs implementation work under governed rules.");
        }

        if (governance.DirectiveFlags.DirectsTrancheWork)
        {
            builder.AppendLine("This package directs tranche-scoped work under governed rules.");
        }

        builder.AppendLine();
    }

    private static void AppendStopSection(StringBuilder builder, RelayStopMetadata stop)
    {
        builder.AppendLine("STOP:");
        builder.AppendLine(
            stop.State == RelayStopState.Active
                ? "STOP is active — ordinary progression may be blocked."
                : "STOP is not active.");
        if (!string.IsNullOrWhiteSpace(stop.ExplicitLabel))
        {
            builder.AppendLine($"STOP label: {stop.ExplicitLabel}");
        }

        builder.AppendLine();
    }

    private static string DescribeCounterparty(GovernedRelayHumanReadableCounterparty counterparty) =>
        counterparty switch
        {
            GovernedRelayHumanReadableCounterparty.EngineeringAgent => "Engineering Agent",
            GovernedRelayHumanReadableCounterparty.ProjectArchitect => "Project Architect",
            _ => "Governed counterparty",
        };

    private static string DescribePackagePurpose(GovernedPackageKind kind) =>
        kind switch
        {
            GovernedPackageKind.PaReviewExport => "Project Architect review export",
            GovernedPackageKind.PaHandoverImport => "Project Architect handover import",
            GovernedPackageKind.EngineeringAgentHandoverExport => "Engineering Agent handover export",
            GovernedPackageKind.EngineeringResultImport => "Engineering Result import",
            _ => kind.ToString(),
        };

    private static string FormatBool(bool value) => value ? "Yes" : "No";
}
