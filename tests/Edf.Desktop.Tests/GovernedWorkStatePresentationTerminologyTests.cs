using Edf.Desktop.ViewModels;

namespace Edf.Desktop.Tests;

public class GovernedWorkStatePresentationTerminologyTests
{
    [Fact]
    public void PostPlanningDwa_OrdinaryCopy_DoesNotExposePlanningRegionTerminology()
    {
        var next = GovernedWorkStatePresentation.ComposeContinuePlanningRegionWorkNextStepSummary();
        var explanation = GovernedWorkStatePresentation.ComposePlanningRegionGovernedExchangeActionExplanation();

        Assert.Equal("Continue planning work in Guided Work.", next);
        Assert.DoesNotContain("planning-region", next, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Planning-Region", next, StringComparison.Ordinal);
        Assert.DoesNotContain("planning-region", explanation, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not grant implementation authorization", explanation, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("do not create durable development work authorization", explanation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CurrentWorkPrimaryActionButton_Copy_IsContinueGuidedPlanningWork()
    {
        const string expected = "Continue Guided Planning Work";
        var axamlPath = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "Edf.Desktop", "MainWindow.axaml");
        axamlPath = Path.GetFullPath(axamlPath);
        var axaml = File.ReadAllText(axamlPath);
        Assert.Contains($"Content=\"{expected}\"", axaml, StringComparison.Ordinal);
        Assert.DoesNotContain("Planning-Region Work", axaml, StringComparison.Ordinal);
    }

    [Fact]
    public void PlanningRegionWorkStepCode_PlainDescription_UsesGuidedPlanningLanguage()
    {
        var plain = GovernedWorkStatePresentation.DescribeNextStepCodePlain(
            Edf.Application.Workflow.Eligibility.WorkflowEligibilityReasonCodes.NextActionPlanningRegionWork);

        Assert.DoesNotContain("planning-region", plain, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("guided planning work", plain, StringComparison.OrdinalIgnoreCase);
    }
}
