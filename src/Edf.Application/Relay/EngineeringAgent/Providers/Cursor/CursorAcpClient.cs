namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Text.Json;
using Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Provider-internal ACP client over NDJSON JSON-RPC (Cursor CLI <c>agent acp</c>).
/// </summary>
internal sealed class CursorAcpClient : IAsyncDisposable
{
    private readonly ICursorAcpTransport _transport;
    private readonly CursorAcpPermissionPolicy _permissionPolicy;
    private readonly TimeSpan _ioTimeout;
    private int _nextRequestId = 1;
    private bool _initialized;
    private bool _authenticated;

    public CursorAcpClient(
        ICursorAcpTransport transport,
        CursorAcpPermissionPolicy? permissionPolicy = null,
        TimeSpan? ioTimeout = null)
    {
        _transport = transport ?? throw new ArgumentNullException(nameof(transport));
        _permissionPolicy = permissionPolicy ?? new CursorAcpPermissionPolicy();
        _ioTimeout = ioTimeout ?? TimeSpan.FromMinutes(5);
    }

    public bool IsInitialized => _initialized;

    public bool IsAuthenticated => _authenticated;

    public async Task<EngineeringAgentProviderInitializeResult> InitializeAndAuthenticateAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await _transport.StartAsync(cancellationToken).ConfigureAwait(false);
            var initResponse = await SendRequestAsync(
                "initialize",
                new { clientName = "ProjectConcord", clientVersion = "A4-T6" },
                cancellationToken).ConfigureAwait(false);
            if (initResponse.HasError)
            {
                return Failure(
                    EngineeringAgentProviderFailureKind.InitializationFailed,
                    DescribeError(initResponse.RawLine));
            }

            _initialized = true;

            var authResponse = await SendRequestAsync("authenticate", new { }, cancellationToken).ConfigureAwait(false);
            if (authResponse.HasError)
            {
                return Failure(
                    EngineeringAgentProviderFailureKind.AuthenticationUnavailable,
                    DescribeError(authResponse.RawLine));
            }

            _authenticated = true;
            return new EngineeringAgentProviderInitializeResult(
                new EngineeringAgentProviderHealth(true, true, null),
                Failure: null);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Failure(EngineeringAgentProviderFailureKind.InitializationFailed, ex.Message);
        }
    }

    public async Task<CursorAcpPromptResult> RunPromptAsync(
        string cursorMode,
        string promptText,
        string? existingSessionId,
        CancellationToken cancellationToken)
    {
        if (!_initialized || !_authenticated)
        {
            return CursorAcpPromptResult.FromFailure(
                EngineeringAgentProviderFailureKind.Unavailable,
                "ACP client is not initialized and authenticated.");
        }

        var sessionId = existingSessionId;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            var sessionResponse = await SendRequestAsync(
                "session/new",
                new { mode = cursorMode },
                cancellationToken).ConfigureAwait(false);
            if (sessionResponse.HasError)
            {
                return CursorAcpPromptResult.FromFailure(
                    EngineeringAgentProviderFailureKind.ForwardFailed,
                    DescribeError(sessionResponse.RawLine));
            }

            sessionId = TryReadSessionId(sessionResponse.RawLine)
                ?? throw new InvalidOperationException("session/new did not return a session id.");
        }
        else
        {
            var loadResponse = await SendRequestAsync(
                "session/load",
                new { sessionId },
                cancellationToken).ConfigureAwait(false);
            if (loadResponse.HasError)
            {
                return CursorAcpPromptResult.FromFailure(
                    EngineeringAgentProviderFailureKind.ForwardFailed,
                    DescribeError(loadResponse.RawLine));
            }
        }

        var promptResponse = await SendRequestAsync(
            "session/prompt",
            new { sessionId, prompt = promptText },
            cancellationToken).ConfigureAwait(false);
        if (promptResponse.HasError)
        {
            return CursorAcpPromptResult.FromFailure(
                EngineeringAgentProviderFailureKind.ForwardFailed,
                DescribeError(promptResponse.RawLine));
        }

        var collected = await CollectPromptUpdatesAsync(sessionId!, cancellationToken).ConfigureAwait(false);
        if (collected.Failure is not null)
        {
            return collected;
        }

        return CursorAcpPromptResult.FromSuccess(sessionId!, collected.ResultText ?? string.Empty);
    }

    public async Task<EngineeringAgentCancelResult> CancelSessionAsync(
        string sessionId,
        CancellationToken cancellationToken)
    {
        var response = await SendRequestAsync(
            "session/cancel",
            new { sessionId },
            cancellationToken).ConfigureAwait(false);
        if (response.HasError)
        {
            return new EngineeringAgentCancelResult(
                false,
                new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.CancelFailed,
                    DescribeError(response.RawLine)));
        }

        return new EngineeringAgentCancelResult(true, null);
    }

    public async ValueTask DisposeAsync() => await _transport.DisposeAsync().ConfigureAwait(false);

    private async Task<CursorAcpPromptResult> CollectPromptUpdatesAsync(
        string sessionId,
        CancellationToken cancellationToken)
    {
        var builder = new System.Text.StringBuilder();
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_ioTimeout);

        while (!timeoutCts.IsCancellationRequested)
        {
            var line = await _transport.ReadLineAsync(timeoutCts.Token).ConfigureAwait(false);
            if (line is null)
            {
                return CursorAcpPromptResult.FromFailure(
                    EngineeringAgentProviderFailureKind.ResultRetrievalFailed,
                    "ACP stream ended before prompt completion.");
            }

            if (!CursorAcpNdjsonCodec.TryParseLine(line, out var message))
            {
                continue;
            }

            if (message.IsNotification)
            {
                await HandleNotificationAsync(message, sessionId, builder, timeoutCts.Token).ConfigureAwait(false);
                if (IsPromptComplete(message))
                {
                    return CursorAcpPromptResult.FromSuccess(sessionId, builder.ToString());
                }

                continue;
            }
        }

        return CursorAcpPromptResult.FromFailure(
            EngineeringAgentProviderFailureKind.TimedOut,
            "Timed out waiting for ACP prompt completion.");
    }

    private async Task HandleNotificationAsync(
        CursorAcpInboundMessage message,
        string sessionId,
        System.Text.StringBuilder builder,
        CancellationToken cancellationToken)
    {
        if (message.Method == "session/update"
            && TryReadSessionUpdate(message.RawLine, out var updateType, out var updateText))
        {
            if (!string.IsNullOrEmpty(updateText))
            {
                builder.Append(updateText);
            }

            return;
        }

        if (message.Method == "session/request_permission"
            && TryReadPermissionRequest(message.RawLine, out var requestId, out var kind, out var detail))
        {
            var decision = _permissionPolicy.Evaluate(new CursorAcpPermissionRequest(requestId, kind, detail));
            var wireDecision = decision.Disposition == EngineeringAgentProviderPermissionDisposition.AllowOnce
                ? "allow-once"
                : "deny";
            await SendRequestAsync(
                "session/permission_response",
                new { sessionId, requestId, decision = wireDecision },
                cancellationToken).ConfigureAwait(false);
        }
    }

    private static bool IsPromptComplete(CursorAcpInboundMessage message) =>
        message.Method == "session/update"
        && TryReadSessionUpdate(message.RawLine, out var updateType, out _)
        && string.Equals(updateType, "prompt_complete", StringComparison.OrdinalIgnoreCase);

    private async Task<CursorAcpInboundMessage> SendRequestAsync(
        string method,
        object parameters,
        CancellationToken cancellationToken)
    {
        var id = _nextRequestId++;
        var payload = CursorAcpNdjsonCodec.SerializeRequest(method, parameters, id);
        await _transport.WriteLineAsync(payload, cancellationToken).ConfigureAwait(false);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_ioTimeout);

        while (!timeoutCts.IsCancellationRequested)
        {
            var line = await _transport.ReadLineAsync(timeoutCts.Token).ConfigureAwait(false);
            if (line is null)
            {
                break;
            }

            if (!CursorAcpNdjsonCodec.TryParseLine(line, out var message))
            {
                continue;
            }

            if (message.IsNotification)
            {
                continue;
            }

            if (message.Id == id)
            {
                return message;
            }
        }

        return new CursorAcpInboundMessage(null, id, true, """{"error":{"message":"timeout"}}""");
    }

    private static string? TryReadSessionId(string rawLine)
    {
        using var doc = JsonDocument.Parse(rawLine);
        if (!doc.RootElement.TryGetProperty("result", out var resultEl))
        {
            return null;
        }

        if (resultEl.TryGetProperty("sessionId", out var sessionIdEl) && sessionIdEl.ValueKind == JsonValueKind.String)
        {
            return sessionIdEl.GetString();
        }

        return null;
    }

    private static bool TryReadSessionUpdate(string rawLine, out string? updateType, out string? updateText)
    {
        updateType = null;
        updateText = null;
        using var doc = JsonDocument.Parse(rawLine);
        if (!doc.RootElement.TryGetProperty("params", out var paramsEl)
            || !paramsEl.TryGetProperty("update", out var updateEl))
        {
            return false;
        }

        if (updateEl.TryGetProperty("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String)
        {
            updateType = typeEl.GetString();
        }

        if (updateEl.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
        {
            updateText = textEl.GetString();
        }

        return true;
    }

    private static bool TryReadPermissionRequest(
        string rawLine,
        out string requestId,
        out string kind,
        out string detail)
    {
        requestId = string.Empty;
        kind = "unknown";
        detail = rawLine;
        using var doc = JsonDocument.Parse(rawLine);
        if (!doc.RootElement.TryGetProperty("params", out var paramsEl))
        {
            return false;
        }

        if (paramsEl.TryGetProperty("requestId", out var reqEl) && reqEl.ValueKind == JsonValueKind.String)
        {
            requestId = reqEl.GetString() ?? string.Empty;
        }

        if (paramsEl.TryGetProperty("permission", out var permEl) && permEl.ValueKind == JsonValueKind.String)
        {
            kind = permEl.GetString() ?? "unknown";
        }

        return true;
    }

    private static string DescribeError(string rawLine)
    {
        using var doc = JsonDocument.Parse(rawLine);
        if (doc.RootElement.TryGetProperty("error", out var errorEl)
            && errorEl.TryGetProperty("message", out var messageEl)
            && messageEl.ValueKind == JsonValueKind.String)
        {
            return messageEl.GetString() ?? "ACP error";
        }

        return rawLine;
    }

    private static EngineeringAgentProviderInitializeResult Failure(
        EngineeringAgentProviderFailureKind kind,
        string message) =>
        new(
            EngineeringAgentProviderHealth.Unavailable(message),
            new EngineeringAgentProviderFailure(kind, message));
}

internal sealed record CursorAcpPromptResult(
    bool Succeeded,
    string? SessionId,
    string? ResultText,
    EngineeringAgentProviderFailure? Failure)
{
    public static CursorAcpPromptResult FromSuccess(string sessionId, string resultText) =>
        new(true, sessionId, resultText, null);

    public static CursorAcpPromptResult FromFailure(EngineeringAgentProviderFailureKind kind, string message) =>
        new(false, null, null, new EngineeringAgentProviderFailure(kind, message));
}
