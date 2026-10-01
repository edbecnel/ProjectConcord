namespace Edf.Application.Relay.EngineeringAgent.Plugins;

/// <summary>
/// Neutral provider/transport failure classes for orchestration and operator projections (ADR-0021 §2, ADR-0022 §14).
/// </summary>
public enum EngineeringAgentProviderFailureKind
{
    Unavailable = 0,
    Incompatible = 1,
    AuthenticationUnavailable = 2,
    InitializationFailed = 3,
    ForwardFailed = 4,
    ResultRetrievalFailed = 5,
    CancelFailed = 6,
    TimedOut = 7,
    AmbiguousOutcome = 8,
    RoutingIntentUnsupported = 9,
}
