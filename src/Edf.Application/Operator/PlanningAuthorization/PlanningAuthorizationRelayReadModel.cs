namespace Edf.Application.Operator.PlanningAuthorization;

using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public sealed class PlanningAuthorizationRelayReadModel : IPlanningAuthorizationRelayReadModel
{
    private readonly IRelayOperationalStore _store;

    public PlanningAuthorizationRelayReadModel(IRelayOperationalStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public PlanningAuthorizationRelayReadModelSnapshot Resolve(ProjectConcordProjectId projectId)
    {
        var events = _store.ListProvenanceEvents(projectId);
        PlanningAuthorizationRelayHandoverSnapshot? latestConsumed = null;
        PlanningAuthorizationRelayHandoverSnapshot? latestReview = null;

        for (var i = events.Count - 1; i >= 0; i--)
        {
            var evt = events[i];
            if (evt.PackageId is not { } packageId)
            {
                continue;
            }

            var persisted = _store.GetPackage(packageId);
            if (persisted is null)
            {
                continue;
            }

            if (evt.EventType == RelayProvenanceEventType.PackageConsumed
                && persisted.Package.Kind == GovernedPackageKind.PaHandoverImport
                && latestConsumed is null)
            {
                latestConsumed = ToSnapshot(persisted);
            }

            if (evt.EventType == RelayProvenanceEventType.PackageProduced
                && persisted.Package.Kind == GovernedPackageKind.PaReviewExport
                && latestReview is null
                && IsPlanningAuthorizationReviewExport(evt, persisted.Package))
            {
                latestReview = ToSnapshot(persisted);
            }

            if (latestConsumed is not null && latestReview is not null)
            {
                break;
            }
        }

        return new PlanningAuthorizationRelayReadModelSnapshot(latestConsumed, latestReview);
    }

    private static bool IsPlanningAuthorizationReviewExport(
        RelayProvenanceEvent packageProducedEvent,
        GovernedRelayPackage package)
    {
        if (RelayPaReviewExportProvenance.TryReadProfile(packageProducedEvent)
            != PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization)
        {
            return false;
        }

        var gc = package.GovernanceCritical;
        return gc.EngineeringAgentMode == EngineeringAgentMode.Plan
               && !gc.AuthorizationDispositionPresent
               && !gc.WorkContextPresent
               && !gc.DirectiveFlags.DirectsImplementationWork
               && !gc.DirectiveFlags.DirectsTrancheWork;
    }

    private static PlanningAuthorizationRelayHandoverSnapshot ToSnapshot(PersistedGovernedRelayPackage persisted) =>
        new(persisted.Package, ToValidationResult(persisted));

    private static RelayValidationResult ToValidationResult(PersistedGovernedRelayPackage persisted) =>
        persisted.ValidationState switch
        {
            RelayValidationState.Valid => RelayValidationResult.Valid(persisted.ValidationDiagnostics),
            RelayValidationState.Incomplete => RelayValidationResult.Incomplete(persisted.ValidationDiagnostics.ToList()),
            RelayValidationState.RejectedMalformed => RelayValidationResult.RejectedMalformed(
                persisted.ValidationDiagnostics.ToList()),
            _ => RelayValidationResult.Incomplete(persisted.ValidationDiagnostics.ToList()),
        };
}
