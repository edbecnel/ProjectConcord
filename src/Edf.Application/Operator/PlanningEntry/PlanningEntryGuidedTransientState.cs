namespace Edf.Application.Operator.PlanningEntry;

using Edf.Application.Relay;

public sealed class PlanningEntryGuidedTransientState
{
    public bool ReviewCopied { get; set; }

    public bool AwaitingPaResponseAcknowledged { get; set; }

    public string PaResponseDraft { get; set; } = string.Empty;

    public string? LastOperatorValidationMessage { get; set; }

    public bool LastValidationAttemptFailed { get; set; }

    public PaHandoverCorrectionFailureClass LastCorrectionFailureClass { get; set; }

    public string? CachedRenderedReview { get; set; }

    public void ResetForNewReviewCycle()
    {
        ReviewCopied = false;
        AwaitingPaResponseAcknowledged = false;
        PaResponseDraft = string.Empty;
        LastOperatorValidationMessage = null;
        LastValidationAttemptFailed = false;
        LastCorrectionFailureClass = PaHandoverCorrectionFailureClass.None;
        CachedRenderedReview = null;
    }
}
