namespace Edf.Application.Relay.Serialization;

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral tolerant recovery of one canonical relay-v1 document from imperfect manual paste.
/// Does not grant authority; does not alter governed field values.
/// </summary>
public static class GovernedRelayTolerantManualPasteInterpreter
{
    private const int MaxPresentationalOuterFencePeels = 8;

    private static readonly Regex MachineBlockRegex = new(
        $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\r?\\n([\\s\\S]*?)\\r?\\n```",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public static bool TryRecoverCanonicalDocument(
        string pastedText,
        out string canonicalDocument,
        out RelayValidationResult failure)
    {
        canonicalDocument = string.Empty;
        failure = RelayValidationResult.Valid([]);

        if (string.IsNullOrWhiteSpace(pastedText))
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteEmpty,
                "Paste is empty.");
            return false;
        }

        var trimmed = pastedText.Trim();

        if (IsJsonOnlyPaste(trimmed))
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteJsonOnlyRejected,
                "JSON-only paste is not a complete governed relay document.");
            return false;
        }

        if (IsMachineBlockOnlyPaste(trimmed))
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteMachineBlockOnlyRejected,
                "Machine-block-only paste is not a complete governed relay document.");
            return false;
        }

        var renderMarker = GovernedRelayV1Format.RenderVersionLinePrefix;
        var markerCount = CountOccurrences(trimmed, renderMarker);
        if (markerCount > 1)
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteRenderMarkerAmbiguous,
                "Multiple ProjectConcord relay render markers found; manual paste is ambiguous.");
            return false;
        }

        if (markerCount == 0)
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteRenderMarkerMissing,
                "ProjectConcord relay render marker is missing from the pasted manual transfer payload.");
            return false;
        }

        var candidates = new List<string> { trimmed };
        var peeled = PeelPresentationalOuterFences(trimmed);
        if (!string.Equals(peeled, trimmed, StringComparison.Ordinal))
        {
            candidates.Add(peeled);
        }

        foreach (var candidate in candidates)
        {
            if (GovernedRelayAutomatedResultDocumentExtractor.TryExtract(
                    candidate,
                    out var extracted,
                    out _))
            {
                canonicalDocument = extracted.TrimEnd() + Environment.NewLine;
                return true;
            }

            if (TryRecoverFramedDocumentWithStructuralMachineBlock(
                    candidate,
                    out var structurallyRecovered,
                    out _))
            {
                canonicalDocument = structurallyRecovered.TrimEnd() + Environment.NewLine;
                return true;
            }
        }

        var fallback = PeelPresentationalOuterFences(trimmed);
        if (CountOccurrences(fallback, renderMarker) == 1)
        {
            var machineFence = $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}";
            if (fallback.Contains(machineFence, StringComparison.Ordinal)
                || TryLocateRelayMachineJson(fallback, out _, out _))
            {
                canonicalDocument = fallback.TrimEnd() + Environment.NewLine;
                return true;
            }
        }

        failure = ClassifyRecoveryFailure(trimmed);
        return false;
    }

    private static RelayValidationResult ClassifyRecoveryFailure(string trimmed)
    {
        var renderMarker = GovernedRelayV1Format.RenderVersionLinePrefix;
        var reminder = GovernedRelayV1Format.PaEngineeringAgentReminder;
        var startIndex = trimmed.IndexOf(renderMarker, StringComparison.Ordinal);
        var reminderIndex = trimmed.IndexOf(reminder, StringComparison.Ordinal);

        if (startIndex < 0)
        {
            return Failed(
                RelayValidationCodes.ManualPasteRenderMarkerMissing,
                "ProjectConcord relay render marker is missing from the pasted manual transfer payload.");
        }

        var fromStart = trimmed[startIndex..];
        if (!MachineBlockRegex.IsMatch(fromStart)
            && !TryLocateRelayMachineJson(fromStart, out _, out _))
        {
            return Failed(
                RelayValidationCodes.ManualPasteMachineBlockMissing,
                "Required projectconcord-relay-v1 machine payload is missing from the pasted manual transfer payload.");
        }

        if (reminderIndex < 0)
        {
            return Failed(
                RelayValidationCodes.ManualPasteIncomplete,
                "The pasted response appears incomplete (missing the end of the governed relay document).");
        }

        var span = trimmed.Substring(startIndex, reminderIndex + reminder.Length - startIndex);
        if (!span.Contains(GovernedRelayV1Format.GovernanceCriticalHeading, StringComparison.Ordinal)
            || !span.Contains(GovernedRelayV1Format.StopHeading, StringComparison.Ordinal))
        {
            return Failed(
                RelayValidationCodes.ManualPasteIncomplete,
                "The pasted response appears incomplete (required governance projections are missing).");
        }

        return Failed(
            RelayValidationCodes.ManualPasteMachineBlockMissing,
            "Required projectconcord-relay-v1 machine fence is missing from the pasted manual transfer payload.");
    }

    private static string PeelPresentationalOuterFences(string text)
    {
        var current = text.Trim();
        for (var i = 0; i < MaxPresentationalOuterFencePeels; i++)
        {
            if (!GovernedRelayManualPasteCopyFence.TryUnwrapPresentationalOuterFence(
                    current,
                    out var inner,
                    out _))
            {
                break;
            }

            current = inner.Trim();
        }

        return current;
    }

    private static bool TryRecoverFramedDocumentWithStructuralMachineBlock(
        string untrusted,
        out string recovered,
        out RelayValidationResult failure)
    {
        recovered = string.Empty;
        failure = RelayValidationResult.Valid([]);

        var startMarker = GovernedRelayV1Format.RenderVersionLinePrefix;
        var endReminder = GovernedRelayV1Format.PaEngineeringAgentReminder;

        var startIndex = untrusted.IndexOf(startMarker, StringComparison.Ordinal);
        var reminderIndex = untrusted.IndexOf(endReminder, StringComparison.Ordinal);
        if (startIndex < 0 || reminderIndex < startIndex)
        {
            return false;
        }

        var endExclusive = reminderIndex + endReminder.Length;
        var span = untrusted.Substring(startIndex, endExclusive - startIndex);

        if (!span.Contains(GovernedRelayV1Format.GovernanceCriticalHeading, StringComparison.Ordinal)
            || !span.Contains(GovernedRelayV1Format.StopHeading, StringComparison.Ordinal))
        {
            return false;
        }

        if (MachineBlockRegex.IsMatch(span))
        {
            return false;
        }

        if (!TryLocateRelayMachineJson(span, out var jsonStartInSpan, out var jsonEndExclusiveInSpan))
        {
            return false;
        }

        var machineJson = span.Substring(jsonStartInSpan, jsonEndExclusiveInSpan - jsonStartInSpan).Trim();
        if (!IsRecognizedRelayMachineJson(machineJson))
        {
            return false;
        }

        var renderLineEnd = span.IndexOf('\n', StringComparison.Ordinal);
        if (renderLineEnd < 0)
        {
            renderLineEnd = span.Length;
        }

        var afterJson = span[jsonEndExclusiveInSpan..];
        if (!afterJson.Contains(GovernedRelayV1Format.GovernanceCriticalHeading, StringComparison.Ordinal))
        {
            return false;
        }

        var builder = new StringBuilder();
        builder.Append(span[..renderLineEnd].TrimEnd('\r'));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine($"```{GovernedRelayV1Format.MachineBlockFenceLanguage}");
        builder.AppendLine(machineJson);
        builder.AppendLine("```");
        builder.Append(afterJson.TrimStart('\r', '\n'));

        recovered = builder.ToString().TrimEnd() + Environment.NewLine;
        return true;
    }

    private static bool TryLocateRelayMachineJson(
        string span,
        out int jsonStartInSpan,
        out int jsonEndExclusiveInSpan)
    {
        jsonStartInSpan = 0;
        jsonEndExclusiveInSpan = 0;

        var governanceIndex = span.IndexOf(GovernedRelayV1Format.GovernanceCriticalHeading, StringComparison.Ordinal);
        var searchEnd = governanceIndex >= 0 ? governanceIndex : span.Length;
        var braceIndex = span.IndexOf('{', 0, searchEnd);
        if (braceIndex < 0)
        {
            return false;
        }

        if (!TryReadCompleteJsonObject(span, braceIndex, out jsonEndExclusiveInSpan))
        {
            return false;
        }

        jsonStartInSpan = braceIndex;
        return jsonEndExclusiveInSpan > jsonStartInSpan;
    }

    private static bool TryReadCompleteJsonObject(string text, int startIndex, out int endIndexExclusive)
    {
        endIndexExclusive = startIndex;
        if (startIndex < 0 || startIndex >= text.Length || text[startIndex] != '{')
        {
            return false;
        }

        var depth = 0;
        var inString = false;
        var escape = false;
        for (var i = startIndex; i < text.Length; i++)
        {
            var c = text[i];
            if (inString)
            {
                if (escape)
                {
                    escape = false;
                    continue;
                }

                if (c == '\\')
                {
                    escape = true;
                    continue;
                }

                if (c == '"')
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"')
            {
                inString = true;
                continue;
            }

            if (c == '{')
            {
                depth++;
                continue;
            }

            if (c == '}')
            {
                depth--;
                if (depth == 0)
                {
                    endIndexExclusive = i + 1;
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsRecognizedRelayMachineJson(string machineJson)
    {
        try
        {
            using var document = JsonDocument.Parse(machineJson);
            if (!document.RootElement.TryGetProperty("kind", out var kind))
            {
                return false;
            }

            return kind.ValueKind == JsonValueKind.String
                   && !string.IsNullOrWhiteSpace(kind.GetString());
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool IsJsonOnlyPaste(string trimmed)
    {
        if (!trimmed.StartsWith("{", StringComparison.Ordinal))
        {
            return false;
        }

        return !trimmed.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, StringComparison.Ordinal);
    }

    private static bool IsMachineBlockOnlyPaste(string trimmed)
    {
        var hasMachineFence = trimmed.Contains(
            $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}",
            StringComparison.Ordinal)
            || TryLocateRelayMachineJson(trimmed, out _, out _);
        if (!hasMachineFence)
        {
            return false;
        }

        return !trimmed.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, StringComparison.Ordinal);
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var index = 0;
        while (index < text.Length)
        {
            var found = text.IndexOf(value, index, StringComparison.Ordinal);
            if (found < 0)
            {
                break;
            }

            count++;
            index = found + value.Length;
        }

        return count;
    }

    private static RelayValidationResult Failed(string code, string message) =>
        RelayValidationResult.RejectedMalformed(
        [
            new RelayValidationDiagnostic(
                code,
                message,
                RelayValidationDiagnosticSeverity.Malformed),
        ]);
}
