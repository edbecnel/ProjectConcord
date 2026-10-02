namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Text;
using System.Text.Json;

/// <summary>
/// Newline-delimited JSON-RPC framing for Cursor CLI ACP stdio (provider-internal).
/// </summary>
internal static class CursorAcpNdjsonCodec
{
    internal static string SerializeRequest(string method, object? parameters, int id)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("jsonrpc", "2.0");
            writer.WriteNumber("id", id);
            writer.WriteString("method", method);
            if (parameters is not null)
            {
                writer.WritePropertyName("params");
                JsonSerializer.Serialize(writer, parameters);
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    internal static string SerializeResponse(int id, object result)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("jsonrpc", "2.0");
            writer.WriteNumber("id", id);
            writer.WritePropertyName("result");
            JsonSerializer.Serialize(writer, result);
            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    internal static bool TryParseLine(string line, out CursorAcpInboundMessage message)
    {
        message = default!;
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        try
        {
            using var doc = JsonDocument.Parse(line);
            var root = doc.RootElement;
            var method = root.TryGetProperty("method", out var methodEl)
                ? methodEl.GetString()
                : null;
            var id = root.TryGetProperty("id", out var idEl) && idEl.ValueKind == JsonValueKind.Number
                ? idEl.GetInt32()
                : (int?)null;
            var hasError = root.TryGetProperty("error", out _);

            message = new CursorAcpInboundMessage(method, id, hasError, line);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

internal readonly record struct CursorAcpInboundMessage(
    string? Method,
    int? Id,
    bool HasError,
    string RawLine)
{
    public bool IsResponse => Id is not null && Method is null;

    public bool IsNotification => Id is null && Method is not null;

    public bool IsServerRequest => Id is not null && Method is not null;
}
