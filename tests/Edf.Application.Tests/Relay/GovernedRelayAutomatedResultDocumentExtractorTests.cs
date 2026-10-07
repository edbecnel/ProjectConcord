using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayAutomatedResultDocumentExtractorTests
{
    private readonly EngineeringAgentManualRelayBridge _bridge = new();

    [Fact]
    public void TryExtract_ValidDocumentOnly_ReturnsUnchanged()
    {
        var doc = RenderValidEngineeringResult();
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(doc, out var extracted, out _));
        Assert.Equal(doc.TrimEnd(), extracted);
    }

    [Fact]
    public void TryExtract_PreambleOnPriorLines_ReturnsExactDocument()
    {
        var doc = RenderValidEngineeringResult();
        var raw = $"Agent preamble line 1.\nLine 2.\n\n{doc}";
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out var extracted, out _));
        Assert.Equal(doc.TrimEnd(), extracted);
    }

    [Fact]
    public void TryExtract_MidLineRenderMarker_C2_ReturnsExactSuffix()
    {
        var doc = RenderValidEngineeringResult();
        var raw = BuildMidLineEnvelope(doc);
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out var extracted, out _));
        var reminderEnd = raw.IndexOf(GovernedRelayV1Format.PaEngineeringAgentReminder, StringComparison.Ordinal)
            + GovernedRelayV1Format.PaEngineeringAgentReminder.Length;
        var start = raw.IndexOf(GovernedRelayV1Format.RenderVersionLinePrefix, StringComparison.Ordinal);
        Assert.Equal(raw.Substring(start, reminderEnd - start), extracted);
        Assert.True(_bridge.TryParseEngineeringResult(extracted).Validation.State == RelayValidationState.Valid);
    }

    [Fact]
    public void TryExtract_TrailingProseAfterReminder_Excluded()
    {
        var doc = RenderValidEngineeringResult().TrimEnd();
        var raw = $"{doc}\n\nTrailing provider commentary.";
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out var extracted, out _));
        Assert.Equal(doc, extracted.TrimEnd());
        Assert.DoesNotContain("Trailing provider commentary", extracted, StringComparison.Ordinal);
    }

    [Fact]
    public void TryExtract_PreambleAndTrailing_Excluded()
    {
        var doc = RenderValidEngineeringResult().TrimEnd();
        var raw = $"Preamble.\n\n{doc}\n\nTrailing.";
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out var extracted, out _));
        Assert.Equal(doc, extracted.TrimEnd());
    }

    [Fact]
    public void TryExtract_IsByteIdenticalToRawSubstring()
    {
        var doc = RenderValidEngineeringResult();
        var raw = BuildMidLineEnvelope(doc);
        var start = raw.IndexOf(GovernedRelayV1Format.RenderVersionLinePrefix, StringComparison.Ordinal);
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out var extracted, out _));
        Assert.Equal(raw.Substring(start, extracted.Length), extracted);
    }

    [Fact]
    public void TryExtract_NoRenderMarker_Fails()
    {
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(
            "plain prose only",
            out _,
            out var failure));
        Assert.Contains(failure.Diagnostics, d => d.Code == RelayValidationCodes.TransportExtractionStartMarkerMissing);
    }

    [Fact]
    public void TryExtract_MissingEndReminder_Fails()
    {
        var doc = RenderValidEngineeringResult();
        var withoutReminder = doc.Replace(GovernedRelayV1Format.PaEngineeringAgentReminder, string.Empty, StringComparison.Ordinal);
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(withoutReminder, out _, out var failure));
        Assert.Contains(failure.Diagnostics, d => d.Code == RelayValidationCodes.TransportExtractionEndBoundaryMissing);
    }

    [Fact]
    public void TryExtract_MultipleRenderMarkers_Fails()
    {
        var doc = RenderValidEngineeringResult();
        var raw = doc + "\n\n" + doc;
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out _, out var failure));
        Assert.Contains(failure.Diagnostics, d => d.Code == RelayValidationCodes.TransportExtractionStartMarkerAmbiguous);
    }

    [Fact]
    public void TryExtract_MultipleMachineBlocks_Fails()
    {
        var doc = RenderValidEngineeringResult().TrimEnd();
        var reminder = GovernedRelayV1Format.PaEngineeringAgentReminder;
        var idx = doc.IndexOf(reminder, StringComparison.Ordinal);
        var extraBlock = $"\n\n```{GovernedRelayV1Format.MachineBlockFenceLanguage}\n{{}}\n```\n\n";
        var raw = doc.Insert(idx, extraBlock);
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out _, out var failure));
        Assert.Contains(failure.Diagnostics, d => d.Code == RelayValidationCodes.TransportExtractionMachineBlockAmbiguous);
    }

    [Fact]
    public void TryExtract_MultipleEndReminders_Fails()
    {
        var doc = RenderValidEngineeringResult();
        var raw = GovernedRelayV1Format.PaEngineeringAgentReminder + "\n" + doc;
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out _, out var failure));
        Assert.Contains(failure.Diagnostics, d => d.Code == RelayValidationCodes.TransportExtractionEndBoundaryAmbiguous);
    }

    [Fact]
    public void TryExtract_ReminderBeforeMarker_Fails()
    {
        var doc = RenderValidEngineeringResult();
        var raw = GovernedRelayV1Format.PaEngineeringAgentReminder + "\n" + doc.Replace(GovernedRelayV1Format.PaEngineeringAgentReminder, "X", StringComparison.Ordinal);
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out _, out _));
    }

    [Fact]
    public void TryExtract_MalformedThenValidMarker_FailClosed()
    {
        var valid = RenderValidEngineeringResult();
        var raw = "broken.ProjectConcord-Relay-Render: 99\n\n" + valid;
        Assert.False(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out _, out var failure));
        Assert.Contains(failure.Diagnostics, d => d.Code == RelayValidationCodes.TransportExtractionStartMarkerAmbiguous);
    }

    [Fact]
    public void ManualBridge_MidLineEnvelope_IsToleratedBySharedManualPasteReader()
    {
        var doc = RenderValidEngineeringResult();
        var raw = BuildMidLineEnvelope(doc);
        Assert.Equal(RelayValidationState.Valid, _bridge.TryParseEngineeringResult(raw).Validation.State);
    }

    [Fact]
    public void ExtractedMidLineEnvelope_PassesExistingTryParseEngineeringResult()
    {
        var doc = RenderValidEngineeringResult();
        var raw = BuildMidLineEnvelope(doc);
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(raw, out var extracted, out _));
        var import = _bridge.TryParseEngineeringResult(extracted);
        Assert.Equal(RelayValidationState.Valid, import.Validation.State);
        Assert.Equal(GovernedPackageKind.EngineeringResultImport, import.Package!.Kind);
    }

    [Fact]
    public void RegressionFixture_MidLineMarkerShape_MatchesC2Expectations()
    {
        var fixture = EngineeringResultMidLineEnvelopeFixtures.Load();
        Assert.True(GovernedRelayAutomatedResultDocumentExtractor.TryExtract(fixture.RawEnvelope, out var extracted, out _));
        var reminderEnd = fixture.RawEnvelope.IndexOf(GovernedRelayV1Format.PaEngineeringAgentReminder, StringComparison.Ordinal)
            + GovernedRelayV1Format.PaEngineeringAgentReminder.Length;
        var start = fixture.RawEnvelope.IndexOf(GovernedRelayV1Format.RenderVersionLinePrefix, StringComparison.Ordinal);
        Assert.Equal(fixture.RawEnvelope.Substring(start, reminderEnd - start), extracted);
        Assert.Equal(RelayValidationState.Valid, _bridge.TryParseEngineeringResult(extracted).Validation.State);
    }

    private static string BuildMidLineEnvelope(string doc)
    {
        var trimmed = doc.TrimEnd();
        var firstNewline = trimmed.IndexOf('\n');
        var firstLine = trimmed[..firstNewline];
        var remainder = trimmed[(firstNewline + 1)..];
        return $"I read the handover. No file changes.\nMore prose.{firstLine}\n{remainder}\nAfterthought commentary.";
    }

    private static string RenderValidEngineeringResult(ProjectConcordProjectId? projectId = null)
    {
        var package = RelaySerializationFixtures.CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            authorizationDispositionPresent: false,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty) with
        {
            ProjectId = projectId ?? ProjectConcordProjectId.New(),
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
        return new GovernedRelayV1Renderer().Render(package);
    }
}
