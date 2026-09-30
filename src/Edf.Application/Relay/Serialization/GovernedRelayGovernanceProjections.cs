namespace Edf.Application.Relay.Serialization;

using System.Text;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Deterministic human-readable governance projections (non-authoritative duplicates of machine state).
/// </summary>
internal static class GovernedRelayGovernanceProjections
{
    internal sealed record ParsedProjections(
        EngineeringAgentMode? EngineeringAgentMode,
        EngineeringAgentMode? PriorEngineeringAgentMode,
        EngineeringAgentModeTransition? ModeTransition,
        AgentSessionIntent? ProjectArchitectSessionIntent,
        AgentSessionIntent? EngineeringAgentSessionIntent,
        RelayStopState StopState,
        string? StopLabel,
        bool AuthorizationDispositionPresent,
        bool WorkContextPresent,
        bool DirectsImplementationWork,
        bool DirectsTrancheWork,
        AuthorizationDispositionProjection? AuthorizationDisposition,
        WorkContextProjection? WorkContext,
        IReadOnlyList<string>? EdfArtifactReferences);

    public static string Render(GovernedRelayEnvelopeV1Dto envelope)
    {
        var governance = envelope.GovernanceCritical;
        var session = governance.SessionContinuity;
        var builder = new StringBuilder();

        builder.AppendLine(GovernedRelayV1Format.GovernanceCriticalHeading);
        builder.AppendLine(FormatKeyValue("Cursor-Mode", FormatMode(governance.EngineeringAgentMode)));
        builder.AppendLine(FormatKeyValue("Cursor-Chat", FormatSessionIntent(session.EngineeringAgentSessionIntent)));
        builder.AppendLine(FormatKeyValue("ChatGPT-Chat", FormatSessionIntent(session.ProjectArchitectSessionIntent)));

        if (governance.ModeTransition is not null)
        {
            builder.AppendLine(FormatKeyValue(
                "Cursor-Mode-Transition",
                FormatModeTransition(governance.ModeTransition)));
        }

        builder.AppendLine(FormatKeyValue(
            "Authorization-Disposition-Present",
            FormatBool(governance.AuthorizationDispositionPresent)));
        builder.AppendLine(FormatKeyValue(
            "Work-Context-Present",
            FormatBool(governance.WorkContextPresent)));
        builder.AppendLine(FormatKeyValue(
            "Directs-Implementation-Work",
            FormatBool(governance.DirectiveFlags.DirectsImplementationWork)));
        builder.AppendLine(FormatKeyValue(
            "Directs-Tranche-Work",
            FormatBool(governance.DirectiveFlags.DirectsTrancheWork)));
        builder.AppendLine();

        builder.AppendLine(GovernedRelayV1Format.StopHeading);
        builder.AppendLine(FormatKeyValue("State", FormatStopState(governance.Stop.State)));
        if (!string.IsNullOrWhiteSpace(governance.Stop.ExplicitLabel))
        {
            builder.AppendLine(FormatKeyValue("Label", governance.Stop.ExplicitLabel));
        }

        builder.AppendLine();

        if (envelope.SoftwareDevelopmentProfile?.AuthorizationDisposition is not null
            || governance.AuthorizationDispositionPresent)
        {
            builder.AppendLine(GovernedRelayV1Format.AuthorizationDispositionHeading);
            var disposition = envelope.SoftwareDevelopmentProfile?.AuthorizationDisposition;
            builder.AppendLine(FormatKeyValue(
                "Planning-Authorized",
                FormatBool(disposition?.PlanningAuthorized ?? false)));
            builder.AppendLine(FormatKeyValue(
                "Implementation-Authorized",
                FormatBool(disposition?.ImplementationAuthorized ?? false)));
            builder.AppendLine(FormatKeyValue("Authorized-Tranche-Id", disposition?.AuthorizedTrancheId ?? string.Empty));
            builder.AppendLine(FormatKeyValue("Disposition-Summary", disposition?.DispositionSummary ?? string.Empty));
            builder.AppendLine();
        }

        if (envelope.SoftwareDevelopmentProfile?.WorkContext is not null || governance.WorkContextPresent)
        {
            builder.AppendLine(GovernedRelayV1Format.WorkContextHeading);
            var work = envelope.SoftwareDevelopmentProfile?.WorkContext;
            builder.AppendLine(FormatKeyValue("Requested-Tranche-Id", work?.RequestedTrancheId ?? string.Empty));
            builder.AppendLine(FormatKeyValue("Requested-Work-Summary", work?.RequestedWorkSummary ?? string.Empty));
            builder.AppendLine();
        }

        if (governance.EdfCorrelation?.ArtifactReferences is { Count: > 0 } refs)
        {
            builder.AppendLine(GovernedRelayV1Format.EdfCorrelationHeading);
            builder.AppendLine(FormatKeyValue("Artifact-References", string.Join(", ", refs)));
            builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }

    public static bool TryParse(string renderedBody, out ParsedProjections? projections, out string? error)
    {
        projections = null;
        error = null;

        var governanceSection = ExtractSection(renderedBody, GovernedRelayV1Format.GovernanceCriticalHeading);
        if (governanceSection is null)
        {
            error = "Governance-Critical section is required.";
            return false;
        }

        var stopSection = ExtractSection(renderedBody, GovernedRelayV1Format.StopHeading) ?? string.Empty;
        var authSection = ExtractSection(renderedBody, GovernedRelayV1Format.AuthorizationDispositionHeading);
        var workSection = ExtractSection(renderedBody, GovernedRelayV1Format.WorkContextHeading);
        var edfSection = ExtractSection(renderedBody, GovernedRelayV1Format.EdfCorrelationHeading);

        if (!TryParseMode(ReadKey(governanceSection, "Cursor-Mode"), out var mode, out error))
        {
            return false;
        }

        if (!TryParseSessionIntent(ReadKey(governanceSection, "Cursor-Chat"), out var eaIntent, out error))
        {
            return false;
        }

        if (!TryParseSessionIntent(ReadKey(governanceSection, "ChatGPT-Chat"), out var paIntent, out error))
        {
            return false;
        }

        EngineeringAgentModeTransition? transition = null;
        var transitionText = ReadKey(governanceSection, "Cursor-Mode-Transition");
        if (!string.IsNullOrWhiteSpace(transitionText))
        {
            if (!TryParseModeTransition(transitionText, out transition, out error))
            {
                return false;
            }
        }

        EngineeringAgentMode? priorMode = transition?.From;

        if (!TryParseStopState(ReadKey(stopSection, "State"), out var stopState, out error))
        {
            return false;
        }

        var stopLabel = ReadKey(stopSection, "Label");

        var authPresent = ParseBool(ReadKey(governanceSection, "Authorization-Disposition-Present"));
        var workPresent = ParseBool(ReadKey(governanceSection, "Work-Context-Present"));
        var directsImplementation = ParseBool(ReadKey(governanceSection, "Directs-Implementation-Work"));
        var directsTranche = ParseBool(ReadKey(governanceSection, "Directs-Tranche-Work"));

        AuthorizationDispositionProjection? authProjection = null;
        if (authSection is not null)
        {
            authProjection = new AuthorizationDispositionProjection(
                ReadKey(authSection, "Disposition-Summary"),
                ParseBool(ReadKey(authSection, "Planning-Authorized")),
                ParseBool(ReadKey(authSection, "Implementation-Authorized")),
                NullIfEmpty(ReadKey(authSection, "Authorized-Tranche-Id")));
        }

        WorkContextProjection? workProjection = null;
        if (workSection is not null)
        {
            workProjection = new WorkContextProjection(
                NullIfEmpty(ReadKey(workSection, "Requested-Tranche-Id")),
                NullIfEmpty(ReadKey(workSection, "Requested-Work-Summary")));
        }

        IReadOnlyList<string>? edfRefs = null;
        if (edfSection is not null)
        {
            var refsText = ReadKey(edfSection, "Artifact-References");
            if (!string.IsNullOrWhiteSpace(refsText))
            {
                edfRefs = refsText
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }
        }

        projections = new ParsedProjections(
            mode,
            priorMode,
            transition,
            paIntent,
            eaIntent,
            stopState,
            NullIfEmpty(stopLabel),
            authPresent,
            workPresent,
            directsImplementation,
            directsTranche,
            authProjection,
            workProjection,
            edfRefs);

        return true;
    }

    public static bool ProjectionsAgree(GovernedRelayEnvelopeV1Dto envelope, ParsedProjections parsed)
    {
        var governance = envelope.GovernanceCritical;
        var session = governance.SessionContinuity;
        var profile = envelope.SoftwareDevelopmentProfile;

        if (governance.EngineeringAgentMode != parsed.EngineeringAgentMode)
        {
            return false;
        }

        if (!ModeTransitionEquals(governance.ModeTransition, parsed.ModeTransition))
        {
            return false;
        }

        if (session.ProjectArchitectSessionIntent != parsed.ProjectArchitectSessionIntent
            || session.EngineeringAgentSessionIntent != parsed.EngineeringAgentSessionIntent)
        {
            return false;
        }

        if (governance.Stop.State != parsed.StopState)
        {
            return false;
        }

        if (!string.Equals(
                governance.Stop.ExplicitLabel ?? string.Empty,
                parsed.StopLabel ?? string.Empty,
                StringComparison.Ordinal))
        {
            return false;
        }

        if (governance.AuthorizationDispositionPresent != parsed.AuthorizationDispositionPresent
            || governance.WorkContextPresent != parsed.WorkContextPresent
            || governance.DirectiveFlags.DirectsImplementationWork != parsed.DirectsImplementationWork
            || governance.DirectiveFlags.DirectsTrancheWork != parsed.DirectsTrancheWork)
        {
            return false;
        }

        if (!AuthorizationDispositionEquals(profile?.AuthorizationDisposition, parsed.AuthorizationDisposition))
        {
            return false;
        }

        if (!WorkContextEquals(profile?.WorkContext, parsed.WorkContext))
        {
            return false;
        }

        var machineRefs = governance.EdfCorrelation?.ArtifactReferences;
        if (!ArtifactReferencesEqual(machineRefs, parsed.EdfArtifactReferences))
        {
            return false;
        }

        return true;
    }

    private static bool ModeTransitionEquals(
        EngineeringAgentModeTransition? machine,
        EngineeringAgentModeTransition? parsed) =>
        machine?.From == parsed?.From && machine?.To == parsed?.To;

    private static bool AuthorizationDispositionEquals(
        AuthorizationDispositionProjection? machine,
        AuthorizationDispositionProjection? parsed)
    {
        if (machine is null && parsed is null)
        {
            return true;
        }

        if (machine is null || parsed is null)
        {
            return machine is null
                && parsed is not null
                && !parsed.PlanningAuthorized
                && !parsed.ImplementationAuthorized
                && string.IsNullOrWhiteSpace(parsed.AuthorizedTrancheId)
                && string.IsNullOrWhiteSpace(parsed.DispositionSummary);
        }

        return machine.PlanningAuthorized == parsed.PlanningAuthorized
            && machine.ImplementationAuthorized == parsed.ImplementationAuthorized
            && string.Equals(machine.AuthorizedTrancheId, parsed.AuthorizedTrancheId, StringComparison.Ordinal)
            && string.Equals(machine.DispositionSummary, parsed.DispositionSummary, StringComparison.Ordinal);
    }

    private static bool WorkContextEquals(WorkContextProjection? machine, WorkContextProjection? parsed)
    {
        if (machine is null && parsed is null)
        {
            return true;
        }

        if (machine is null || parsed is null)
        {
            return machine is null
                && parsed is not null
                && string.IsNullOrWhiteSpace(parsed.RequestedTrancheId)
                && string.IsNullOrWhiteSpace(parsed.RequestedWorkSummary);
        }

        return string.Equals(machine.RequestedTrancheId, parsed.RequestedTrancheId, StringComparison.Ordinal)
            && string.Equals(machine.RequestedWorkSummary, parsed.RequestedWorkSummary, StringComparison.Ordinal);
    }

    private static bool ArtifactReferencesEqual(
        IReadOnlyList<string>? machine,
        IReadOnlyList<string>? parsed)
    {
        var left = machine ?? Array.Empty<string>();
        var right = parsed ?? Array.Empty<string>();
        if (left.Count != right.Count)
        {
            return false;
        }

        for (var i = 0; i < left.Count; i++)
        {
            if (!string.Equals(left[i], right[i], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static string FormatKeyValue(string key, string value) => $"{key}: {value}";

    private static string FormatMode(EngineeringAgentMode? mode) =>
        mode switch
        {
            EngineeringAgentMode.Plan => "PLAN",
            EngineeringAgentMode.Agent => "AGENT",
            EngineeringAgentMode.Debug => "DEBUG",
            _ => string.Empty,
        };

    private static string FormatSessionIntent(AgentSessionIntent? intent) =>
        intent switch
        {
            AgentSessionIntent.New => "NEW",
            AgentSessionIntent.Continue => "CONTINUE",
            _ => string.Empty,
        };

    private static string FormatModeTransition(EngineeringAgentModeTransition transition) =>
        $"{FormatMode(transition.From)} -> {FormatMode(transition.To)}";

    private static string FormatStopState(RelayStopState state) =>
        state switch
        {
            RelayStopState.None => "None",
            RelayStopState.Active => "Active",
            RelayStopState.Acknowledged => "Acknowledged",
            _ => "None",
        };

    private static string? ExtractSection(string body, string heading)
    {
        var index = body.IndexOf(heading, StringComparison.Ordinal);
        if (index < 0)
        {
            return null;
        }

        var start = index + heading.Length;
        var nextHeading = body.IndexOf("\n## ", start, StringComparison.Ordinal);
        var section = nextHeading < 0 ? body[start..] : body[start..nextHeading];
        return section.Trim();
    }

    private static string ReadKey(string section, string key)
    {
        foreach (var line in section.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var separator = line.IndexOf(':');
            if (separator <= 0)
            {
                continue;
            }

            var lineKey = line[..separator].Trim();
            if (!string.Equals(lineKey, key, StringComparison.Ordinal))
            {
                continue;
            }

            return line[(separator + 1)..].Trim();
        }

        return string.Empty;
    }

    private static bool TryParseMode(string text, out EngineeringAgentMode? mode, out string? error)
    {
        mode = null;
        error = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        mode = text.Trim().ToUpperInvariant() switch
        {
            "PLAN" => EngineeringAgentMode.Plan,
            "AGENT" => EngineeringAgentMode.Agent,
            "DEBUG" => EngineeringAgentMode.Debug,
            _ => null,
        };

        if (mode is null)
        {
            error = $"Unknown engineering agent mode '{text}'.";
            return false;
        }

        return true;
    }

    private static bool TryParseSessionIntent(string text, out AgentSessionIntent? intent, out string? error)
    {
        intent = null;
        error = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        intent = text.Trim().ToUpperInvariant() switch
        {
            "NEW" => AgentSessionIntent.New,
            "CONTINUE" => AgentSessionIntent.Continue,
            _ => null,
        };

        if (intent is null)
        {
            error = $"Unknown session intent '{text}'.";
            return false;
        }

        return true;
    }

    private static bool TryParseModeTransition(
        string text,
        out EngineeringAgentModeTransition? transition,
        out string? error)
    {
        transition = null;
        error = null;
        var parts = text.Split("->", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            error = "Mode transition must use 'FROM -> TO' format.";
            return false;
        }

        if (!TryParseMode(parts[0], out var from, out error) || from is null)
        {
            return false;
        }

        if (!TryParseMode(parts[1], out var to, out error) || to is null)
        {
            return false;
        }

        transition = new EngineeringAgentModeTransition(from.Value, to.Value);
        return true;
    }

    private static bool TryParseStopState(string text, out RelayStopState state, out string? error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            state = RelayStopState.None;
            return true;
        }

        if (!Enum.TryParse<RelayStopState>(text.Trim(), ignoreCase: true, out state))
        {
            error = $"Unknown STOP state '{text}'.";
            return false;
        }

        return true;
    }

    private static string FormatBool(bool value) =>
        value ? "true" : "false";

    private static bool ParseBool(string text) =>
        bool.TryParse(text, out var value) && value;

    private static string? NullIfEmpty(string text) =>
        string.IsNullOrWhiteSpace(text) ? null : text;
}
