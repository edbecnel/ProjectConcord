namespace Edf.Application.Workflow.Eligibility;

/// <summary>
/// Whether a governance dimension applies to the current operation and, if so, evaluation state.
/// </summary>
public enum GovernedEligibilityDimensionApplicability
{
    NotApplicableToOperation = 0,
    ApplicableUnevaluated = 1,
    ApplicabilityUnknown = 2,
}
