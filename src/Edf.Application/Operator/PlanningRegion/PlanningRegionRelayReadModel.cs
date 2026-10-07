namespace Edf.Application.Operator.PlanningRegion;

using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public sealed class PlanningRegionRelayReadModel : IPlanningRegionRelayReadModel
{
    private readonly IRelayOperationalStore _store;

    public PlanningRegionRelayReadModel(IRelayOperationalStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public PlanningRegionRelayReadModelSnapshot Resolve(ProjectConcordProjectId projectId)
    {
        var events = _store.ListProvenanceEvents(projectId);
        PlanningRegionRelayHandoverSnapshot? latestConsumed = null;
        PlanningRegionRelayHandoverSnapshot? latestReview = null;

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
                && IsPlanningRegionReviewExport(evt, persisted.Package))
            {
                latestReview = ToSnapshot(persisted);
            }

            if (latestConsumed is not null && latestReview is not null)
            {
                break;
            }
        }

        return new PlanningRegionRelayReadModelSnapshot(latestConsumed, latestReview);
    }

    private static bool IsPlanningRegionReviewExport(
        RelayProvenanceEvent packageProducedEvent,
        GovernedRelayPackage package)
    {
        var profile = RelayPaReviewExportProvenance.TryReadProfile(packageProducedEvent);
        if (profile == PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization)
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

    private static PlanningRegionRelayHandoverSnapshot ToSnapshot(PersistedGovernedRelayPackage persisted) =>
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
