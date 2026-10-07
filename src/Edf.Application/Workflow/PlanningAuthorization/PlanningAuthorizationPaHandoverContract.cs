namespace Edf.Application.Workflow.PlanningAuthorization;

using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

/// <summary>
/// Bounded contract for recording Planning DevelopmentWorkAuthorization from a PA handover (distinct from planning-entry topology).
/// </summary>
public static class PlanningAuthorizationPaHandoverContract
{
    public static bool SatisfiesPlanningAuthorizationGrantContract(
        GovernedRelayPackage package,
        RelayValidationResult validation,
        out string reasonCode)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(validation);

        if (validation.State != RelayValidationState.Valid)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.PackageNotValid;
            return false;
        }

        if (package.Kind != GovernedPackageKind.PaHandoverImport)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied;
            return false;
        }

        var governance = package.GovernanceCritical;
        if (!governance.AuthorizationDispositionPresent)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied;
            return false;
        }

        if (governance.Stop.State == RelayStopState.Active)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.RelayStopActive;
            return false;
        }

        if (governance.DirectiveFlags.DirectsImplementationWork
            || governance.DirectiveFlags.DirectsTrancheWork)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied;
            return false;
        }

        if (!SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                package.ProfilePayload,
                out var payload,
                out _)
            || payload is null)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied;
            return false;
        }

        var disposition = payload.AuthorizationDisposition;
        if (disposition is null
            || disposition.ImplementationAuthorized
            || !disposition.PlanningAuthorized)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied;
            return false;
        }

        var dwa = payload.DevelopmentWorkAuthorization;
        if (dwa is null || dwa.Kind != SoftwareDevelopmentAuthorizationKind.Planning)
        {
            reasonCode = PlanningAuthorizationGrantReasonCodes.ContractNotSatisfied;
            return false;
        }

        reasonCode = string.Empty;
        return true;
    }
}
