namespace Edf.Domain.Relay;

public sealed record EngineeringAgentModeTransition(
    EngineeringAgentMode From,
    EngineeringAgentMode To);
