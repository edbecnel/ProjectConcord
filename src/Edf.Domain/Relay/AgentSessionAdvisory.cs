namespace Edf.Domain.Relay;

/// <summary>
/// Optional relay recommendation; MUST NOT override explicit <see cref="AgentSessionIntent"/>.
/// </summary>
public enum AgentSessionAdvisoryKind
{
    None = 0,
    RecommendNew = 1,
    RecommendContinue = 2,
}

public readonly record struct AgentSessionAdvisory(AgentSessionAdvisoryKind Kind)
{
    public static AgentSessionAdvisory None => new(AgentSessionAdvisoryKind.None);
}
