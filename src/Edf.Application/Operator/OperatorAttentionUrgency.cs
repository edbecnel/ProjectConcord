namespace Edf.Application.Operator;

/// <summary>
/// Derived urgency facet when derivable from governing rules (ADR-0020 §3). Noncanonical.
/// </summary>
public enum OperatorAttentionUrgency
{
    Informational = 0,
    Recommended = 1,
    Required = 2,
}
