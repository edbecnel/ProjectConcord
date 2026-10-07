namespace Edf.Application.Operator.PlanningRegion;

using Edf.Application.Relay;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Relay;

public static class PlanningRegionReviewExportRenderer
{
    public static string RenderCompleteClipboardPayload(GovernedRelayPackage reviewExportPackage)
    {
        ArgumentNullException.ThrowIfNull(reviewExportPackage);
        var adapter = new ProjectArchitectManualAdapter();
        var reviewBody = adapter.RenderPaReviewPackage(reviewExportPackage);
        var contract = GovernedRelayPaHandoverResponseInstruction.Render(
            reviewExportPackage,
            PaHandoverResponseProfile.PlanningEntry);
        return reviewBody.TrimEnd()
            + Environment.NewLine
            + Environment.NewLine
            + contract
            + Environment.NewLine;
    }
}
