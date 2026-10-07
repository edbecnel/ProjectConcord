namespace Edf.Application.Relay;

using Edf.Domain.Relay;

/// <summary>
/// Operator-facing messages for manual PA/EA relay paste failures (presentation vs governance).
/// </summary>
public static class GovernedRelayManualPasteOperatorMessages
{
    public static string ComposeImportFailureOperatorMessage(RelayValidationResult validation)
    {
        var codes = validation.Diagnostics.Select(d => d.Code).ToHashSet(StringComparer.Ordinal);

        if (codes.Contains(RelayValidationCodes.GovernanceProjectionMismatch))
        {
            return "ProjectConcord read a Project Architect response, but its governance information is inconsistent. "
                   + "The response could not be accepted. Ask your Project Architect to send a corrected response.";
        }

        if (codes.Contains(RelayValidationCodes.ManualPasteRenderMarkerAmbiguous)
            || codes.Contains(RelayValidationCodes.TransportExtractionStartMarkerAmbiguous)
            || codes.Contains(RelayValidationCodes.TransportExtractionMachineBlockAmbiguous))
        {
            return "ProjectConcord found more than one governed response in what you pasted. "
                   + "Copy only the latest complete Project Architect reply and try again.";
        }

        if (codes.Contains(RelayValidationCodes.ManualPasteIncomplete)
            || codes.Contains(RelayValidationCodes.TransportExtractionEndBoundaryMissing)
            || codes.Contains(RelayValidationCodes.MachineBlockInvalidJson))
        {
            return "The Project Architect response looks incomplete. "
                   + "Copy the entire reply from your Project Architect and try again.";
        }

        if (codes.Contains(RelayValidationCodes.ManualPasteEmpty)
            || codes.Contains(RelayValidationCodes.ManualPasteRenderMarkerMissing)
            || codes.Contains(RelayValidationCodes.ManualPasteJsonOnlyRejected)
            || codes.Contains(RelayValidationCodes.ManualPasteMachineBlockOnlyRejected)
            || codes.Contains(RelayValidationCodes.ManualPasteMachineBlockMissing)
            || codes.Contains(RelayValidationCodes.MachineBlockMissing)
            || codes.Contains(RelayValidationCodes.RenderVersionMissing)
            || codes.Any(c => c.StartsWith("relay.manual_paste.", StringComparison.Ordinal))
            || codes.Any(c => c.StartsWith("relay.transport.extraction.", StringComparison.Ordinal)))
        {
            return "ProjectConcord could not find a complete Project Architect response in what you pasted. "
                   + "Copy the entire reply from your Project Architect and try again.";
        }

        return "ProjectConcord could not validate this Project Architect response. "
               + "Review the response with your Project Architect and try again.";
    }
}
