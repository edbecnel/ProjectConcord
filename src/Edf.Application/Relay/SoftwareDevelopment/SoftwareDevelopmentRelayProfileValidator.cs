namespace Edf.Application.Relay.SoftwareDevelopment;

using Edf.Domain.Relay;

/// <summary>
/// B-layer Software Development relay profile validation (PC-AIGOV-003, PC-AIGOV-004, PC-AIGOV-007, PC-AIGOV-014).
/// </summary>
public sealed class SoftwareDevelopmentRelayProfileValidator : ISoftwareDevelopmentRelayProfileValidator
{
    public static SoftwareDevelopmentRelayProfileValidator Instance { get; } = new();

    public void Validate(GovernedRelayPackage package, RelayValidationAccumulator accumulator)
    {
        if (!RequiresProfileValidation(package.Kind))
        {
            return;
        }

        if (!SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                package.ProfilePayload,
                out var payload,
                out var parseError)
            || payload is null)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.ProfilePayloadMalformed,
                $"Software Development profile payload is not valid JSON: {parseError}");
            return;
        }

        if (payload.PayloadVersion != SoftwareDevelopmentProfilePayloadSerializer.CurrentPayloadVersion)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ProfilePayloadVersionUnsupported,
                "Software Development profile payload version is not supported for this relay boundary.");
        }

        RejectDeferredAuthorityModels(payload, accumulator);
        ValidateHandoverVersusAuthorization(package, payload, accumulator);
        ValidateAuthorizationDispositionAlignment(package, payload, accumulator);
        ValidateImplementationAuthorization(package, payload, accumulator);
        ValidateTrancheScope(package, payload, accumulator);
        ValidateStopInteraction(package, payload, accumulator);
        ValidateArchitecturalAcceptance(package, payload, accumulator);
        _ = package.Tier0Snapshot;
    }

    private static bool RequiresProfileValidation(GovernedPackageKind kind) =>
        kind is GovernedPackageKind.PaHandoverImport
            or GovernedPackageKind.CursorHandoverExport
            or GovernedPackageKind.PaReviewExport
            or GovernedPackageKind.EngineeringResultImport;

    private static void RejectDeferredAuthorityModels(
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        if (payload.AuthorityGrant is not null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.AuthorityGrantNotSupported,
                "Generic AuthorityGrant is deferred; relay packages must use Software Development authorization projections only.");
        }

        if (payload.ProjectWorkRecord?.TreatsRecordAsAuthorization == true)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ProjectWorkRecordAuthorityConflation,
                "Project Work Record identity must not substitute for DevelopmentWorkAuthorization.");
        }

        if (payload.HumanInitiatedWorkItem?.TreatsIntakeAsAuthorization == true)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.HumanInitiatedWorkItemAuthorityConflation,
                "HumanInitiatedWorkItem intake must not substitute for DevelopmentWorkAuthorization or Project Work Record authority.");
        }
    }

    private static void ValidateHandoverVersusAuthorization(
        GovernedRelayPackage package,
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        var handover = payload.HandoverContext;
        if (handover is null)
        {
            return;
        }

        if (!handover.IsInheritedContextOnly)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.HandoverDevelopmentWorkAuthorizationConflation,
                "Handover context must be marked inherited-context-only and must not merge with DevelopmentWorkAuthorization semantics.");
        }

        var directsImplementation = package.GovernanceCritical.DirectiveFlags.DirectsImplementationWork;
        var dwa = payload.DevelopmentWorkAuthorization;
        if (directsImplementation && dwa is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.HandoverDoesNotImplyAuthorization,
                "Handover context does not create implementation authorization; an explicit DevelopmentWorkAuthorization projection is required.");
        }
    }

    private static void ValidateAuthorizationDispositionAlignment(
        GovernedRelayPackage package,
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        var governance = package.GovernanceCritical;
        var disposition = payload.AuthorizationDisposition;

        if (governance.AuthorizationDispositionPresent && disposition is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.AuthorizationDispositionPayloadMissing,
                "Authorization disposition is declared in governance-critical state but missing from the Software Development profile payload.");
        }

        if (disposition is not null && !governance.AuthorizationDispositionPresent)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.AuthorizationDispositionPayloadUnexpected,
                "Software Development authorization disposition is present without matching governance-critical disposition metadata.");
        }

        if (disposition?.ImplementationAuthorized == true && payload.DevelopmentWorkAuthorization is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.AuthorizationDispositionNotDevelopmentWorkAuthorization,
                "Authorization disposition does not substitute for an explicit DevelopmentWorkAuthorization projection.");
        }
    }

    private static void ValidateImplementationAuthorization(
        GovernedRelayPackage package,
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        var directsImplementation = package.GovernanceCritical.DirectiveFlags.DirectsImplementationWork;
        var dwa = payload.DevelopmentWorkAuthorization;

        if (!directsImplementation)
        {
            return;
        }

        if (dwa is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ImplementationAuthorizationMissing,
                "Implementation-directed work requires an explicit DevelopmentWorkAuthorization projection in the profile payload.");
            return;
        }

        if (dwa.Kind == SoftwareDevelopmentAuthorizationKind.Planning)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.PlanningCannotSatisfyImplementation,
                "Planning authorization cannot satisfy implementation-directed relay work (PC-AIGOV-004).");
        }
    }

    private static void ValidateTrancheScope(
        GovernedRelayPackage package,
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        var governance = package.GovernanceCritical;
        var workContext = payload.WorkContext;
        var dwa = payload.DevelopmentWorkAuthorization;

        if (governance.DirectiveFlags.DirectsTrancheWork)
        {
            if (governance.WorkContextPresent && workContext is null)
            {
                accumulator.AddIncomplete(
                    RelayValidationCodes.WorkContextPayloadMissing,
                    "Tranche/work context is declared in governance-critical state but missing from the Software Development profile payload.");
            }

            if (workContext is not null && !governance.WorkContextPresent)
            {
                accumulator.AddIncomplete(
                    RelayValidationCodes.WorkContextPayloadUnexpected,
                    "Software Development work context is present without matching governance-critical work context metadata.");
            }
        }

        if (dwa is null || workContext?.RequestedTrancheId is null)
        {
            return;
        }

        var authorizedTranche = NormalizeTrancheId(dwa.AuthorizedTrancheId);
        var requestedTranche = NormalizeTrancheId(workContext.RequestedTrancheId);
        if (authorizedTranche is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.AuthorizedTrancheMissing,
                "DevelopmentWorkAuthorization projection must name the authorized tranche when tranche work is requested.");
            return;
        }

        if (requestedTranche is null)
        {
            return;
        }

        if (!string.Equals(authorizedTranche, requestedTranche, StringComparison.OrdinalIgnoreCase))
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.TrancheScopeExceeded,
                "Requested tranche work is outside the explicitly authorized tranche (PC-AIGOV-014).");
        }
    }

    private static void ValidateStopInteraction(
        GovernedRelayPackage package,
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        var dwa = payload.DevelopmentWorkAuthorization;
        if (dwa?.AuthorizedByStopAcknowledgment == true)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.StopDoesNotAuthorize,
                "STOP acknowledgment does not create or expand DevelopmentWorkAuthorization.");
        }

        if (package.GovernanceCritical.Stop.State == RelayStopState.Active
            && dwa?.Kind == SoftwareDevelopmentAuthorizationKind.Implementation
            && string.IsNullOrWhiteSpace(dwa.AuthorizationReference))
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.StopBlocksImplicitImplementationAuthorization,
                "Active STOP requires explicit disposition; relay must not treat STOP metadata as implementation authorization.");
        }
    }

    private static void ValidateArchitecturalAcceptance(
        GovernedRelayPackage package,
        SoftwareDevelopmentProfilePayload payload,
        RelayValidationAccumulator accumulator)
    {
        var review = payload.ArchitecturalReview;
        if (review is null)
        {
            return;
        }

        if (review.ImpliesImplementationAuthorization
            && payload.DevelopmentWorkAuthorization?.Kind != SoftwareDevelopmentAuthorizationKind.Implementation)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ArchitecturalAcceptanceNotImplementationAuthorization,
                "Project Architect acceptance or architectural review disposition does not imply implementation authorization.");
        }

        if (package.GovernanceCritical.DirectiveFlags.DirectsImplementationWork
            && review.PaDisposition == ArchitecturalReviewPaDisposition.Accepted
            && payload.DevelopmentWorkAuthorization is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ArchitecturalAcceptanceNotImplementationAuthorization,
                "Project Architect acceptance recorded in the profile does not substitute for DevelopmentWorkAuthorization.");
        }
    }

    private static string? NormalizeTrancheId(string? trancheId) =>
        string.IsNullOrWhiteSpace(trancheId) ? null : trancheId.Trim();
}
