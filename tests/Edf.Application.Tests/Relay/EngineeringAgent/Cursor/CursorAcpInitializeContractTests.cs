using System.Text.Json;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class CursorAcpInitializeContractTests
{
    [Fact]
    public async Task InitializeRequest_IncludesRequiredProtocolVersionAndClientInfo()
    {
        var transport = new ScriptedCursorAcpTransport([CursorAcpFixtures.InitializeSuccessLine(1), """{"jsonrpc":"2.0","id":2,"result":{}}"""]);
        var client = new CursorAcpClient(transport);
        _ = await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        var initializeLine = transport.WrittenLines[0];
        using var doc = JsonDocument.Parse(initializeLine);
        var root = doc.RootElement;
        Assert.Equal("initialize", root.GetProperty("method").GetString());
        var parameters = root.GetProperty("params");
        Assert.Equal(JsonValueKind.Number, parameters.GetProperty("protocolVersion").ValueKind);
        Assert.Equal(1, parameters.GetProperty("protocolVersion").GetInt32());
        Assert.Equal("ProjectConcord", parameters.GetProperty("clientInfo").GetProperty("name").GetString());
        Assert.False(parameters.GetProperty("clientCapabilities").GetProperty("terminal").GetBoolean());
    }

    [Fact]
    public async Task Authenticate_ProceedsOnlyAfterSuccessfulInitialize()
    {
        var transport = new ScriptedCursorAcpTransport(
        [
            CursorAcpFixtures.InitializeSuccessLine(1),
            """{"jsonrpc":"2.0","id":2,"result":{}}""",
        ]);
        var client = new CursorAcpClient(transport);
        var result = await client.InitializeAndAuthenticateAsync(CancellationToken.None);
        Assert.True(result.Succeeded);
        Assert.Equal(2, transport.WrittenLines.Count);
        Assert.Contains("cursor_login", transport.WrittenLines[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task InitializeError_ReturnsNeutralInitializationFailure()
    {
        var transport = new ScriptedCursorAcpTransport(
        [
            """{"jsonrpc":"2.0","id":1,"error":{"message":"protocol mismatch"}}""",
        ]);
        var client = new CursorAcpClient(transport);
        var result = await client.InitializeAndAuthenticateAsync(CancellationToken.None);
        Assert.False(result.Succeeded);
        Assert.Equal(EngineeringAgentProviderFailureKind.InitializationFailed, result.Failure!.Kind);
        Assert.Single(transport.WrittenLines);
    }
}
