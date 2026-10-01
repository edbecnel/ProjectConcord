namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Collections.Concurrent;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

/// <summary>
/// First-party Cursor Engineering Agent provider (ACP reference realization — ADR-0022 §16).
/// </summary>
public sealed class CursorEngineeringAgentProviderPlugin : IEngineeringAgentProviderPlugin
{
    private readonly Func<ICursorAcpTransport> _transportFactory;
    private readonly CursorAcpPermissionPolicy _permissionPolicy;
    private readonly ConcurrentDictionary<TransportOperationId, CursorTransportOperationState> _operations = new();
    private CursorAcpClient? _client;
    private bool _lifecycleInitialized;

    public CursorEngineeringAgentProviderPlugin()
        : this(static () => new CursorAcpSubprocessTransport())
    {
    }

    internal CursorEngineeringAgentProviderPlugin(
        Func<ICursorAcpTransport> transportFactory,
        CursorAcpPermissionPolicy? permissionPolicy = null)
    {
        _transportFactory = transportFactory ?? throw new ArgumentNullException(nameof(transportFactory));
        _permissionPolicy = permissionPolicy ?? new CursorAcpPermissionPolicy();
    }

    public EngineeringAgentProviderPluginId PluginId => CursorEngineeringAgentPluginIds.Reference;

    public EngineeringAgentProviderCapabilities DeclareCapabilities() =>
        new(
            SupportsAutomatedTransport: true,
            SupportedRenderProtocolMajor: RelayRenderVersion.V1.Major,
            RoutingIntentSupport: new EngineeringAgentRoutingIntentSupport(
                SupportsPlan: true,
                SupportsAgent: true,
                SupportsDebugSemantically: false),
            IsAvailableForSelection: true);

    public EngineeringAgentProviderHealth GetHealth()
    {
        if (!_lifecycleInitialized || _client is null)
        {
            return EngineeringAgentProviderHealth.Unavailable("Cursor provider lifecycle is not initialized.");
        }

        return new EngineeringAgentProviderHealth(
            _client.IsInitialized,
            _client.IsAuthenticated,
            _client.IsAuthenticated ? null : "Cursor ACP is not authenticated.");
    }

    public async Task<EngineeringAgentProviderInitializeResult> InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        if (_lifecycleInitialized && _client is not null)
        {
            return new EngineeringAgentProviderInitializeResult(GetHealth(), Failure: null);
        }

        var transport = _transportFactory();
        _client = new CursorAcpClient(transport, _permissionPolicy);
        var result = await _client.InitializeAndAuthenticateAsync(cancellationToken).ConfigureAwait(false);
        if (result.Failure is not null)
        {
            await _client.DisposeAsync().ConfigureAwait(false);
            _client = null;
            _lifecycleInitialized = false;
            return result;
        }

        _lifecycleInitialized = true;
        return result;
    }

    public async Task<EngineeringAgentProviderShutdownResult> ShutdownAsync(
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        _operations.Clear();
        if (_client is not null)
        {
            await _client.DisposeAsync().ConfigureAwait(false);
            _client = null;
        }

        _lifecycleInitialized = false;
        return new EngineeringAgentProviderShutdownResult(true, null);
    }

    public EngineeringAgentForwardResult Forward(
        EngineeringAgentForwardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_client is null || !_lifecycleInitialized)
        {
            return new EngineeringAgentForwardResult(
                false,
                null,
                new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.Unavailable,
                    "Cursor provider is not initialized."));
        }

        if (!CursorRoutingIntentMapper.TryToCursorAcpMode(request.RoutingIntent, out var cursorMode, out var routingFailure))
        {
            return new EngineeringAgentForwardResult(false, null, routingFailure);
        }

        var existingSession = request.ProviderSessionHint?.Value;
        var prompt = _client.RunPromptAsync(cursorMode, request.RenderedHandoverBody, existingSession, cancellationToken)
            .GetAwaiter()
            .GetResult();
        if (prompt.Failure is not null)
        {
            return new EngineeringAgentForwardResult(false, null, prompt.Failure);
        }

        var state = new CursorTransportOperationState
        {
            TransportOperationId = request.TransportOperationId,
            SessionId = prompt.SessionId,
            ForwardAcknowledged = true,
            ResultCandidate = string.IsNullOrWhiteSpace(prompt.ResultText)
                ? null
                : new EngineeringAgentTransportResultCandidate(
                    request.TransportOperationId,
                    prompt.ResultText!,
                    IsReadyForParse: true),
        };
        _operations[request.TransportOperationId] = state;

        return new EngineeringAgentForwardResult(
            true,
            prompt.SessionId is null
                ? null
                : EngineeringAgentProviderSessionHandle.FromOpaque(prompt.SessionId),
            Failure: null);
    }

    public EngineeringAgentResultCandidateResult TryGetResultCandidate(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        if (!_operations.TryGetValue(transportOperationId, out var state)
            || state.ResultCandidate is null)
        {
            return new EngineeringAgentResultCandidateResult(false, null, null);
        }

        return new EngineeringAgentResultCandidateResult(true, state.ResultCandidate, null);
    }

    public EngineeringAgentCancelResult Cancel(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        if (_client is null)
        {
            return new EngineeringAgentCancelResult(
                false,
                new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.Unavailable,
                    "Cursor provider is not initialized."));
        }

        if (!_operations.TryGetValue(transportOperationId, out var state)
            || string.IsNullOrWhiteSpace(state.SessionId))
        {
            return new EngineeringAgentCancelResult(
                false,
                new EngineeringAgentProviderFailure(
                    EngineeringAgentProviderFailureKind.CancelFailed,
                    "No Cursor session is associated with the transport operation."));
        }

        return _client.CancelSessionAsync(state.SessionId!, cancellationToken).GetAwaiter().GetResult();
    }
}
