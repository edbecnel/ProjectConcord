namespace Edf.Application.Relay;

using Edf.Domain.Relay;

internal static class RelayValidationResultMapper
{
    public static RelayValidationResult ToResult(
        RelayValidationState state,
        IReadOnlyList<RelayValidationDiagnostic> diagnostics) =>
        state switch
        {
            RelayValidationState.Valid => RelayValidationResult.Valid(diagnostics),
            RelayValidationState.Incomplete => RelayValidationResult.Incomplete(diagnostics),
            RelayValidationState.RejectedMalformed => RelayValidationResult.RejectedMalformed(diagnostics),
            _ => RelayValidationResult.RejectedMalformed(diagnostics),
        };

    public static (RelayValidationState State, IReadOnlyList<RelayValidationDiagnostic> Diagnostics) FromResult(
        RelayValidationResult result) =>
        (result.State, result.Diagnostics);
}
