namespace Edf.Domain.Relay;

/// <summary>
/// User session continuity for Project Architect and engineering-agent interactions.
/// </summary>
public sealed record RelaySessionContinuity(
    AgentSessionIntent? ProjectArchitectSessionIntent,
    AgentSessionAdvisory ProjectArchitectSessionAdvisory,
    AgentSessionIntent? EngineeringAgentSessionIntent,
    AgentSessionAdvisory EngineeringAgentSessionAdvisory);
