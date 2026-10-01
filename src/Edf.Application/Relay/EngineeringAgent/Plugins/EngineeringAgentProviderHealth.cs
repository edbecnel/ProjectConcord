namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Neutral availability / authentication readiness facets for hosting and orchestration (ADR-0021 §2).
/// </summary>
public sealed record EngineeringAgentProviderHealth(
    bool IsInitialized,
    bool IsAuthenticated,
    string? StatusMessage)
{
    public static EngineeringAgentProviderHealth Unavailable(string? message = null) =>
        new(IsInitialized: false, IsAuthenticated: false, StatusMessage: message);
}
