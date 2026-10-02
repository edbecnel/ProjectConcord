using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class CursorAcpModelConfigurationClientTests
{
    private const string TestRoot = "/tmp/projectconcord-acp-model-config-root";

    [Fact]
    public async Task RunPromptAsync_SetModelRejection_DoesNotReachPrompt()
    {
        var script = new List<string>
        {
            CursorAcpFixtures.InitializeSuccessLine(1),
            """{"jsonrpc":"2.0","id":2,"result":{}}""",
            CursorAcpFixtures.SessionNewDiscoveryLine(3),
            """{"jsonrpc":"2.0","id":4,"error":{"code":-32602,"message":"Invalid model value: composer-2.5"}}""",
        };
        var transport = new ScriptedCursorAcpTransport(script);
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(5));
        await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        var result = await client.RunPromptAsync(
            "agent",
            "body",
            TestRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(EngineeringAgentProviderFailureKind.ModelConfigurationFailed, result.Failure?.Kind);
        Assert.DoesNotContain("session/prompt", string.Join('\n', transport.WrittenLines), StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunPromptAsync_VerifiedNonFast_AllowsPrompt()
    {
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptSuccessScript());
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(5));
        await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        var result = await client.RunPromptAsync(
            "agent",
            "body",
            TestRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Contains("session/prompt", string.Join('\n', transport.WrittenLines), StringComparison.Ordinal);
        var configuredNew = transport.WrittenLines.First(l => l.Contains("composer-2.5\"", StringComparison.Ordinal));
        Assert.Contains("modelId", configuredNew, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunPromptAsync_OnlyFastAvailable_FailsBeforePrompt()
    {
        var script = new List<string>
        {
            CursorAcpFixtures.InitializeSuccessLine(1),
            """{"jsonrpc":"2.0","id":2,"result":{}}""",
            """{"jsonrpc":"2.0","id":3,"result":{"sessionId":"disc","models":{"currentModelId":"composer-2.5[fast=true]","availableModels":[{"modelId":"composer-2.5[fast=true]","name":"composer-2.5"}]}}}""",
        };
        var transport = new ScriptedCursorAcpTransport(script);
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(5));
        await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        var result = await client.RunPromptAsync(
            "agent",
            "body",
            TestRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(EngineeringAgentProviderFailureKind.ModelConfigurationFailed, result.Failure?.Kind);
        Assert.DoesNotContain("session/prompt", string.Join('\n', transport.WrittenLines), StringComparison.Ordinal);
    }
}
