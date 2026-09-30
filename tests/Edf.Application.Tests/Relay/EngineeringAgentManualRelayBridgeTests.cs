using System.Text.RegularExpressions;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class EngineeringAgentManualRelayBridgeTests
{
    private readonly EngineeringAgentManualRelayBridge _bridge = new();
    private readonly ProjectArchitectManualAdapter _paAdapter = new();
    private readonly GovernedRelayV1Renderer _renderer = new();
    [Fact]
    public void DeclareCapabilities_ReportsManualP0WithoutAutomation()
    {
        var capabilities = _bridge.DeclareCapabilities();

        Assert.True(capabilities.SupportsManualPaste);
        Assert.False(capabilities.SupportsAutomatedTransport);
        Assert.Equal(RelayRenderVersion.V1.Major, capabilities.SupportedRenderVersionMajor);
    }

    [Fact]
    public void BridgeAssembly_DoesNotReferenceClipboardOrAutomationSurfaces()
    {
        var assembly = typeof(EngineeringAgentManualRelayBridge).Assembly;
        var typeNames = assembly.GetTypes().Select(t => t.FullName).Where(n => n is not null).ToList();

        Assert.DoesNotContain(typeNames, n => n!.Contains("Clipboard", StringComparison.Ordinal));
        Assert.DoesNotContain(typeNames, n => n!.Contains("AppleScript", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(typeNames, n => n!.Contains("Mcp", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidPaImport_CanBePreparedForManualEngineeringAgentHandover()
    {
        var (package, validation) = ImportValidPaHandover();

        var result = _bridge.TryRenderValidatedHandover(package, validation);

        Assert.True(result.IsReadyForManualTransfer);
        Assert.NotNull(result.RenderedHandover);
        Assert.NotNull(result.ExportPackage);
        Assert.Equal(GovernedPackageKind.EngineeringAgentHandoverExport, result.ExportPackage!.Kind);
        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.True(result.Validation.IsEligibleForValidatedEngineeringAgentHandover);
    }

    [Fact]
    public void PreparedHandover_UsesProjectConcordRelayV1MachineBlock()
    {
        var (package, validation) = ImportValidPaHandover();
        var result = _bridge.TryRenderValidatedHandover(package, validation);

        Assert.Matches(
            new Regex($"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\{{", RegexOptions.Multiline),
            result.RenderedHandover!);
    }

    [Fact]
    public void PreparedHandover_SerializesProviderNeutralEngineeringAgentExportKind()
    {
        var (package, validation) = ImportValidPaHandover();
        var result = _bridge.TryRenderValidatedHandover(package, validation);

        Assert.Contains("engineeringAgentHandoverExport", result.RenderedHandover!, StringComparison.Ordinal);
    }

    [Fact]
    public void IncompleteBoundaryValidation_CannotPrepareEngineeringAgentHandover()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover();
        var incomplete = RelayValidationResult.Incomplete(
        [
            new RelayValidationDiagnostic(
                RelayValidationCodes.ImplementationAuthorizationMissing,
                "missing",
                RelayValidationDiagnosticSeverity.Incomplete),
        ]);

        var result = _bridge.TryRenderValidatedHandover(package, incomplete);

        Assert.False(result.IsReadyForManualTransfer);
        Assert.Null(result.RenderedHandover);
        Assert.False(result.Validation.IsEligibleForValidatedEngineeringAgentHandover);
    }

    [Fact]
    public void RejectedMalformedBoundaryValidation_CannotPrepareEngineeringAgentHandover()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover();
        var rejected = RelayValidationResult.RejectedMalformed(
        [
            new RelayValidationDiagnostic(
                RelayValidationCodes.MachineBlockMissing,
                "missing",
                RelayValidationDiagnosticSeverity.Malformed),
        ]);

        var result = _bridge.TryRenderValidatedHandover(package, rejected);

        Assert.False(result.IsReadyForManualTransfer);
        Assert.Null(result.RenderedHandover);
    }

    [Fact]
    public void HandoverPreparation_UsesRelayValidatedHandoverEligibility_NotAdHocReadiness()
    {
        foreach (var state in Enum.GetValues<RelayValidationState>())
        {
            var expected = RelayValidatedHandoverEligibility.IsEligibleForValidatedEngineeringAgentHandover(state);
            var validation = state switch
            {
                RelayValidationState.Valid => RelayValidationResult.Valid(),
                RelayValidationState.Incomplete => RelayValidationResult.Incomplete([]),
                RelayValidationState.RejectedMalformed => RelayValidationResult.RejectedMalformed([]),
                _ => RelayValidationResult.Incomplete([]),
            };

            var package = RelaySerializationFixtures.ValidImplementationHandover();
            var result = _bridge.TryRenderValidatedHandover(package, validation);

            Assert.Equal(expected, result.IsReadyForManualTransfer);
        }
    }

    [Fact]
    public void ActiveStop_BlocksEngineeringAgentHandoverPreparation()
    {
        var (package, validation) = ImportValidPaHandover();
        var stopped = package with
        {
            GovernanceCritical = package.GovernanceCritical with
            {
                Stop = new RelayStopMetadata(RelayStopState.Active, "STOP"),
            },
        };

        var result = _bridge.TryRenderValidatedHandover(stopped, validation);

        Assert.False(result.IsReadyForManualTransfer);
        Assert.Equal(RelayValidationState.Valid, validation.State);
        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotEqual(RelayValidationState.Incomplete, result.Validation.State);
        Assert.NotEqual(RelayValidationState.RejectedMalformed, result.Validation.State);
        Assert.Contains(
            result.Validation.Diagnostics,
            d => d.Code == RelayValidationCodes.EngineeringAgentHandoverBlockedByActiveStop);
    }

    [Fact]
    public void RoundTrip_PreservesExplicitEngineeringAgentPlanMode()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                PriorEngineeringAgentMode = null,
                ModeTransition = null,
            },
        };
        var validation = ValidatePaImport(package);
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var imported = _paAdapter.TryParsePaHandoverImport(prepared.RenderedHandover!);

        Assert.Equal(EngineeringAgentMode.Plan, imported.Package!.GovernanceCritical.EngineeringAgentMode);
    }

    [Fact]
    public void RoundTrip_PreservesExplicitEngineeringAgentAgentMode()
    {
        var (package, validation) = ImportValidPaHandover();
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var imported = _paAdapter.TryParsePaHandoverImport(prepared.RenderedHandover!);

        Assert.Equal(EngineeringAgentMode.Agent, imported.Package!.GovernanceCritical.EngineeringAgentMode);
    }

    [Fact]
    public void RoundTrip_PreservesExplicitPlanToAgentTransition()
    {
        var (package, validation) = ImportValidPaHandover();
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var imported = _paAdapter.TryParsePaHandoverImport(prepared.RenderedHandover!);

        Assert.Equal(
            EngineeringAgentMode.Plan,
            imported.Package!.GovernanceCritical.ModeTransition!.From);
        Assert.Equal(
            EngineeringAgentMode.Agent,
            imported.Package!.GovernanceCritical.ModeTransition.To);
    }

    [Fact]
    public void PlanModeWithoutTransition_DoesNotManufactureTransitionOnExport()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                PriorEngineeringAgentMode = null,
                ModeTransition = null,
            },
        };
        var validation = ValidatePaImport(package);
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);

        Assert.DoesNotContain(
            $"{GovernedRelayV1Format.EngineeringAgentModeTransitionField}:",
            prepared.RenderedHandover!,
            StringComparison.Ordinal);
    }

    [Fact]
    public void RoundTrip_PreservesEngineeringAgentNewAndContinueIntents()
    {
        var continuity = new RelaySessionContinuity(
            ProjectArchitectSessionIntent: AgentSessionIntent.Continue,
            ProjectArchitectSessionAdvisory: AgentSessionAdvisory.None,
            EngineeringAgentSessionIntent: AgentSessionIntent.New,
            EngineeringAgentSessionAdvisory: AgentSessionAdvisory.None);

        var package = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
            {
                SessionContinuity = continuity,
            },
        };
        var validation = ValidatePaImport(package);
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var imported = _paAdapter.TryParsePaHandoverImport(prepared.RenderedHandover!);

        Assert.Equal(AgentSessionIntent.New, imported.Package!.GovernanceCritical.SessionContinuity.EngineeringAgentSessionIntent);
        Assert.Equal(AgentSessionIntent.Continue, imported.Package!.GovernanceCritical.SessionContinuity.ProjectArchitectSessionIntent);
    }

    [Fact]
    public void AdvisoryDoesNotOverrideExplicitSessionIntent_AfterEngineeringAgentExport()
    {
        var continuity = new RelaySessionContinuity(
            ProjectArchitectSessionIntent: AgentSessionIntent.Continue,
            ProjectArchitectSessionAdvisory: AgentSessionAdvisory.None,
            EngineeringAgentSessionIntent: AgentSessionIntent.New,
            EngineeringAgentSessionAdvisory: new AgentSessionAdvisory(AgentSessionAdvisoryKind.RecommendContinue));

        var package = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
            {
                SessionContinuity = continuity,
            },
        };
        var validation = ValidatePaImport(package);
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        prepared = prepared with
        {
            RenderedHandover = prepared.RenderedHandover + "\n\nEngineering-Agent-Chat-Advisory: CONTINUE\n",
        };
        var imported = _paAdapter.TryParsePaHandoverImport(prepared.RenderedHandover!);

        Assert.Equal(AgentSessionIntent.New, imported.Package!.GovernanceCritical.SessionContinuity.EngineeringAgentSessionIntent);
    }

    [Fact]
    public void SubstantiveSoftwareDevelopmentValidation_RemainsEffectiveAtBridge()
    {
        var package = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty);

        var validation = ValidatePaImport(package);
        Assert.Equal(RelayValidationState.Incomplete, validation.State);

        var result = _bridge.TryRenderValidatedHandover(package, validation);

        Assert.False(result.IsReadyForManualTransfer);
    }

    [Fact]
    public void ProseCannotManufactureEngineeringAgentMode_OnEngineeringResultImport()
    {
        var rendered = RenderEngineeringResultPackage(
            RelaySerializationFixtures.ValidImplementationHandover() with
            {
                Kind = GovernedPackageKind.EngineeringResultImport,
                GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
                {
                    EngineeringAgentMode = EngineeringAgentMode.Plan,
                    PriorEngineeringAgentMode = null,
                    ModeTransition = null,
                },
            });

        rendered += $"\n\n{GovernedRelayV1Format.EngineeringAgentModeField}: AGENT\n";

        var result = _bridge.TryParseEngineeringResult(rendered);

        Assert.Equal(EngineeringAgentMode.Plan, result.Package!.GovernanceCritical.EngineeringAgentMode);
    }

    [Fact]
    public void EngineeringResultImport_RequiresEngineeringResultKind()
    {
        var rendered = _paAdapter.RenderPaReviewPackage(RelaySerializationFixtures.ValidImplementationHandover());
        var result = _bridge.TryParseEngineeringResult(rendered);

        Assert.Equal(RelayValidationState.Incomplete, result.Validation.State);
        Assert.Contains(
            result.Validation.Diagnostics,
            d => d.Code == RelayValidationCodes.EngineeringResultPackageKindMismatch);
    }

    [Fact]
    public void ThinEngineeringResultImport_CanParseWithoutFullImplementationAuthorization()
    {
        var package = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            authorizationDispositionPresent: false,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty) with
        {
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

        var rendered = RenderEngineeringResultPackage(package);
        var result = _bridge.TryParseEngineeringResult(rendered);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.Equal(GovernedPackageKind.EngineeringResultImport, result.Package!.Kind);
    }

    [Fact]
    public void Bridge_UsesSubstantiveSoftwareDevelopmentProfileValidator()
    {
        var rendered = RenderEngineeringResultPackage(
            RelaySerializationFixtures.CreateBasePackage(
                directives: new RelayGovernanceDirectiveFlags(true, false),
                authorizationDispositionPresent: true,
                payload: SoftwareDevelopmentProfilePayloadSerializer.Empty) with
            {
                Kind = GovernedPackageKind.EngineeringResultImport,
            });

        var bridgeResult = _bridge.TryParseEngineeringResult(rendered);
        Assert.Equal(RelayValidationState.Incomplete, bridgeResult.Validation.State);

        var nullPath = new GovernedRelayPackageValidator(NullSoftwareDevelopmentRelayProfileValidator.Instance);
        var nullResult = nullPath.Validate(bridgeResult.Package!);
        Assert.Equal(RelayValidationState.Valid, nullResult.State);
    }

    private (GovernedRelayPackage Package, RelayValidationResult Validation) ImportValidPaHandover()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover();
        var validation = ValidatePaImport(package);
        Assert.Equal(RelayValidationState.Valid, validation.State);
        return (package, validation);
    }

    private RelayValidationResult ValidatePaImport(GovernedRelayPackage package)
    {
        var rendered = _paAdapter.RenderPaReviewPackage(package);
        return _paAdapter.TryParsePaHandoverImport(rendered).Validation;
    }

    private string RenderEngineeringResultPackage(GovernedRelayPackage package) =>
        _renderer.Render(package);
}
