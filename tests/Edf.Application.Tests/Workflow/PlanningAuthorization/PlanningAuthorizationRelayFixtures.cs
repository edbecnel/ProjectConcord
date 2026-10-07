using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Workflow.PlanningAuthorization;

public static class PlanningAuthorizationRelayFixtures
{
    public static GovernedRelayPackage CreateQualifyingPlanningAuthorizationHandover(
        ProjectConcordProjectId projectId,
        Func<SoftwareDevelopmentProfilePayload, SoftwareDevelopmentProfilePayload>? configurePayload = null)
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            AuthorizationDisposition = new AuthorizationDispositionProjection(
                "PLANNING DEVELOPMENT WORK AUTHORIZED",
                PlanningAuthorized: true,
                ImplementationAuthorized: false,
                AuthorizedTrancheId: null),
            DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                SoftwareDevelopmentAuthorizationKind.Planning,
                null,
                ["planning-governed-work"],
                "planning-dwa-ref",
                false),
        };
        if (configurePayload is not null)
        {
            payload = configurePayload(payload);
        }

        var bytes = SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload);
        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode.Plan,
            null,
            null,
            new RelaySessionContinuity(
                AgentSessionIntent.Continue,
                AgentSessionAdvisory.None,
                AgentSessionIntent.Continue,
                AgentSessionAdvisory.None),
            RelayStopMetadata.None,
            AuthorizationDispositionPresent: true,
            WorkContextPresent: false,
            new RelayGovernanceDirectiveFlags(false, false),
            null);

        return new GovernedRelayPackage(
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            GovernedPackageKind.PaHandoverImport,
            RelaySchemaVersion.Current,
            RelayRenderVersion.V1,
            projectId,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            governance,
            Tier0RelaySnapshot.Empty,
            bytes,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }
}
