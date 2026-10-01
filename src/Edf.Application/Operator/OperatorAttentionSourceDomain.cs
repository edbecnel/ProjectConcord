namespace Edf.Application.Operator;

/// <summary>
/// Derived facet: authoritative source domain for one Attention item (ADR-0020 §3). Noncanonical.
/// </summary>
public enum OperatorAttentionSourceDomain
{
    EngineeringAgentAutomatedTransport = 0,
    GovernedRelay = 1,
}
