namespace Edf.Application.Workflow.PlanningEntry;

using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Bounded planning-entry contract for Intake → planning-governed transition (PC-PAR-025 applicability).
/// </summary>
public static class PlanningEntryPaHandoverContract
{
    public static bool SatisfiesPlanningEntryContract(
        GovernedRelayPackage package,
        RelayValidationResult validation,
        out string reasonCode)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(validation);

        if (validation.State != RelayValidationState.Valid)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.PackageNotValid;
            return false;
        }

        if (package.Kind != GovernedPackageKind.PaHandoverImport)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.ContractNotSatisfied;
            return false;
        }

        var governance = package.GovernanceCritical;
        if (!governance.AuthorizationDispositionPresent)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.ContractNotSatisfied;
            return false;
        }

        if (governance.Stop.State == RelayStopState.Active)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.RelayStopActive;
            return false;
        }

        if (governance.DirectiveFlags.DirectsImplementationWork)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.ContractNotSatisfied;
            return false;
        }

        if (!SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                package.ProfilePayload,
                out var payload,
                out _)
            || payload is null)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.ContractNotSatisfied;
            return false;
        }

        if (payload.DevelopmentWorkAuthorization is not null)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.ContractNotSatisfied;
            return false;
        }

        var disposition = payload.AuthorizationDisposition;
        if (disposition is null
            || !disposition.PlanningAuthorized
            || disposition.ImplementationAuthorized)
        {
            reasonCode = PlanningEntryTransitionReasonCodes.ContractNotSatisfied;
            return false;
        }

        reasonCode = string.Empty;
        return true;
    }
}
