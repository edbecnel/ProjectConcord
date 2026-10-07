namespace Edf.Application.Operator.PlanningAuthorization;

using Edf.Application.Relay;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Relay;

public static class PlanningAuthorizationReviewExportRenderer
{
    public static string RenderCompleteClipboardPayload(GovernedRelayPackage reviewExportPackage)
    {
        ArgumentNullException.ThrowIfNull(reviewExportPackage);
        var adapter = new ProjectArchitectManualAdapter();
        var reviewBody = adapter.RenderPaReviewPackage(reviewExportPackage);
        var contract = GovernedRelayPaHandoverResponseInstruction.Render(
            reviewExportPackage,
            PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization);
        return reviewBody.TrimEnd()
            + Environment.NewLine
            + Environment.NewLine
            + contract
            + Environment.NewLine;
    }
}
