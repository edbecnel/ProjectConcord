namespace Edf.Application.Relay;

using Edf.Domain.Relay;

/// <summary>
/// Maps relay validation outcomes to retryable transport failures vs governance outcomes.
/// </summary>
public static class PaHandoverCorrectionFailureClassifier
{
    public static PaHandoverCorrectionFailureClass ClassifyImportFailure(RelayValidationResult validation)
    {
        ArgumentNullException.ThrowIfNull(validation);
        if (validation.State == RelayValidationState.Valid)
        {
            return PaHandoverCorrectionFailureClass.None;
        }

        var codes = validation.Diagnostics.Select(d => d.Code).ToHashSet(StringComparer.Ordinal);

        if (codes.Contains(RelayValidationCodes.ManualPasteRenderMarkerAmbiguous)
            || codes.Contains(RelayValidationCodes.TransportExtractionStartMarkerAmbiguous)
            || codes.Contains(RelayValidationCodes.TransportExtractionMachineBlockAmbiguous)
            || codes.Contains(RelayValidationCodes.TransportExtractionEndBoundaryAmbiguous))
        {
            return PaHandoverCorrectionFailureClass.Ambiguous;
        }

        if (codes.Contains(RelayValidationCodes.ManualPasteIncomplete)
            || codes.Contains(RelayValidationCodes.TransportExtractionEndBoundaryMissing))
        {
            return PaHandoverCorrectionFailureClass.Incomplete;
        }

        if (codes.Contains(RelayValidationCodes.GovernanceProjectionMismatch))
        {
            return PaHandoverCorrectionFailureClass.GovernanceProjectionInconsistent;
        }

        if (codes.Contains(RelayValidationCodes.HandoverCorrelationMismatch))
        {
            return PaHandoverCorrectionFailureClass.CorrelationMismatch;
        }

        if (codes.Contains(RelayValidationCodes.MachineBlockInvalidJson))
        {
            return PaHandoverCorrectionFailureClass.StructuralMalformed;
        }

        return PaHandoverCorrectionFailureClass.Unrecognized;
    }

    public static bool IsRetryable(PaHandoverCorrectionFailureClass failureClass) =>
        failureClass is PaHandoverCorrectionFailureClass.Unrecognized
            or PaHandoverCorrectionFailureClass.Incomplete
            or PaHandoverCorrectionFailureClass.Ambiguous
            or PaHandoverCorrectionFailureClass.StructuralMalformed
            or PaHandoverCorrectionFailureClass.GovernanceProjectionInconsistent
            or PaHandoverCorrectionFailureClass.ProjectIdentityMismatch
            or PaHandoverCorrectionFailureClass.CorrelationMismatch;

    public static bool OffersCorrectionRequest(PaHandoverCorrectionFailureClass failureClass) =>
        IsRetryable(failureClass);

    public static PaHandoverCorrectionFailureClass ClassifyGovernanceNonQualifying() =>
        PaHandoverCorrectionFailureClass.GovernanceNonQualifying;
}
