namespace Edf.Application.Relay.Serialization;

using System.Text.RegularExpressions;
using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// Provider-neutral extraction of a single explicitly framed relay-v1 document from an untrusted
/// automated transport envelope (A4-T6 remediation #3D). Does not validate governance.
/// </summary>
public static class GovernedRelayAutomatedResultDocumentExtractor
{
    private static readonly Regex MachineBlockRegex = new(
        $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\r?\\n([\\s\\S]*?)\\r?\\n```",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    /// <summary>
    /// Attempts to extract exactly one contiguous relay document using marker-byte START (C2) and
    /// canonical PA reminder END. On failure, <paramref name="failure"/> is populated and no text is returned.
    /// </summary>
    public static bool TryExtract(
        string untrustedProviderResponse,
        out string extractedText,
        out RelayValidationResult failure)
    {
        extractedText = string.Empty;
        failure = RelayValidationResult.Valid([]);

        if (string.IsNullOrEmpty(untrustedProviderResponse))
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionStartMarkerMissing,
                "Provider response is empty; no ProjectConcord relay render marker.");
            return false;
        }

        var startMarker = GovernedRelayV1Format.RenderVersionLinePrefix;
        var endReminder = GovernedRelayV1Format.PaEngineeringAgentReminder;

        var startMarkerCount = CountOccurrences(untrustedProviderResponse, startMarker);
        if (startMarkerCount == 0)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionStartMarkerMissing,
                "No ProjectConcord relay render marker found in provider response.");
            return false;
        }

        if (startMarkerCount > 1)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionStartMarkerAmbiguous,
                "Multiple ProjectConcord relay render markers found; extraction is ambiguous.");
            return false;
        }

        var startIndex = untrustedProviderResponse.IndexOf(startMarker, StringComparison.Ordinal);

        var endReminderCount = CountOccurrences(untrustedProviderResponse, endReminder);
        if (endReminderCount == 0)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionEndBoundaryMissing,
                "Canonical relay document end boundary (PA reminder) is missing.");
            return false;
        }

        if (endReminderCount > 1)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionEndBoundaryAmbiguous,
                "Multiple canonical relay document end boundaries found; extraction is ambiguous.");
            return false;
        }

        var reminderIndex = untrustedProviderResponse.IndexOf(endReminder, StringComparison.Ordinal);
        if (reminderIndex < startIndex)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionBoundaryOrderInvalid,
                "Relay document end boundary appears before the render marker.");
            return false;
        }

        var endExclusive = reminderIndex + endReminder.Length;
        var candidateLength = endExclusive - startIndex;
        if (candidateLength <= 0)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionBoundaryOrderInvalid,
                "Relay document framing boundaries are invalid.");
            return false;
        }

        var candidateSpan = untrustedProviderResponse.Substring(startIndex, candidateLength);
        var machineMatches = MachineBlockRegex.Matches(candidateSpan);
        if (machineMatches.Count == 0)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionMachineBlockAmbiguous,
                "No projectconcord-relay-v1 machine block found within framed relay document boundaries.");
            return false;
        }

        if (machineMatches.Count > 1)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionMachineBlockAmbiguous,
                "Multiple projectconcord-relay-v1 machine blocks found; extraction is ambiguous.");
            return false;
        }

        var machineMatch = machineMatches[0];
        var machineEndInCandidate = machineMatch.Index + machineMatch.Length;
        var reminderOffsetInCandidate = reminderIndex - startIndex;
        if (machineMatch.Index <= 0 || machineEndInCandidate > reminderOffsetInCandidate)
        {
            failure = Failed(
                RelayValidationCodes.TransportExtractionBoundaryOrderInvalid,
                "Render marker, machine block, and PA reminder are not in valid order.");
            return false;
        }

        extractedText = candidateSpan;
        return true;
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
