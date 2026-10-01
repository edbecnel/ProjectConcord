using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

public class EngineeringAgentA4T0ContractTests
{
    [Fact]
    public void ProviderPluginId_IsComparableAndDistinctFromGuidPackageIds()
    {
        var pluginId = EngineeringAgentProviderPluginId.Parse("test.provider.alpha");
        var duplicate = EngineeringAgentProviderPluginId.Parse("test.provider.alpha");

        Assert.Equal(pluginId, duplicate);
        Assert.NotEqual(
            EngineeringAgentProviderPluginId.Parse("test.provider.beta"),
            pluginId);
    }

    [Fact]
    public void TransportOperationId_IsDistinctTypeFromGovernedPackageId()
    {
        var packageId = GovernedPackageId.New();
        var operationId = TransportOperationId.New();

        Assert.NotEqual(packageId.Value, operationId.Value);
        Assert.False(TransportOperationIdentityRules.SharesSameGuidValue(operationId, packageId));
    }

    [Fact]
    public void RequireDistinctFromPackage_RejectsSameGuidValue()
    {
        var shared = Guid.NewGuid();
        var packageId = new GovernedPackageId(shared);
        var operationId = new TransportOperationId(shared);

        Assert.Throws<InvalidOperationException>(() =>
            TransportOperationIdentityRules.RequireDistinctFromPackage(operationId, packageId));
    }

    [Fact]
    public void Capabilities_ExpressAutomatedTransportAndRenderCompatibility()
    {
        var capabilities = new EngineeringAgentProviderCapabilities(
            SupportsAutomatedTransport: true,
            SupportedRenderProtocolMajor: RelayRenderVersion.V1.Major,
            RoutingIntentSupport: new EngineeringAgentRoutingIntentSupport(
                SupportsPlan: true,
                SupportsAgent: true,
                SupportsDebugSemantically: false),
            IsAvailableForSelection: true);

        Assert.True(capabilities.IsCompatibleWithRenderProtocol(RelayRenderVersion.V1.Major));
        Assert.False(capabilities.IsCompatibleWithRenderProtocol(RelayRenderVersion.V1.Major + 1));
    }

    [Theory]
    [InlineData(EngineeringAgentMode.Plan, true)]
    [InlineData(EngineeringAgentMode.Agent, true)]
    [InlineData(EngineeringAgentMode.Debug, false)]
    public void RoutingIntentSupport_UsesNeutralEngineeringAgentMode(
        EngineeringAgentMode mode,
        bool expectedSupported)
    {
        var support = new EngineeringAgentRoutingIntentSupport(
            SupportsPlan: true,
            SupportsAgent: true,
            SupportsDebugSemantically: false);

        Assert.Equal(expectedSupported, support.SupportsRoutingIntent(mode));
    }

    [Fact]
    public void ResultCandidate_DoesNotImplyGovernanceAcceptance()
    {
        var candidate = new EngineeringAgentTransportResultCandidate(
            TransportOperationId.New(),
            UntrustedImportText: "untrusted provider text",
            IsReadyForParse: true);

        Assert.True(candidate.IsReadyForParse);
        Assert.Contains("untrusted", candidate.UntrustedImportText, StringComparison.Ordinal);
    }

    [Fact]
    public void ProviderSessionHandle_IsOpaqueAndNonCanonical()
    {
        var handle = EngineeringAgentProviderSessionHandle.FromOpaque("provider-internal-id");
        Assert.Equal("(opaque provider session handle)", handle.ToString());
    }

    [Fact]
    public void ProviderFailure_IsNeutralAndTyped()
    {
        var failure = new EngineeringAgentProviderFailure(
            EngineeringAgentProviderFailureKind.AmbiguousOutcome,
            "Unknown provider state after restart.");

        Assert.Equal(EngineeringAgentProviderFailureKind.AmbiguousOutcome, failure.Kind);
    }

    [Fact]
    public void DenyByDefaultPermissionPolicy_DeniesExpansion()
    {
        var policy = new DenyByDefaultEngineeringAgentProviderPermissionPolicy();
        var decision = policy.Evaluate(
            new EngineeringAgentProviderPermissionRequest(
                EngineeringAgentProviderPluginId.Parse("test.provider"),
                "Expand filesystem scope"));

        Assert.Equal(EngineeringAgentProviderPermissionDisposition.Deny, decision.Disposition);
    }

    [Fact]
    public async Task FakeProvider_InitializeAndShutdownLifecycle_AreNeutral()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.test"),
            supportsDebug: false,
            renderProtocolMajor: 1);

        Assert.False(plugin.GetHealth().IsInitialized);

        var init = await plugin.InitializeAsync();
        Assert.True(init.Succeeded);

        var shutdown = await plugin.ShutdownAsync();
        Assert.True(shutdown.IsAcknowledged);
        Assert.Null(shutdown.Failure);
    }

    [Fact]
    public void FakeProvider_ImplementsContractWithoutProviderSpecificDependencies()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.test"),
            supportsDebug: false,
            renderProtocolMajor: 1);

        Assert.Equal("fake.test", plugin.PluginId.Value);
        Assert.True(plugin.DeclareCapabilities().SupportsAutomatedTransport);
    }

    [Fact]
    public void LifecycleEnum_IncludesAdrRequiredSemanticDistinctions()
    {
        Assert.True(Enum.IsDefined(typeof(TransportOperationLifecycleState), TransportOperationLifecycleState.CreatedNotForwarded));
        Assert.True(Enum.IsDefined(typeof(TransportOperationLifecycleState), TransportOperationLifecycleState.ResultCandidateReceived));
        Assert.True(Enum.IsDefined(typeof(TransportOperationLifecycleState), TransportOperationLifecycleState.Ambiguous));
    }
}
