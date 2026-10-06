using System.Text.RegularExpressions;
using Edf.Application.Relay;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Tests.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayGovernanceProjectionBooleanTests
{
    private readonly ProjectArchitectManualAdapter _adapter = new();
    private readonly GovernedRelayV1Importer _importer = new();

    [Theory]
    [InlineData("true")]
    [InlineData("True")]
    [InlineData("yes")]
    [InlineData("Yes")]
    public void PlanningAuthorizedTrueLexicals_MachineTrue_Imports(string planningLexical)
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: true, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, planningLexical, "No");

        AssertStructuralImportSucceeds(rendered);
    }

    [Theory]
    [InlineData("false")]
    [InlineData("False")]
    [InlineData("no")]
    [InlineData("No")]
    public void PlanningAuthorizedFalseLexicals_MachineFalse_Imports(string planningLexical)
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: false, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, planningLexical, "No");

        AssertStructuralImportSucceeds(rendered);
    }

    [Fact]
    public void TryParse_UnknownBoolean_FailsStructuralParse()
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: true, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, "maybe", "no");

        var attempt = _importer.Import(rendered);

        Assert.Equal(RelayValidationState.RejectedMalformed, attempt.StructuralResult.State);
        Assert.Contains(
            attempt.StructuralResult.Diagnostics,
            d => d.Message.Contains("Unrecognized boolean projection value", StringComparison.Ordinal));
    }

    [Fact]
    public void MachineTrue_ProjectionYes_Agrees()
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: true, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, "Yes", "No");

        AssertStructuralImportSucceeds(rendered);
    }

    [Fact]
    public void MachineFalse_ProjectionNo_Agrees()
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: false, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, "No", "No");

        AssertStructuralImportSucceeds(rendered);
    }

    [Fact]
    public void MachineTrue_ProjectionFalse_ProducesMismatch()
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: true, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, "false", "false");

        var attempt = _importer.Import(rendered);

        Assert.Equal(RelayValidationState.RejectedMalformed, attempt.StructuralResult.State);
        Assert.Contains(
            attempt.StructuralResult.Diagnostics,
            d => d.Code == RelayValidationCodes.GovernanceProjectionMismatch);
    }

    [Fact]
    public void MachineFalse_ProjectionTrue_ProducesMismatch()
    {
        var rendered = BuildPlanningEntryHandover(planningAuthorized: false, implementationAuthorized: false);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, "true", "false");

        var attempt = _importer.Import(rendered);

        Assert.Equal(RelayValidationState.RejectedMalformed, attempt.StructuralResult.State);
        Assert.Contains(
            attempt.StructuralResult.Diagnostics,
            d => d.Code == RelayValidationCodes.GovernanceProjectionMismatch);
    }

    [Fact]
    public void RealShape_YesNoPlanningEntryHandover_ImportsSuccessfully()
    {
        var projectId = ProjectConcordProjectId.New();
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(projectId);
        var rendered = new GovernedRelayV1Renderer().Render(package);
        rendered = ReplaceAuthorizationDispositionBooleans(rendered, "Yes", "No");

        var result = _adapter.TryParsePaHandoverImport(rendered);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.Package);
    }

    [Fact]
    public void PaHandoverOutputContract_SpecifiesTrueFalse_NotYesNo()
    {
        var reviewPackage = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            Kind = GovernedPackageKind.PaReviewExport,
        };
        var contract = GovernedRelayPaHandoverOutputContract.RenderCompleteContract(
            reviewPackage,
            PaHandoverResponseProfile.PlanningEntry);

        Assert.Contains("lowercase `true` or `false`", contract, StringComparison.Ordinal);
        Assert.DoesNotContain("use `Yes`/`No`", contract, StringComparison.Ordinal);
        Assert.Contains("Planning-Authorized", contract, StringComparison.Ordinal);
        Assert.Contains("Implementation-Authorized", contract, StringComparison.Ordinal);
        var template = GovernedRelayPaHandoverOutputContract.RenderStructuralTemplateWithPlaceholders(
            reviewPackage,
            PaHandoverResponseProfile.PlanningEntry);
        Assert.Contains("Planning-Authorized: false", template, StringComparison.Ordinal);
        Assert.Contains("Implementation-Authorized: false", template, StringComparison.Ordinal);
        Assert.DoesNotContain("Planning-Authorized: Yes", template, StringComparison.Ordinal);
    }

    [Fact]
    public void CanonicalTrueFalseFixture_StillImports()
    {
        var path = Path.Combine(
            RepoRoot.Find(),
            "docs",
            "Verification",
            "Fixtures",
            "MVR-0005",
            "pa-handover-planning-entry-valid.relay.txt");
        var rendered = File.ReadAllText(path);
        var result = _adapter.TryParsePaHandoverImport(rendered);
        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
    }

    private static string BuildPlanningEntryHandover(bool planningAuthorized, bool implementationAuthorized)
    {
        var projectId = ProjectConcordProjectId.New();
        var package = Mvr0005PlanningEntryRelayFixtures.CreatePlanningEntryHandover(
            projectId,
            configurePayload: payload => payload with
            {
                AuthorizationDisposition = new AuthorizationDispositionProjection(
                    "PLANNING AUTHORIZED",
                    planningAuthorized,
                    implementationAuthorized,
                    null),
            });
        return new GovernedRelayV1Renderer().Render(package);
    }

    private void AssertStructuralImportSucceeds(string rendered)
    {
        var attempt = _importer.Import(rendered);
        Assert.NotEqual(RelayValidationState.RejectedMalformed, attempt.StructuralResult.State);
        Assert.DoesNotContain(
            attempt.StructuralResult.Diagnostics,
            d => d.Code == RelayValidationCodes.GovernanceProjectionMismatch);
    }

    private static string ReplaceAuthorizationDispositionBooleans(
        string rendered,
        string planningAuthorized,
        string implementationAuthorized)
    {
        rendered = Regex.Replace(
            rendered,
            @"Planning-Authorized:\s*\S+",
            $"Planning-Authorized: {planningAuthorized}",
            RegexOptions.CultureInvariant);
        return Regex.Replace(
            rendered,
            @"Implementation-Authorized:\s*\S+",
            $"Implementation-Authorized: {implementationAuthorized}",
            RegexOptions.CultureInvariant);
    }

    private static class RepoRoot
    {
        public static string Find()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null)
            {
                if (File.Exists(Path.Combine(dir.FullName, "ProjectConcord.sln")))
                {
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            throw new InvalidOperationException("Repository root not found.");
        }
    }
}
