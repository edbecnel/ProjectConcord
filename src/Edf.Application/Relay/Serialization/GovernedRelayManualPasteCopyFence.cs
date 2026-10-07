namespace Edf.Application.Relay.Serialization;

/// <summary>
/// Outer plain-text Markdown fence so chat UIs copy the complete canonical relay document literally.
/// </summary>
public static class GovernedRelayManualPasteCopyFence
{
    public const string OuterFenceLanguage = "text";

    public static string WrapForManualCopy(string canonicalRelayDocument)
    {
        ArgumentNullException.ThrowIfNull(canonicalRelayDocument);
        var inner = canonicalRelayDocument.TrimEnd();
        return $"```{OuterFenceLanguage}{Environment.NewLine}{inner}{Environment.NewLine}```{Environment.NewLine}";
    }

    /// <summary>
    /// Unwraps one presentational outer fence when the paste is only that fence, or when trailing
    /// prose exists after a closing fence (provider copy surfaces).
    /// </summary>
    public static bool TryUnwrapPresentationalOuterFence(string pastedText, out string inner, out string? error)
    {
        if (TryUnwrapSingleOuterFence(pastedText, out inner, out error))
        {
            return true;
        }

        return TryUnwrapLeadingOuterFence(pastedText, out inner, out error);
    }

    /// <summary>
    /// Unwraps at most one outer generic copy fence when the entire paste is that fence.
    /// </summary>
    public static bool TryUnwrapSingleOuterFence(string pastedText, out string inner, out string? error)
    {
        inner = string.Empty;
        error = null;
        if (string.IsNullOrWhiteSpace(pastedText))
        {
            error = "Paste is empty.";
            return false;
        }

        var trimmed = pastedText.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            error = "Paste is not wrapped in an outer copy fence.";
            return false;
        }

        var firstNewline = trimmed.IndexOf('\n');
        if (firstNewline < 0)
        {
            error = "Outer copy fence is malformed.";
            return false;
        }

        var openingLine = trimmed[..firstNewline].TrimEnd('\r');
        if (!IsSupportedOuterOpeningLine(openingLine))
        {
            error = "Outer copy fence language is not a supported generic plain-text fence.";
            return false;
        }

        var fenceLength = CountLeadingBackticks(openingLine);
        var closing = new string('`', fenceLength);
        var closingIndex = trimmed.LastIndexOf(closing, StringComparison.Ordinal);
        if (closingIndex <= firstNewline)
        {
            error = "Outer copy fence is malformed.";
            return false;
        }

        var trailingAfterClose = trimmed[(closingIndex + closing.Length)..].Trim();
        if (trailingAfterClose.Length > 0)
        {
            error = "Outer copy fence must be the only content in the paste.";
            return false;
        }

        inner = trimmed[(firstNewline + 1)..closingIndex].TrimEnd('\r', '\n', ' ');
        if (string.IsNullOrWhiteSpace(inner))
        {
            error = "Outer copy fence contains no relay document.";
            return false;
        }

        return true;
    }

    private static bool TryUnwrapLeadingOuterFence(string pastedText, out string inner, out string? error)
    {
        inner = string.Empty;
        error = null;
        if (string.IsNullOrWhiteSpace(pastedText))
        {
            error = "Paste is empty.";
            return false;
        }

        var trimmed = pastedText.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            error = "Paste is not wrapped in an outer copy fence.";
            return false;
        }

        var firstNewline = trimmed.IndexOf('\n');
        if (firstNewline < 0)
        {
            error = "Outer copy fence is malformed.";
            return false;
        }

        var openingLine = trimmed[..firstNewline].TrimEnd('\r');
        if (!IsSupportedOuterOpeningLine(openingLine))
        {
            error = "Outer copy fence language is not a supported generic plain-text fence.";
            return false;
        }

        var fenceLength = CountLeadingBackticks(openingLine);
        var closing = new string('`', fenceLength);
        var closingIndex = trimmed.IndexOf($"\n{closing}", firstNewline, StringComparison.Ordinal);
        if (closingIndex < 0)
        {
            error = "Outer copy fence is malformed.";
            return false;
        }

        inner = trimmed[(firstNewline + 1)..closingIndex].TrimEnd('\r', '\n', ' ');
        if (string.IsNullOrWhiteSpace(inner))
        {
            error = "Outer copy fence contains no relay document.";
            return false;
        }

        return true;
    }

    private static int CountLeadingBackticks(string openingLine)
    {
        var count = 0;
        foreach (var c in openingLine)
        {
            if (c == '`')
            {
                count++;
                continue;
            }

            break;
        }

        return count >= 3 ? count : 3;
    }

    private static bool IsSupportedOuterOpeningLine(string openingLine)
    {
        var backtickCount = CountLeadingBackticks(openingLine);
        if (backtickCount < 3)
        {
            return false;
        }

        var language = openingLine[backtickCount..].Trim();
        return language.Length == 0
               || string.Equals(language, OuterFenceLanguage, StringComparison.OrdinalIgnoreCase)
               || string.Equals(language, "plaintext", StringComparison.OrdinalIgnoreCase);
    }
}
