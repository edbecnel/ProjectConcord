namespace Edf.Application.Operator;

using Edf.Application.Relay;
using Edf.Domain.Relay;

/// <summary>
/// Shared guided-exchange logic for PA handover validation failures and correction requests.
/// </summary>
public static class PaHandoverExchangeCorrectionSupport
{
    public static PaHandoverCorrectionFailureClass ClassifyValidationFailure(
        PaHandoverImportOperationResult result)
    {
        if (!result.ProjectIdMatched)
        {
            return PaHandoverCorrectionFailureClass.ProjectIdentityMismatch;
        }

        return PaHandoverCorrectionFailureClassifier.ClassifyImportFailure(result.Import.Validation);
    }

    public static bool ShouldOfferCorrectionRequest(PaHandoverCorrectionFailureClass failureClass) =>
        PaHandoverCorrectionFailureClassifier.OffersCorrectionRequest(failureClass);

    /// <summary>
    /// Guided Validate PA Response may record durable consumption only after strict validation succeeds.
    /// </summary>
    public static bool IsAuthorizedForGuidedDurableConsumption(PaHandoverImportOperationResult result) =>
        result.ProjectIdMatched
        && result.Import.Package is not null
        && result.Import.Validation.State == RelayValidationState.Valid;
}
