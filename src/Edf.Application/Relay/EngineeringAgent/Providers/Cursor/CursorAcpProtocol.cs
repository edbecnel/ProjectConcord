namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Text.Json;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

/// <summary>
/// Wire-format constants for Cursor CLI <c>agent acp</c> (provider-internal; see Cursor CLI ACP docs).
/// </summary>
internal static class CursorAcpProtocol
{
    internal const int ProtocolVersion = 1;

    internal const string AuthenticateMethodId = "cursor_login";

    internal const string ClientName = "ProjectConcord";

    internal const string ClientVersion = "A4-T6";

    internal static object CreateInitializeParameters() =>
        new
        {
            protocolVersion = ProtocolVersion,
            clientCapabilities = new
            {
                fs = new { readTextFile = false, writeTextFile = false },
                terminal = false,
            },
            clientInfo = new { name = ClientName, version = ClientVersion },
        };

    internal static object CreateAuthenticateParameters() =>
        new { methodId = AuthenticateMethodId };

    internal static object CreateSessionNewParameters(string cursorMode, string workingDirectory, string? modelId = null) =>
        modelId is null
            ? new
            {
                cwd = workingDirectory,
                mcpServers = Array.Empty<object>(),
                mode = cursorMode,
            }
            : new
            {
                cwd = workingDirectory,
                mcpServers = Array.Empty<object>(),
                mode = cursorMode,
                modelId,
            };

    internal static object CreateSessionSetModelParameters(string sessionId, string modelId) =>
        new { sessionId, modelId };

    internal static bool TryParseSessionNewResult(
        string rawLine,
        out string? sessionId,
        out string? currentModelId,
        out IReadOnlyList<CursorAcpAdvertisedModel> availableModels)
    {
        sessionId = null;
        currentModelId = null;
        availableModels = Array.Empty<CursorAcpAdvertisedModel>();
        try
        {
            using var doc = JsonDocument.Parse(rawLine);
            if (!doc.RootElement.TryGetProperty("result", out var resultEl))
            {
                return false;
            }

            if (resultEl.TryGetProperty("sessionId", out var sessionIdEl)
                && sessionIdEl.ValueKind == JsonValueKind.String)
            {
                sessionId = sessionIdEl.GetString();
            }

            if (!resultEl.TryGetProperty("models", out var modelsEl))
            {
                return !string.IsNullOrWhiteSpace(sessionId);
            }

            if (modelsEl.TryGetProperty("currentModelId", out var currentEl)
                && currentEl.ValueKind == JsonValueKind.String)
            {
                currentModelId = currentEl.GetString();
            }

            if (modelsEl.TryGetProperty("availableModels", out var availableEl)
                && availableEl.ValueKind == JsonValueKind.Array)
            {
                var list = new List<CursorAcpAdvertisedModel>();
                foreach (var item in availableEl.EnumerateArray())
                {
                    if (!item.TryGetProperty("modelId", out var modelIdEl)
                        || modelIdEl.ValueKind != JsonValueKind.String)
                    {
                        continue;
                    }

                    var wireId = modelIdEl.GetString() ?? string.Empty;
                    var name = wireId;
                    if (item.TryGetProperty("name", out var nameEl) && nameEl.ValueKind == JsonValueKind.String)
                    {
                        name = nameEl.GetString() ?? wireId;
                    }

                    list.Add(new CursorAcpAdvertisedModel(wireId, name));
                }

                availableModels = list;
            }

            return !string.IsNullOrWhiteSpace(sessionId);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    internal static object CreateSessionLoadParameters(string sessionId) =>
        new { sessionId };

    internal static object CreateSessionPromptParameters(string sessionId, string promptText) =>
        new
        {
            sessionId,
            prompt = new[]
            {
                new { type = "text", text = promptText },
            },
        };

    internal static bool TryReadPromptStopReason(string rawLine, out string? stopReason)
    {
        stopReason = null;
        try
        {
            using var doc = JsonDocument.Parse(rawLine);
            if (!doc.RootElement.TryGetProperty("result", out var resultEl)
                || !resultEl.TryGetProperty("stopReason", out var stopEl)
                || stopEl.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            stopReason = stopEl.GetString();
            return !string.IsNullOrWhiteSpace(stopReason);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    internal static bool IsTerminalPromptStopReason(string? stopReason) =>
        string.Equals(stopReason, "end_turn", StringComparison.OrdinalIgnoreCase)
        || string.Equals(stopReason, "prompt_complete", StringComparison.OrdinalIgnoreCase);

    internal static bool TryReadInitializeResultProtocolVersion(string rawLine, out int protocolVersion)
    {
        protocolVersion = 0;
        try
        {
            using var doc = JsonDocument.Parse(rawLine);
            if (!doc.RootElement.TryGetProperty("result", out var resultEl)
                || !resultEl.TryGetProperty("protocolVersion", out var versionEl)
                || versionEl.ValueKind != JsonValueKind.Number)
            {
                return false;
            }

            protocolVersion = versionEl.GetInt32();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
