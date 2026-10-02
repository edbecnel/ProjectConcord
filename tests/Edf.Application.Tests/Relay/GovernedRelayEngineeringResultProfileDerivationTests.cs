using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayEngineeringResultProfileDerivationTests
{
    [Fact]
    public void ImplementationHandover_DerivesThinReporting()
    {
        var handover = RelaySerializationFixtures.ValidImplementationHandover();
        var reporting = GovernedRelayEngineeringResultProfileDerivation.DeriveReportingRequirements(handover);

        Assert.False(reporting.IsRich);
        Assert.Equal(GovernedRelayEngineeringResultReportingRequirements.Thin, reporting);
    }

    [Fact]
    public void TrancheHandoverWithoutImplementationDirective_StillDerivesThin_ForAutomatedTransport()
    {
        var handover = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(false, true),
            authorizationDispositionPresent: true,
            workContextPresent: true,
            payload: RelaySerializationFixtures.BuildImplementationPayload("A2-T5") with
            {
                DevelopmentWorkAuthorization = null,
            }) with
        {
            Kind = GovernedPackageKind.EngineeringAgentHandoverExport,
        };

        var reporting = GovernedRelayEngineeringResultProfileDerivation.DeriveReportingRequirements(handover);

        Assert.Equal(GovernedRelayEngineeringResultReportingRequirements.Thin, reporting);
    }

    [Fact]
    public void CompletedThinExample_StillValidatesThroughImporter()
    {
        var bridge = new EngineeringAgentManualRelayBridge();
        var handover = RelaySerializationFixtures.ValidImplementationHandover();
        var completed = GovernedRelayEngineeringResultOutputContract.RenderCompletedValidExample(handover);
        var import = bridge.TryParseEngineeringResult(completed);

        Assert.Equal(RelayValidationState.Valid, import.Validation.State);
    }
}
