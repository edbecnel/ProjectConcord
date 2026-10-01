namespace Edf.Application.Operator;

/// <summary>
/// Next Action presentation class (ADR-0020 §4). Derived only — not governance authority.
/// </summary>
public enum OperatorNextActionClass
{
    Available = 0,
    Recommended = 1,
    Required = 2,
}
