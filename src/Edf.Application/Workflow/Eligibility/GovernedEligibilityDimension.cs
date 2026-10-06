namespace Edf.Application.Workflow.Eligibility;

/// <summary>
/// Governance dimensions that may affect full actionability when applicable to an operation.
/// </summary>
public enum GovernedEligibilityDimension
{
    SynchronizationPoint = 0,
    AuthorizedExecutionInterval = 1,
    EvidenceRequirement = 2,
    ControlBeyondDwaKindAtPlace = 3,
    ConditionalAlternateSynchronization = 4,
    RelayPackageApplicability = 5,
}
