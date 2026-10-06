using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Workflow.PlanningEntry;

public static class Mvr0005PlanningEntryRelayFixtures
{
    public static GovernedRelayPackage CreatePlanningEntryHandover(
        ProjectConcordProjectId projectId,
        Func<RelayGovernanceCriticalState, RelayGovernanceCriticalState>? configureGovernance = null,
        Func<SoftwareDevelopmentProfilePayload, SoftwareDevelopmentProfilePayload>? configurePayload = null)
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            AuthorizationDisposition = new AuthorizationDispositionProjection(
                "PLANNING AUTHORIZED",
                PlanningAuthorized: true,
                ImplementationAuthorized: false,
                AuthorizedTrancheId: null),
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

        if (configureGovernance is not null)
        {
            governance = configureGovernance(governance);
        }

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

    public static GovernedRelayPackage CreateImplementationAuthorizedHandover(ProjectConcordProjectId projectId)
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            AuthorizationDisposition = new AuthorizationDispositionProjection(
                "IMPLEMENTATION AUTHORIZED",
                false,
                true,
                "A2-MVR"),
            DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                SoftwareDevelopmentAuthorizationKind.Implementation,
                "A2-MVR",
                ["scope"],
                "dwa-ref",
                false),
            WorkContext = new WorkContextProjection("A2-MVR", "tranche"),
        };

        var bytes = SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload);
        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode.Agent,
            EngineeringAgentMode.Plan,
            new EngineeringAgentModeTransition(EngineeringAgentMode.Plan, EngineeringAgentMode.Agent),
            new RelaySessionContinuity(
                AgentSessionIntent.Continue,
                AgentSessionAdvisory.None,
                AgentSessionIntent.New,
                AgentSessionAdvisory.None),
            RelayStopMetadata.None,
            true,
            true,
            new RelayGovernanceDirectiveFlags(true, true),
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

    public static RelayValidationResult Validate(GovernedRelayPackage package)
    {
        var validator = new GovernedRelayPackageValidator(SoftwareDevelopmentRelayProfileValidator.Instance);
        return validator.Validate(package);
    }
}
