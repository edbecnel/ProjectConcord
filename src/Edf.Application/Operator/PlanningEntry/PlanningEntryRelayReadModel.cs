namespace Edf.Application.Operator.PlanningEntry;

using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public sealed class PlanningEntryRelayReadModel : IPlanningEntryRelayReadModel
{
    private readonly IRelayOperationalStore _store;

    public PlanningEntryRelayReadModel(IRelayOperationalStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public PlanningEntryRelayReadModelSnapshot Resolve(ProjectConcordProjectId projectId)
    {
        var events = _store.ListProvenanceEvents(projectId);
        PlanningEntryRelayHandoverSnapshot? latestConsumed = null;
        PlanningEntryRelayHandoverSnapshot? latestReview = null;

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
                && IsPlanningEntryReviewExport(persisted.Package))
            {
                latestReview = ToSnapshot(persisted);
            }

            if (latestConsumed is not null && latestReview is not null)
            {
                break;
            }
        }

        return new PlanningEntryRelayReadModelSnapshot(latestConsumed, latestReview);
    }

    private static bool IsPlanningEntryReviewExport(GovernedRelayPackage package)
    {
        var gc = package.GovernanceCritical;
        return gc.EngineeringAgentMode == EngineeringAgentMode.Plan
               && !gc.AuthorizationDispositionPresent
               && !gc.WorkContextPresent
               && !gc.DirectiveFlags.DirectsImplementationWork
               && !gc.DirectiveFlags.DirectsTrancheWork;
    }

    private static PlanningEntryRelayHandoverSnapshot ToSnapshot(PersistedGovernedRelayPackage persisted) =>
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
