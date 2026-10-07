namespace Edf.Application.Operator.PlanningRegion;

using Edf.Application.Operator;
using Edf.Application.Relay;
using Edf.Domain.Relay;

public sealed class PlanningRegionGuidedTransientState
{
    public string? CachedRenderedReview { get; set; }

    public bool ReviewTransferred { get; set; }

    public bool AwaitingPaResponseAcknowledged { get; set; }

    public string PaResponseDraft { get; set; } = string.Empty;

    public bool ImportAttestationConfirmed { get; set; }

    public bool LastValidationAttemptFailed { get; set; }

    public PaHandoverCorrectionFailureClass LastCorrectionFailureClass { get; set; }

    public string? LastOperatorValidationMessage { get; set; }

    public GovernedRelayPackage? LastCommittedHandoverPackage { get; set; }

    public RelayValidationResult? LastCommittedHandoverValidation { get; set; }

    public string? CachedEngineeringHandover { get; set; }

    public void ResetForNewPaCycle()
    {
        CachedRenderedReview = null;
        ReviewTransferred = false;
        AwaitingPaResponseAcknowledged = false;
        PaResponseDraft = string.Empty;
        ImportAttestationConfirmed = false;
        LastValidationAttemptFailed = false;
        LastCorrectionFailureClass = PaHandoverCorrectionFailureClass.None;
        LastOperatorValidationMessage = null;
    }
}
