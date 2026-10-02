namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Text;
using System.Text.Json;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

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
                CursorAcpProtocol.CreateInitializeParameters(),
                cancellationToken).ConfigureAwait(false);
            if (initResponse.HasError)
            {
                return Failure(
                    EngineeringAgentProviderFailureKind.InitializationFailed,
                    DescribeError(initResponse.RawLine));
            }

            if (!CursorAcpProtocol.TryReadInitializeResultProtocolVersion(
                    initResponse.RawLine,
                    out var negotiatedVersion)
                || negotiatedVersion != CursorAcpProtocol.ProtocolVersion)
            {
                return Failure(
                    EngineeringAgentProviderFailureKind.InitializationFailed,
                    "ACP initialize did not return the expected protocolVersion.");
            }

            _initialized = true;

            var authResponse = await SendRequestAsync(
                "authenticate",
                CursorAcpProtocol.CreateAuthenticateParameters(),
                cancellationToken).ConfigureAwait(false);
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
        string governedProjectRootAbsolutePath,
        CursorEngineeringAgentModelSelection modelSelection,
        string? existingSessionId,
        CancellationToken cancellationToken)
    {
        if (!_initialized || !_authenticated)
        {
            return CursorAcpPromptResult.FromFailure(
                EngineeringAgentProviderFailureKind.Unavailable,
                "ACP client is not initialized and authenticated.");
        }

        if (string.IsNullOrWhiteSpace(governedProjectRootAbsolutePath))
        {
            return CursorAcpPromptResult.FromFailure(
                EngineeringAgentProviderFailureKind.ForwardFailed,
                "Governed Project Root path is required for ACP session/new.");
        }

        var sessionPrepare = await PrepareVerifiedSessionAsync(
                cursorMode,
                governedProjectRootAbsolutePath,
                modelSelection,
                existingSessionId,
                cancellationToken)
            .ConfigureAwait(false);
        if (sessionPrepare.Failure is not null)
        {
            return CursorAcpPromptResult.FromFailure(sessionPrepare.Failure.Kind, sessionPrepare.Failure.Message);
        }

        var sessionId = sessionPrepare.SessionId!;

        var streamCollector = new CursorAcpPromptStreamCollector(sessionId!);
        var promptResponse = await SendRequestAsync(
            "session/prompt",
            CursorAcpProtocol.CreateSessionPromptParameters(sessionId!, promptText),
            cancellationToken,
            streamCollector).ConfigureAwait(false);
        if (promptResponse.HasError)
        {
            return CursorAcpPromptResult.FromFailure(
                EngineeringAgentProviderFailureKind.ForwardFailed,
                DescribeError(promptResponse.RawLine));
        }

        if (CursorAcpProtocol.TryReadPromptStopReason(promptResponse.RawLine, out var stopReason)
            && CursorAcpProtocol.IsTerminalPromptStopReason(stopReason))
        {
            return CursorAcpPromptResult.FromSuccess(sessionId!, streamCollector.ResultText);
        }

        var tail = await CollectPromptUpdatesAfterRpcAsync(sessionId!, streamCollector, cancellationToken)
            .ConfigureAwait(false);
        return tail;
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

    private async Task<CursorAcpPromptResult> CollectPromptUpdatesAfterRpcAsync(
        string sessionId,
        CursorAcpPromptStreamCollector collector,
        CancellationToken cancellationToken)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_ioTimeout);

        while (!timeoutCts.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await _transport.ReadLineAsync(timeoutCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                break;
            }

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

            if (message.IsServerRequest)
            {
                await HandleServerRequestAsync(message, sessionId, collector, cancellationToken)
                    .ConfigureAwait(false);
                continue;
            }

            if (message.IsNotification)
            {
                collector.TryAccumulate(message);
                if (collector.IsComplete)
                {
                    return CursorAcpPromptResult.FromSuccess(sessionId, collector.ResultText);
                }

                continue;
            }
        }

        return CursorAcpPromptResult.FromFailure(
            EngineeringAgentProviderFailureKind.TimedOut,
            "Timed out waiting for ACP prompt completion.");
    }

    private async Task<CursorAcpInboundMessage> SendRequestAsync(
        string method,
        object parameters,
        CancellationToken cancellationToken,
        CursorAcpPromptStreamCollector? streamCollector = null)
    {
        var id = _nextRequestId++;
        var payload = CursorAcpNdjsonCodec.SerializeRequest(method, parameters, id);
        await _transport.WriteLineAsync(payload, cancellationToken).ConfigureAwait(false);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(_ioTimeout);

        while (!cancellationToken.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await _transport.ReadLineAsync(timeoutCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                break;
            }

            if (line is null)
            {
                break;
            }

            if (!CursorAcpNdjsonCodec.TryParseLine(line, out var message))
            {
                continue;
            }

            if (message.IsServerRequest)
            {
                await HandleServerRequestAsync(
                        message,
                        streamCollector?.SessionId ?? string.Empty,
                        streamCollector,
                        cancellationToken)
                    .ConfigureAwait(false);
                continue;
            }

            if (message.IsNotification)
            {
                streamCollector?.TryAccumulate(message);
                continue;
            }

            if (message.Id == id)
            {
                return message;
            }
        }

        return new CursorAcpInboundMessage(null, id, true, """{"error":{"message":"timeout"}}""");
    }

    private async Task HandleServerRequestAsync(
        CursorAcpInboundMessage message,
        string sessionId,
        CursorAcpPromptStreamCollector? streamCollector,
        CancellationToken cancellationToken)
    {
        if (message.Method == "session/request_permission"
            && message.Id is { } serverId
            && TryReadPermissionRequest(message.RawLine, out var requestId, out var kind, out var detail))
        {
            var decision = _permissionPolicy.Evaluate(new CursorAcpPermissionRequest(requestId, kind, detail));
            var wireDecision = decision.Disposition == EngineeringAgentProviderPermissionDisposition.AllowOnce
                ? "allow-once"
                : "deny";
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                await SendRequestAsync(
                    "session/permission_response",
                    new { sessionId, requestId, decision = wireDecision },
                    cancellationToken,
                    streamCollector).ConfigureAwait(false);
            }
            else
            {
                var response = CursorAcpNdjsonCodec.SerializeResponse(
                    serverId,
                    new { outcome = new { outcome = "selected", optionId = wireDecision } });
                await _transport.WriteLineAsync(response, cancellationToken).ConfigureAwait(false);
            }

            return;
        }

        if (message.Id is { } unhandledId)
        {
            var response = CursorAcpNdjsonCodec.SerializeResponse(
                unhandledId,
                new { outcome = new { outcome = "cancelled" } });
            await _transport.WriteLineAsync(response, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<(string? SessionId, EngineeringAgentProviderFailure? Failure)> PrepareVerifiedSessionAsync(
        string cursorMode,
        string governedProjectRootAbsolutePath,
        CursorEngineeringAgentModelSelection modelSelection,
        string? existingSessionId,
        CancellationToken cancellationToken)
    {
        var discoveryResponse = await SendRequestAsync(
            "session/new",
            CursorAcpProtocol.CreateSessionNewParameters(cursorMode, governedProjectRootAbsolutePath),
            cancellationToken).ConfigureAwait(false);
        if (discoveryResponse.HasError)
        {
            return (null, new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                DescribeError(discoveryResponse.RawLine)));
        }

        if (!CursorAcpProtocol.TryParseSessionNewResult(
                discoveryResponse.RawLine,
                out _,
                out _,
                out var availableModels))
        {
            return (null, new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                "ACP session/new did not return advertised models for model resolution."));
        }

        if (!CursorAcpModelResolver.TryResolveWireModelId(
                modelSelection,
                availableModels,
                out var resolvedWireModelId,
                out var resolveMessage))
        {
            return (null, new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                resolveMessage ?? "Cursor model resolution failed."));
        }

        string sessionId;
        if (string.IsNullOrWhiteSpace(existingSessionId))
        {
            var sessionResponse = await SendRequestAsync(
                "session/new",
                CursorAcpProtocol.CreateSessionNewParameters(
                    cursorMode,
                    governedProjectRootAbsolutePath,
                    resolvedWireModelId),
                cancellationToken).ConfigureAwait(false);
            if (sessionResponse.HasError)
            {
                return (null, new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                    DescribeError(sessionResponse.RawLine)));
            }

            if (!CursorAcpProtocol.TryParseSessionNewResult(
                    sessionResponse.RawLine,
                    out var newSessionId,
                    out var currentModelId,
                    out _)
                || string.IsNullOrWhiteSpace(newSessionId))
            {
                return (null, new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                    "ACP session/new did not return a session id for the configured model."));
            }

            sessionId = newSessionId;
            if (!CursorAcpModelResolver.VerifyCurrentModelId(
                    resolvedWireModelId,
                    modelSelection,
                    currentModelId ?? string.Empty,
                    out var verifyMessage))
            {
                return (null, new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                    verifyMessage ?? "Cursor session model verification failed."));
            }
        }
        else
        {
            sessionId = existingSessionId;
            var loadResponse = await SendRequestAsync(
                "session/load",
                CursorAcpProtocol.CreateSessionLoadParameters(sessionId),
                cancellationToken).ConfigureAwait(false);
            if (loadResponse.HasError)
            {
                return (null, new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.ForwardFailed,
                    DescribeError(loadResponse.RawLine)));
            }

            var setModelResponse = await SendRequestAsync(
                "session/set_model",
                CursorAcpProtocol.CreateSessionSetModelParameters(sessionId, resolvedWireModelId),
                cancellationToken).ConfigureAwait(false);
            if (setModelResponse.HasError)
            {
                return (null, new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.ModelConfigurationFailed,
                    DescribeError(setModelResponse.RawLine)));
            }
        }

        return (sessionId, null);
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

    private sealed class CursorAcpPromptStreamCollector
    {
        private readonly StringBuilder _builder = new();

        public CursorAcpPromptStreamCollector(string sessionId) => SessionId = sessionId;

        public string SessionId { get; }

        public bool IsComplete { get; private set; }

        public string ResultText => _builder.ToString();

        public void TryAccumulate(CursorAcpInboundMessage message)
        {
            if (message.Method != "session/update"
                || !TryReadSessionUpdate(message.RawLine, out var updateType, out var updateText))
            {
                return;
            }

            if (!string.IsNullOrEmpty(updateText))
            {
                _builder.Append(updateText);
            }

            if (IsPromptCompleteUpdate(updateType))
            {
                IsComplete = true;
            }
        }
    }

    private static bool IsPromptCompleteUpdate(string? updateType) =>
        string.Equals(updateType, "prompt_complete", StringComparison.OrdinalIgnoreCase)
        || string.Equals(updateType, "end_turn", StringComparison.OrdinalIgnoreCase);

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

        if (updateEl.TryGetProperty("sessionUpdate", out var sessionUpdateEl)
            && sessionUpdateEl.ValueKind == JsonValueKind.String)
        {
            updateType = sessionUpdateEl.GetString();
        }
        else if (updateEl.TryGetProperty("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String)
        {
            updateType = typeEl.GetString();
        }

        if (updateEl.TryGetProperty("content", out var contentEl)
            && contentEl.TryGetProperty("text", out var contentTextEl)
            && contentTextEl.ValueKind == JsonValueKind.String)
        {
            updateText = contentTextEl.GetString();
        }
        else if (updateEl.TryGetProperty("text", out var textEl) && textEl.ValueKind == JsonValueKind.String)
        {
            updateText = textEl.GetString();
        }

        return true;
    }
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
