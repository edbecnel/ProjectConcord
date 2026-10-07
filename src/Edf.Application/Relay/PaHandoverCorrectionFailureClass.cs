namespace Edf.Application.Relay;

/// <summary>
/// Safe, fixed classification of a failed PA handover paste for operator/PA correction requests.
/// </summary>
public enum PaHandoverCorrectionFailureClass
{
    None = 0,
    Unrecognized = 1,
    Incomplete = 2,
    Ambiguous = 3,
    StructuralMalformed = 4,
    GovernanceProjectionInconsistent = 5,
    ProjectIdentityMismatch = 6,
    CorrelationMismatch = 7,
    GovernanceNonQualifying = 8,
}
