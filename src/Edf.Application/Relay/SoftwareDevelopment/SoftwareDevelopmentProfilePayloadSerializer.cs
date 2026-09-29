namespace Edf.Application.Relay.SoftwareDevelopment;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class SoftwareDevelopmentProfilePayloadSerializer
{
    public const int CurrentPayloadVersion = 1;

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public static SoftwareDevelopmentProfilePayload Empty =>
        new(
            CurrentPayloadVersion,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

    public static bool TryDeserialize(
        ReadOnlyMemory<byte> payloadBytes,
        out SoftwareDevelopmentProfilePayload? payload,
        out string? error)
    {
        payload = null;
        error = null;

        if (payloadBytes.IsEmpty)
        {
            payload = Empty;
            return true;
        }

        var text = Encoding.UTF8.GetString(payloadBytes.Span).Trim();
        if (text.Length == 0 || text == "{}")
        {
            payload = Empty;
            return true;
        }

        try
        {
            var deserialized = JsonSerializer.Deserialize<SoftwareDevelopmentProfilePayload>(text, Options);
            if (deserialized is null)
            {
                error = "Profile payload JSON deserialized to null.";
                return false;
            }

            payload = deserialized with
            {
                PayloadVersion = deserialized.PayloadVersion == 0 ? CurrentPayloadVersion : deserialized.PayloadVersion,
            };
            return true;
        }
        catch (JsonException ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public static byte[] Serialize(SoftwareDevelopmentProfilePayload payload) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload, Options));
}
