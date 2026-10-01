using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class CursorAcpNdjsonCodecTests
{
    [Fact]
    public void SerializeRequest_ProducesJsonRpcEnvelope()
    {
        var line = CursorAcpNdjsonCodec.SerializeRequest("initialize", new { clientName = "ProjectConcord" }, 1);
        Assert.Contains("\"jsonrpc\":\"2.0\"", line, StringComparison.Ordinal);
        Assert.Contains("\"method\":\"initialize\"", line, StringComparison.Ordinal);
        Assert.Contains("\"id\":1", line, StringComparison.Ordinal);
    }

    [Fact]
    public void TryParseLine_ParsesResponse()
    {
        Assert.True(
            CursorAcpNdjsonCodec.TryParseLine(
                """{"jsonrpc":"2.0","id":2,"result":{"sessionId":"abc"}}""",
                out var message));
        Assert.True(message.IsResponse);
        Assert.Equal(2, message.Id);
    }
}
