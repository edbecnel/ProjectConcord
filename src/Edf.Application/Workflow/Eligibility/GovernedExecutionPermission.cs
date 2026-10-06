namespace Edf.Application.Workflow.Eligibility;

/// <summary>
/// Derived execution permission for a suggested step — not governance authority.
/// </summary>
public enum GovernedExecutionPermission
{
    Indeterminate = 0,
    Denied = 1,
    NotApplicable = 2,
}
