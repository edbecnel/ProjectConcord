namespace Edf.Application.Workflow.Eligibility;

/// <summary>
/// Whether full governed actionability can be determined from authoritative inputs.
/// WF-1d never reports <see cref="FullyActionable"/>.
/// </summary>
public enum GovernedActionabilityCompleteness
{
    Indeterminate = 0,
    FullyActionable = 1,
    NotFullyActionable = 2,
}
