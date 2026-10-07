using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Operator;

public class PlanningAuthorizationRelayReadModelTests
{
    [Fact]
    public void Resolve_IdentifiesLatestPlanningAuthorizationReviewExport_FromProvenanceProfile()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var projectId = ProjectConcordProjectId.New();
        var relay = new GovernedInteractionRelayService(
            persistence.RelayOperational,
            new Edf.Application.Relay.Tier0RelaySnapshotProvider());
        var readModel = new PlanningAuthorizationRelayReadModel(persistence.RelayOperational);

        var planningEntryExport = CreateReviewExportPackage(projectId);
        relay.RecordProducedPackage(
            planningEntryExport,
            RelayValidationResult.Valid([]),
            paReviewResponseProfile: PaHandoverResponseProfile.PlanningEntry);

        var planningAuthExport = CreateReviewExportPackage(projectId);
        relay.RecordProducedPackage(
            planningAuthExport,
            RelayValidationResult.Valid([]),
            paReviewResponseProfile: PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization);

        var snapshot = readModel.Resolve(projectId);

        Assert.NotNull(snapshot.LatestPaReviewExport);
        Assert.Equal(planningAuthExport.PackageId, snapshot.LatestPaReviewExport!.Package.PackageId);
    }

    private static GovernedRelayPackage CreateReviewExportPackage(ProjectConcordProjectId projectId) =>
        new(
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            GovernedPackageKind.PaReviewExport,
            RelaySchemaVersion.Current,
            RelayRenderVersion.V1,
            projectId,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            new RelayGovernanceCriticalState(
                EngineeringAgentMode.Plan,
                null,
                null,
                new RelaySessionContinuity(
                    AgentSessionIntent.New,
                    AgentSessionAdvisory.None,
                    AgentSessionIntent.Continue,
                    AgentSessionAdvisory.None),
                RelayStopMetadata.None,
                AuthorizationDispositionPresent: false,
                WorkContextPresent: false,
                new RelayGovernanceDirectiveFlags(false, false),
                null),
            null,
            SoftwareDevelopmentProfilePayloadSerializer.Serialize(SoftwareDevelopmentProfilePayloadSerializer.Empty),
            GovernedRelayPackage.DefaultStructuralAgreement);
}
