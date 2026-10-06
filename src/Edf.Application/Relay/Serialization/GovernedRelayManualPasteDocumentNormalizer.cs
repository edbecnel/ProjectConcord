namespace Edf.Application.Relay.Serialization;

using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// E-layer manual paste normalization before canonical relay v1 import. Does not grant authority.
/// </summary>
public static class GovernedRelayManualPasteDocumentNormalizer
{
    public static bool TryNormalizeToCanonicalRelayDocument(
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
        var candidate = trimmed;
        if (GovernedRelayManualPasteCopyFence.TryUnwrapSingleOuterFence(trimmed, out var unwrapped, out _))
        {
            candidate = unwrapped.Trim();
            if (GovernedRelayManualPasteCopyFence.TryUnwrapSingleOuterFence(candidate, out _, out _))
            {
                failure = Failed(
                    RelayValidationCodes.ManualPasteRenderMarkerAmbiguous,
                    "Nested outer manual copy fences are not supported.");
                return false;
            }
        }

        if (IsJsonOnlyPaste(candidate))
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteJsonOnlyRejected,
                "JSON-only paste is not a complete governed relay document. "
                + "Copy the outer plain-text relay artifact that includes ProjectConcord-Relay-Render and governance projections.");
            return false;
        }

        if (IsMachineBlockOnlyPaste(candidate))
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteMachineBlockOnlyRejected,
                "Machine-block-only paste is not a complete governed relay document. "
                + "Copy the outer plain-text relay artifact that includes ProjectConcord-Relay-Render and governance projections.");
            return false;
        }

        var renderMarker = GovernedRelayV1Format.RenderVersionLinePrefix;
        var markerCount = CountOccurrences(candidate, renderMarker);
        if (markerCount == 0)
        {
            if (GovernedRelayAutomatedResultDocumentExtractor.TryExtract(
                    trimmed,
                    out var extracted,
                    out var extractionFailure))
            {
                candidate = extracted.Trim();
                markerCount = CountOccurrences(candidate, renderMarker);
            }
            else if (extractionFailure.Diagnostics.Count > 0)
            {
                failure = extractionFailure;
                return false;
            }
        }

        if (markerCount == 0)
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteRenderMarkerMissing,
                "ProjectConcord relay render marker is missing from the pasted manual transfer payload.");
            return false;
        }

        if (markerCount > 1)
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteRenderMarkerAmbiguous,
                "Multiple ProjectConcord relay render markers found; manual paste is ambiguous.");
            return false;
        }

        if (!candidate.Contains(
                $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}",
                StringComparison.Ordinal))
        {
            failure = Failed(
                RelayValidationCodes.ManualPasteMachineBlockMissing,
                "Required projectconcord-relay-v1 machine fence is missing from the pasted manual transfer payload.");
            return false;
        }

        canonicalDocument = candidate;
        return true;
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
            StringComparison.Ordinal);
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
