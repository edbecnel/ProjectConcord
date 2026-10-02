using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayAutomatedExecutionPromptTests
{
    private readonly EngineeringAgentManualRelayBridge _bridge = new();
    private readonly ProjectArchitectManualAdapter _paAdapter = new();

    [Fact]
    public void TryRenderValidatedHandover_OutputUnchanged_ForP0ManualPath()
    {
        var (package, validation) = ImportValidPaHandover();
        var handoverOnly = _bridge.TryRenderValidatedHandover(package, validation);

        Assert.True(handoverOnly.IsReadyForManualTransfer);
        Assert.DoesNotContain(
            GovernedRelayEngineeringResultResponseInstruction.SectionHeading,
            handoverOnly.RenderedHandover!,
            StringComparison.Ordinal);
    }

    [Fact]
    public void Compose_ContainsCanonicalHandover_AndCompleteOutputContract()
    {
        var (package, validation) = ImportValidPaHandover();
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var composed = _bridge.ComposeAutomatedExecutionPrompt(
            prepared.RenderedHandover!,
            prepared.ExportPackage!);

        Assert.StartsWith(prepared.RenderedHandover!.TrimEnd(), composed.TrimEnd(), StringComparison.Ordinal);
        Assert.Contains(GovernedRelayEngineeringResultResponseInstruction.SectionHeading, composed, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, composed, StringComparison.Ordinal);
        Assert.Contains("engineeringResultImport", composed, StringComparison.Ordinal);
        Assert.Contains(package.ProjectId.Value.ToString(), composed, StringComparison.Ordinal);
        Assert.Contains(package.CorrelationId.Value.ToString(), composed, StringComparison.Ordinal);
    }

    [Fact]
    public void ResponseInstruction_IsProviderNeutral_NoCursorReferences()
    {
        var (package, validation) = ImportValidPaHandover();
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var instruction = _bridge.RenderEngineeringResultResponseInstruction(prepared.ExportPackage!);

        Assert.DoesNotContain("Cursor", instruction, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ACP", instruction, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("untrusted", instruction, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidEngineeringResultImport_StillParses_AfterManualPathUnchanged()
    {
        var (package, validation) = ImportValidPaHandover();
        var projectId = package.ProjectId;
        var resultPackage = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            authorizationDispositionPresent: false,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty) with
        {
            ProjectId = projectId,
            CorrelationId = package.CorrelationId,
            Kind = GovernedPackageKind.EngineeringResultImport,
            GovernanceCritical = new RelayGovernanceCriticalState(
                EngineeringAgentMode.Plan,
                null,
                null,
                RelayTestFixtures.SessionWithBothIntents(),
                RelayStopMetadata.None,
                false,
                false,
                new RelayGovernanceDirectiveFlags(false, false),
                null),
        };
        var rendered = new GovernedRelayV1Renderer().Render(resultPackage);
        var import = _bridge.TryParseEngineeringResult(rendered);

        Assert.Equal(RelayValidationState.Valid, import.Validation.State);
        Assert.Equal(GovernedPackageKind.EngineeringResultImport, import.Package!.Kind);
    }

    [Fact]
    public void OrdinaryProse_RemainsImportRejected_ThroughBridge()
    {
        var import = _bridge.TryParseEngineeringResult("This is only explanatory prose from an agent.");

        Assert.Null(import.Package);
        Assert.Equal(RelayValidationState.RejectedMalformed, import.Validation.State);
    }

    private (GovernedRelayPackage Package, RelayValidationResult Validation) ImportValidPaHandover()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover();
        var rendered = _paAdapter.RenderPaReviewPackage(package);
        var imported = _paAdapter.TryParsePaHandoverImport(rendered);
        Assert.True(imported.Validation.IsEligibleForValidatedEngineeringAgentHandover);
        return (imported.Package!, imported.Validation);
    }
}
