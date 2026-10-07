using Edf.Application.Relay;
using Edf.Domain.Relay;

namespace Edf.Desktop.Tests;

public class PlanningEntryGuidedImportFailureMessageTests
{
    [Fact]
    public void GovernanceProjectionMismatch_UsesInconsistencyMessage_NotRecopyTransport()
    {
        var validation = RelayValidationResult.RejectedMalformed(
        [
            new RelayValidationDiagnostic(
                RelayValidationCodes.GovernanceProjectionMismatch,
                "Machine block and projected governance-critical fields disagree.",
                RelayValidationDiagnosticSeverity.Malformed),
        ]);

        var message = GovernedRelayManualPasteOperatorMessages.ComposeImportFailureOperatorMessage(validation);

        Assert.Contains("governance information is inconsistent", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("outer plain-text", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("backtick", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ManualPasteFailure_UsesEntireReplyGuidance()
    {
        var validation = RelayValidationResult.RejectedMalformed(
        [
            new RelayValidationDiagnostic(
                RelayValidationCodes.ManualPasteJsonOnlyRejected,
                "JSON-only paste rejected.",
                RelayValidationDiagnosticSeverity.Malformed),
        ]);

        var message = GovernedRelayManualPasteOperatorMessages.ComposeImportFailureOperatorMessage(validation);

        Assert.Contains("entire reply", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("backtick", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AmbiguousPaste_UsesSingleResponseGuidance()
    {
        var validation = RelayValidationResult.RejectedMalformed(
        [
            new RelayValidationDiagnostic(
                RelayValidationCodes.ManualPasteRenderMarkerAmbiguous,
                "Multiple markers.",
                RelayValidationDiagnosticSeverity.Malformed),
        ]);

        var message = GovernedRelayManualPasteOperatorMessages.ComposeImportFailureOperatorMessage(validation);
        Assert.Contains("more than one", message, StringComparison.OrdinalIgnoreCase);
    }
}
