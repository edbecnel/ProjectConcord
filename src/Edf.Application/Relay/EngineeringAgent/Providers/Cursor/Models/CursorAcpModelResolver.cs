namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

/// <summary>
/// Resolves durable Cursor model selection to exactly one ACP wire <c>modelId</c> (fail-closed).
/// </summary>
internal static class CursorAcpModelResolver
{
    public static bool TryResolveWireModelId(
        CursorEngineeringAgentModelSelection desired,
        IReadOnlyList<CursorAcpAdvertisedModel> availableModels,
        out string wireModelId,
        out string? failureMessage)
    {
        wireModelId = string.Empty;
        failureMessage = null;
        if (availableModels.Count == 0)
        {
            failureMessage = "Cursor ACP did not advertise any models for session selection.";
            return false;
        }

        var matches = availableModels
            .Where(m => CursorAcpModelIdentity.Parse(m.ModelId).Matches(desired))
            .ToList();
        if (matches.Count == 0)
        {
            failureMessage =
                $"No advertised Cursor model matches family '{desired.FamilySlug}' with fast={(desired.FastEnabled ? "true" : "false")}.";
            return false;
        }

        if (matches.Count > 1)
        {
            failureMessage =
                $"Multiple advertised Cursor models match family '{desired.FamilySlug}' with fast={(desired.FastEnabled ? "true" : "false")}; selection is ambiguous.";
            return false;
        }

        wireModelId = matches[0].ModelId;
        return true;
    }

    public static bool VerifyCurrentModelId(
        string resolvedWireModelId,
        CursorEngineeringAgentModelSelection desired,
        string currentModelId,
        out string? failureMessage)
    {
        failureMessage = null;
        if (!string.Equals(resolvedWireModelId, currentModelId, StringComparison.Ordinal))
        {
            failureMessage =
                "Cursor ACP session model does not match the resolved configured model.";
            return false;
        }

        if (!CursorAcpModelIdentity.Parse(currentModelId).Matches(desired))
        {
            failureMessage =
                "Cursor ACP session model does not match the configured model family and fast-mode intent.";
            return false;
        }

        return true;
    }
}
