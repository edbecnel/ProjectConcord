namespace Edf.Application.Relay;

using Edf.Domain.Projects;
using Edf.Domain.Relay;

public sealed class GovernedInteractionRelayService : IGovernedInteractionRelayService
{
    private const string MinimalPayloadJson = "{}";
    private readonly IRelayOperationalStore _store;
    private readonly ITier0RelaySnapshotProvider _tier0;
    private readonly TimeProvider _clock;

    public GovernedInteractionRelayService(
        IRelayOperationalStore store,
        ITier0RelaySnapshotProvider tier0,
        TimeProvider? clock = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _tier0 = tier0 ?? throw new ArgumentNullException(nameof(tier0));
        _clock = clock ?? TimeProvider.System;
    }

    public RelaySessionContinuity GetSessionContinuity(ProjectConcordProjectId projectId) =>
        _store.GetSessionContinuity(projectId);

    public void SetProjectArchitectSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent? userIntent)
    {
        _store.SetProjectArchitectSessionIntent(projectId, userIntent, _clock.GetUtcNow());
        if (userIntent is not null)
        {
            AppendEvent(
                projectId,
                null,
                null,
                RelayProvenanceEventType.UserDecision,
                """{"agentRole":"projectArchitect","decision":"sessionIntent"}""");
        }
    }

    public void SetEngineeringAgentSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent? userIntent)
    {
        _store.SetEngineeringAgentSessionIntent(projectId, userIntent, _clock.GetUtcNow());
        if (userIntent is not null)
        {
            AppendEvent(
                projectId,
                null,
                null,
                RelayProvenanceEventType.UserDecision,
                """{"agentRole":"engineeringAgent","decision":"sessionIntent"}""");
        }
    }

    public void SetProjectArchitectSessionAdvisory(ProjectConcordProjectId projectId, AgentSessionAdvisory advisory)
    {
        _store.SetProjectArchitectSessionAdvisory(projectId, advisory, _clock.GetUtcNow());
        if (advisory.Kind != AgentSessionAdvisoryKind.None)
        {
            AppendEvent(
                projectId,
                null,
                null,
                RelayProvenanceEventType.Advisory,
                """{"agentRole":"projectArchitect"}""");
        }
    }

    public void SetEngineeringAgentSessionAdvisory(ProjectConcordProjectId projectId, AgentSessionAdvisory advisory)
    {
        _store.SetEngineeringAgentSessionAdvisory(projectId, advisory, _clock.GetUtcNow());
        if (advisory.Kind != AgentSessionAdvisoryKind.None)
        {
            AppendEvent(
                projectId,
                null,
                null,
                RelayProvenanceEventType.Advisory,
                """{"agentRole":"engineeringAgent"}""");
        }
    }

    public PersistedGovernedRelayPackage RecordProducedPackage(
        GovernedRelayPackage package,
        RelayValidationResult validation,
        ProjectRoot? projectRootForTier0 = null,
        string? renderedBodyHash = null,
        PaHandoverResponseProfile? paReviewResponseProfile = null)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(validation);

        var tier0 = package.Tier0Snapshot
            ?? (projectRootForTier0 is null ? null : _tier0.CaptureSnapshot(projectRootForTier0));
        var recordedUtc = _clock.GetUtcNow();
        var enriched = tier0 is null || package.Tier0Snapshot is not null
            ? package
            : package with { Tier0Snapshot = tier0, UpdatedUtc = recordedUtc };

        var persisted = ToPersisted(enriched, validation, renderedBodyHash);
        _store.SavePackage(persisted);

        AppendEvent(
            enriched.ProjectId,
            enriched.PackageId,
            enriched.CorrelationId,
            RelayProvenanceEventType.ObservedContext,
            MinimalPayloadJson);
        var producedPayload = package.Kind == GovernedPackageKind.PaReviewExport && paReviewResponseProfile is not null
            ? RelayPaReviewExportProvenance.CreatePayload(paReviewResponseProfile.Value)
            : MinimalPayloadJson;
        AppendEvent(
            enriched.ProjectId,
            enriched.PackageId,
            enriched.CorrelationId,
            RelayProvenanceEventType.PackageProduced,
            producedPayload);

        return persisted;
    }

    public PersistedGovernedRelayPackage RecordConsumedPackage(
        GovernedRelayPackage package,
        RelayValidationResult validation)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(validation);

        var persisted = ToPersisted(package, validation);
        _store.SavePackage(persisted);

        AppendEvent(
            package.ProjectId,
            package.PackageId,
            package.CorrelationId,
            RelayProvenanceEventType.PackageConsumed,
            MinimalPayloadJson);

        return persisted;
    }

    public IReadOnlyList<RelayProvenanceEvent> ListProvenanceEvents(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId? correlationId = null) =>
        _store.ListProvenanceEvents(projectId, correlationId);

    private void AppendEvent(
        ProjectConcordProjectId projectId,
        GovernedPackageId? packageId,
        GovernedCorrelationId? correlationId,
        RelayProvenanceEventType eventType,
        string payloadJson)
    {
        _store.AppendProvenanceEvent(
            new RelayProvenanceEvent(
                RelayProvenanceEventId.New(),
                projectId,
                packageId,
                correlationId,
                eventType,
                payloadJson,
                _clock.GetUtcNow()));
    }

    private static PersistedGovernedRelayPackage ToPersisted(
        GovernedRelayPackage package,
        RelayValidationResult validation,
        string? renderedBodyHash = null)
    {
        var (state, diagnostics) = RelayValidationResultMapper.FromResult(validation);
        return new PersistedGovernedRelayPackage(package, state, diagnostics, renderedBodyHash);
    }
}
