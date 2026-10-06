using Edf.Application.Relay;
using Edf.Domain.Relay;
using Edf.Desktop.ViewModels;
using System.Reflection;

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

        var message = InvokeComposeImportFailureOperatorMessage(validation);

        Assert.Contains("governance information is inconsistent", message, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("outer plain-text", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ManualPasteFailure_UsesTransportRecopyGuidance()
    {
        var validation = RelayValidationResult.RejectedMalformed(
        [
            new RelayValidationDiagnostic(
                RelayValidationCodes.ManualPasteJsonOnlyRejected,
                "JSON-only paste rejected.",
                RelayValidationDiagnosticSeverity.Malformed),
        ]);

        var message = InvokeComposeImportFailureOperatorMessage(validation);

        Assert.Contains("complete", message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not JSON alone", message, StringComparison.OrdinalIgnoreCase);
    }

    private static string InvokeComposeImportFailureOperatorMessage(RelayValidationResult validation)
    {
        var method = typeof(PlanningEntryGuidedExchangeViewModel).GetMethod(
            "ComposeImportFailureOperatorMessage",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);
        return (string)method!.Invoke(null, [validation])!;
    }
}
