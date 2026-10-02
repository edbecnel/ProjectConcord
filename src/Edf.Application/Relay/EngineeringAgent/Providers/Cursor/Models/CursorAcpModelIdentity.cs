namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

using System.Text.RegularExpressions;

/// <summary>
/// Parses Cursor ACP <c>modelId</c> strings into family slug and explicit fast-mode semantics.
/// </summary>
internal sealed record CursorAcpModelIdentity(string BaseSlug, bool FastEnabled)
{
    private static readonly Regex FastParameterRegex = new(
        @"fast\s*=\s*(true|false)",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static CursorAcpModelIdentity Parse(string modelId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelId);
        var bracketIndex = modelId.IndexOf('[', StringComparison.Ordinal);
        var baseSlug = bracketIndex < 0 ? modelId : modelId[..bracketIndex];
        if (bracketIndex < 0)
        {
            return new CursorAcpModelIdentity(baseSlug, FastEnabled: false);
        }

        var parameters = modelId[(bracketIndex + 1)..].TrimEnd(']');
        var fastMatch = FastParameterRegex.Match(parameters);
        if (!fastMatch.Success)
        {
            return new CursorAcpModelIdentity(baseSlug, FastEnabled: false);
        }

        var fastEnabled = string.Equals(fastMatch.Groups[1].Value, "true", StringComparison.OrdinalIgnoreCase);
        return new CursorAcpModelIdentity(baseSlug, fastEnabled);
    }

    public bool Matches(CursorEngineeringAgentModelSelection desired) =>
        string.Equals(BaseSlug, desired.FamilySlug, StringComparison.OrdinalIgnoreCase)
        && FastEnabled == desired.FastEnabled;
}
