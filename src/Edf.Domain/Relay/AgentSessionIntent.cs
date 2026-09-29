namespace Edf.Domain.Relay;

/// <summary>
/// Explicit user-selected NEW/CONTINUE session intent (not inferred from prose or advisories).
/// </summary>
public enum AgentSessionIntent
{
    New = 0,
    Continue = 1,
}
