namespace Edf.Application.Relay;

using Edf.Domain.Relay;

public sealed record RelayValidationResult(
    RelayValidationState State,
    IReadOnlyList<RelayValidationDiagnostic> Diagnostics)
{
    public bool IsEligibleForValidatedCursorHandover =>
        RelayValidatedHandoverEligibility.IsEligibleForValidatedCursorHandover(State);

    public static RelayValidationResult Valid(IReadOnlyList<RelayValidationDiagnostic>? diagnostics = null) =>
        new(RelayValidationState.Valid, diagnostics ?? Array.Empty<RelayValidationDiagnostic>());

    public static RelayValidationResult Incomplete(IReadOnlyList<RelayValidationDiagnostic> diagnostics) =>
        new(RelayValidationState.Incomplete, diagnostics);

    public static RelayValidationResult RejectedMalformed(IReadOnlyList<RelayValidationDiagnostic> diagnostics) =>
        new(RelayValidationState.RejectedMalformed, diagnostics);
}
