using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

internal sealed class FakeEngineeringAgentProviderPlugin : IEngineeringAgentProviderPlugin
{
    public FakeEngineeringAgentProviderPlugin(
        EngineeringAgentProviderPluginId pluginId,
        EngineeringAgentProviderCapabilities? capabilities = null,
        bool initializeSucceeds = true,
        bool shutdownSucceeds = true,
        bool reportsAuthenticatedWhenInitialized = true,
        bool supportsDebug = false,
        int renderProtocolMajor = 1)
    {
        PluginId = pluginId;
        _initializeSucceeds = initializeSucceeds;
        _shutdownSucceeds = shutdownSucceeds;
        _reportsAuthenticatedWhenInitialized = reportsAuthenticatedWhenInitialized;
        _capabilities = capabilities ?? new EngineeringAgentProviderCapabilities(
            SupportsAutomatedTransport: true,
            SupportedRenderProtocolMajor: renderProtocolMajor,
            RoutingIntentSupport: new EngineeringAgentRoutingIntentSupport(
                SupportsPlan: true,
                SupportsAgent: true,
                SupportsDebugSemantically: supportsDebug),
            IsAvailableForSelection: true);
    }

    private readonly EngineeringAgentProviderCapabilities _capabilities;
    private readonly bool _initializeSucceeds;
    private readonly bool _shutdownSucceeds;
    private readonly bool _reportsAuthenticatedWhenInitialized;
    private volatile bool _lifecycleInitialized;

    public int InitializeAsyncCallCount { get; private set; }

    public int ShutdownAsyncCallCount { get; private set; }

    public int ForwardCallCount { get; private set; }

    public EngineeringAgentForwardRequest? LastForwardRequest { get; private set; }

    public int CancelCallCount { get; private set; }

    public Func<EngineeringAgentForwardRequest, EngineeringAgentForwardResult>? ForwardHandler { get; set; }

    public Func<TransportOperationId, EngineeringAgentResultCandidateResult>? CandidateHandler { get; set; }

    public Func<TransportOperationId, EngineeringAgentCancelResult>? CancelHandler { get; set; }

    public EngineeringAgentProviderPluginId PluginId { get; }

    public EngineeringAgentProviderCapabilities DeclareCapabilities() => _capabilities;

    public EngineeringAgentProviderHealth GetHealth()
    {
        if (!_lifecycleInitialized)
        {
            return EngineeringAgentProviderHealth.Unavailable("Plugin lifecycle is not initialized.");
        }

        return new EngineeringAgentProviderHealth(
            IsInitialized: true,
            IsAuthenticated: _reportsAuthenticatedWhenInitialized,
            StatusMessage: null);
    }

    public Task<EngineeringAgentProviderInitializeResult> InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        InitializeAsyncCallCount++;

        if (_lifecycleInitialized)
        {
            return Task.FromResult(new EngineeringAgentProviderInitializeResult(GetHealth(), Failure: null));
        }

        if (!_initializeSucceeds)
        {
            var failure = new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.InitializationFailed,
                "Fake plugin initialization failed.");
            return Task.FromResult(
                new EngineeringAgentProviderInitializeResult(
                    EngineeringAgentProviderHealth.Unavailable(failure.Message),
                    failure));
        }

        _lifecycleInitialized = true;
        return Task.FromResult(new EngineeringAgentProviderInitializeResult(GetHealth(), Failure: null));
    }

    public Task<EngineeringAgentProviderShutdownResult> ShutdownAsync(
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        ShutdownAsyncCallCount++;

        if (!_lifecycleInitialized)
        {
            return Task.FromResult(new EngineeringAgentProviderShutdownResult(IsAcknowledged: true, Failure: null));
        }

        if (!_shutdownSucceeds)
        {
            return Task.FromResult(
                new EngineeringAgentProviderShutdownResult(
                    IsAcknowledged: false,
                    new EngineeringAgentProviderFailure(
                        EngineeringAgentProviderFailureKind.Unavailable,
                        "Fake plugin shutdown failed.")));
        }

        _lifecycleInitialized = false;
        return Task.FromResult(new EngineeringAgentProviderShutdownResult(IsAcknowledged: true, Failure: null));
    }

    public EngineeringAgentForwardResult Forward(
        EngineeringAgentForwardRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        ForwardCallCount++;
        LastForwardRequest = request;
        if (ForwardHandler is not null)
        {
            return ForwardHandler(request);
        }

        return new EngineeringAgentForwardResult(
            IsAcknowledged: true,
            UpdatedSessionHint: EngineeringAgentProviderSessionHandle.FromOpaque("fake-session"),
            Failure: null);
    }

    public EngineeringAgentResultCandidateResult TryGetResultCandidate(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        if (CandidateHandler is not null)
        {
            return CandidateHandler(transportOperationId);
        }

        return new EngineeringAgentResultCandidateResult(
            HasCandidate: false,
            Candidate: null,
            Failure: null);
    }

    public EngineeringAgentCancelResult Cancel(
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        CancelCallCount++;
        if (CancelHandler is not null)
        {
            return CancelHandler(transportOperationId);
        }

        return new EngineeringAgentCancelResult(IsAcknowledged: true, Failure: null);
    }
}
