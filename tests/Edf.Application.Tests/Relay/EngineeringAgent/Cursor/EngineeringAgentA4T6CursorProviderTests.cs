using Edf.Application.Composition;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class EngineeringAgentA4T6CursorProviderTests
{
    [Fact]
    public void ReferencePluginId_IsStable()
    {
        Assert.Equal("cursor-acp-reference", CursorEngineeringAgentPluginIds.Reference.Value);
    }

    [Fact]
    public void ProductionCatalog_RegistersCursorProviderOnly()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        Assert.Single(services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds);
        Assert.Equal(
            CursorEngineeringAgentPluginIds.Reference.Value,
            services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds[0].Value);
    }

    [Fact]
    public void Capabilities_SupportPlanAndAgent_NotDebug()
    {
        var plugin = new CursorEngineeringAgentProviderPlugin(() => new ScriptedCursorAcpTransport([]));
        var caps = plugin.DeclareCapabilities();
        Assert.True(caps.RoutingIntentSupport.SupportsPlan);
        Assert.True(caps.RoutingIntentSupport.SupportsAgent);
        Assert.False(caps.RoutingIntentSupport.SupportsDebugSemantically);
    }

    [Fact]
    public void CompatibilityEvaluator_RejectsDebugRouting()
    {
        var plugin = new CursorEngineeringAgentProviderPlugin(() => new ScriptedCursorAcpTransport([]));
        var assessment = EngineeringAgentPluginCompatibilityEvaluator.Assess(
            plugin,
            RelayRenderVersion.V1.Major,
            EngineeringAgentMode.Debug);
        Assert.False(assessment.RoutingIntent.IsCompatible);
    }

    [Fact]
    public void CompatibilityEvaluator_AcceptsPlanAndAgent()
    {
        var plugin = new CursorEngineeringAgentProviderPlugin(() => new ScriptedCursorAcpTransport([]));
        Assert.True(
            EngineeringAgentPluginCompatibilityEvaluator.Assess(
                plugin,
                RelayRenderVersion.V1.Major,
                EngineeringAgentMode.Plan).RoutingIntent.IsCompatible);
        Assert.True(
            EngineeringAgentPluginCompatibilityEvaluator.Assess(
                plugin,
                RelayRenderVersion.V1.Major,
                EngineeringAgentMode.Agent).RoutingIntent.IsCompatible);
    }

    [Fact]
    public async Task Forward_Debug_ReturnsRoutingUnsupported()
    {
        var plugin = CreateScriptedPlugin(CursorAcpFixtures.MinimalLifecycleScript());
        await plugin.InitializeAsync();

        var forward = plugin.Forward(CreateForwardRequest(EngineeringAgentMode.Debug));
        Assert.False(forward.IsAcknowledged);
        Assert.Equal(EngineeringAgentProviderFailureKind.RoutingIntentUnsupported, forward.Failure!.Kind);
    }

    [Fact]
    public async Task Forward_Plan_MapsToPlanModeAndReturnsCandidate()
    {
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptSuccessScript());
        var plugin = new CursorEngineeringAgentProviderPlugin(() => transport);
        await plugin.InitializeAsync();

        var request = CreateForwardRequest(EngineeringAgentMode.Plan);
        var forward = plugin.Forward(request);
        Assert.True(forward.IsAcknowledged);
        var sessionNewLine = transport.WrittenLines.First(l => l.Contains("session/new", StringComparison.Ordinal));
        Assert.Contains("plan", sessionNewLine, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            request.GovernedProjectRoot.NormalizedAbsolutePath,
            sessionNewLine,
            StringComparison.Ordinal);

        var candidate = plugin.TryGetResultCandidate(request.TransportOperationId);
        Assert.True(candidate.HasCandidate);
        Assert.Contains("engineering-result", candidate.Candidate!.UntrustedImportText, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Forward_Agent_MapsToAgentMode()
    {
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptSuccessScript());
        var plugin = new CursorEngineeringAgentProviderPlugin(() => transport);
        await plugin.InitializeAsync();

        _ = plugin.Forward(CreateForwardRequest(EngineeringAgentMode.Agent));
        Assert.Contains("\"mode\":\"agent\"", transport.WrittenLines[2], StringComparison.Ordinal);
    }

    [Fact]
    public async Task Cancel_AcknowledgesForKnownSession()
    {
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptSuccessWithCancelScript());
        var plugin = new CursorEngineeringAgentProviderPlugin(() => transport);
        await plugin.InitializeAsync();
        var request = CreateForwardRequest(EngineeringAgentMode.Agent);
        _ = plugin.Forward(request);

        var cancel = plugin.Cancel(request.TransportOperationId);
        Assert.True(cancel.IsAcknowledged);
        Assert.Contains("session/cancel", transport.WrittenLines[^1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task PermissionRequest_DefaultPolicy_Denies()
    {
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptWithPermissionScript());
        var plugin = new CursorEngineeringAgentProviderPlugin(() => transport);
        await plugin.InitializeAsync();
        _ = plugin.Forward(CreateForwardRequest(EngineeringAgentMode.Plan));

        Assert.Contains(
            transport.WrittenLines,
            line => line.Contains("\"id\":99", StringComparison.Ordinal)
                    && line.Contains("deny", StringComparison.Ordinal));
    }

    [Fact]
    public void P0Workflow_RemainsAvailable_WithCursorRegistered()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var projectId = Edf.Domain.Projects.ProjectConcordProjectId.New();
        services.RelayWorkflow.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.Continue);
        Assert.NotNull(services.RelayWorkflow);
        Assert.Single(services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds);
    }

    [Fact]
    public async Task Initialize_WithEmptyScript_ReturnsInitializationFailure()
    {
        var plugin = new CursorEngineeringAgentProviderPlugin(() => new ScriptedCursorAcpTransport([]));
        var result = await plugin.InitializeAsync();
        Assert.NotNull(result.Failure);
        Assert.Equal(EngineeringAgentProviderFailureKind.InitializationFailed, result.Failure!.Kind);
    }

    [Fact]
    public async Task PermissionRequest_AllowOncePolicy_WiresAllowOnceResponse()
    {
        var policy = new CursorAcpPermissionPolicy
        {
            Evaluate = _ => new EngineeringAgentProviderPermissionDecision(
                EngineeringAgentProviderPermissionDisposition.AllowOnce),
        };
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptWithPermissionScript());
        var plugin = new CursorEngineeringAgentProviderPlugin(() => transport, modelPreferences: null, policy);
        await plugin.InitializeAsync();
        _ = plugin.Forward(CreateForwardRequest(EngineeringAgentMode.Plan));

        Assert.Contains(
            transport.WrittenLines,
            line => line.Contains("allow-once", StringComparison.Ordinal));
    }

    [Trait("RequiresCursor", "true")]
    [Fact]
    public async Task LiveCursorAcp_InitializeHandshake_Succeeds()
    {
        var transport = new CursorAcpSubprocessTransport();
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(20));
        var result = await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        if (result.Failure?.Kind == EngineeringAgentProviderFailureKind.InitializationFailed)
        {
            Assert.Fail(result.Failure.Message);
        }

        Assert.True(client.IsInitialized);
        if (result.Failure?.Kind == EngineeringAgentProviderFailureKind.AuthenticationUnavailable)
        {
            Assert.False(client.IsAuthenticated);
            return;
        }

        Assert.True(result.Succeeded);
        Assert.True(client.IsAuthenticated);
        await client.DisposeAsync();
    }

    private static CursorEngineeringAgentProviderPlugin CreateScriptedPlugin(IEnumerable<string> script) =>
        new(() => new ScriptedCursorAcpTransport(script));

    private static EngineeringAgentForwardRequest CreateForwardRequest(EngineeringAgentMode mode)
    {
        var operationId = TransportOperationId.New();
        return new EngineeringAgentForwardRequest(
            ProjectConcordProjectId.New(),
            operationId,
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            mode,
            "handover-body",
            RelayTestFixtures.SessionWithBothIntents(),
            ProviderSessionHint: null,
            ProjectLocator.FromPath("/tmp/projectconcord-test-root-a"));
    }
}

internal static class CursorAcpFixtures
{
    public static IEnumerable<string> MinimalLifecycleScript() =>
    [
        InitializeSuccessLine(1),
        """{"jsonrpc":"2.0","id":2,"result":{}}""",
    ];

    public static IEnumerable<string> PromptSuccessScript() =>
    [
        InitializeSuccessLine(1),
        """{"jsonrpc":"2.0","id":2,"result":{}}""",
        SessionNewDiscoveryLine(3),
        SessionNewConfiguredLine(4),
        """{"jsonrpc":"2.0","method":"session/update","params":{"update":{"sessionUpdate":"agent_message_chunk","content":{"type":"text","text":"engineering-result"}}}}""",
        """{"jsonrpc":"2.0","id":5,"result":{"stopReason":"end_turn"}}""",
    ];

    public static string SessionNewDiscoveryLine(int id) =>
        "{\"jsonrpc\":\"2.0\",\"id\":" + id
        + ",\"result\":{\"sessionId\":\"disc-sess\",\"models\":{\"currentModelId\":\"composer-2.5[fast=true]\",\"availableModels\":[{\"modelId\":\"composer-2.5\",\"name\":\"composer-2.5\"}]}}}";

    public static string SessionNewConfiguredLine(int id) =>
        "{\"jsonrpc\":\"2.0\",\"id\":" + id
        + ",\"result\":{\"sessionId\":\"sess-1\",\"models\":{\"currentModelId\":\"composer-2.5\",\"availableModels\":[{\"modelId\":\"composer-2.5\",\"name\":\"composer-2.5\"}]}}}";

    public static IEnumerable<string> PromptLiveCursorOrderScript() => PromptSuccessScript();

    public static string InitializeSuccessLine(int id) =>
        "{\"jsonrpc\":\"2.0\",\"id\":" + id + ",\"result\":{\"protocolVersion\":1}}";

    public static IEnumerable<string> PromptSuccessWithCancelScript()
    {
        foreach (var line in PromptSuccessScript())
        {
            yield return line;
        }

        yield return """{"jsonrpc":"2.0","id":6,"result":{}}""";
    }

    public static IEnumerable<string> PromptWithPermissionScript() =>
    [
        InitializeSuccessLine(1),
        """{"jsonrpc":"2.0","id":2,"result":{}}""",
        SessionNewDiscoveryLine(3),
        SessionNewConfiguredLine(4),
        """{"jsonrpc":"2.0","id":99,"method":"session/request_permission","params":{"requestId":"perm-1","permission":"tool"}}""",
        """{"jsonrpc":"2.0","id":5,"result":{"stopReason":"end_turn"}}""",
        """{"jsonrpc":"2.0","method":"session/update","params":{"update":{"sessionUpdate":"agent_message_chunk","content":{"type":"text","text":"engineering-result"}}}}""",
    ];
}
