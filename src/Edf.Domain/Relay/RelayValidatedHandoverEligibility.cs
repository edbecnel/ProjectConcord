namespace Edf.Domain.Relay;

/// <summary>
/// PC-PAR-014 (A2-active): only <see cref="RelayValidationState.Valid"/> may produce a validated engineering-agent handover later (T6).
/// </summary>
public static class RelayValidatedHandoverEligibility
{
    public static bool IsEligibleForValidatedEngineeringAgentHandover(RelayValidationState validationState) =>
        validationState == RelayValidationState.Valid;
}
