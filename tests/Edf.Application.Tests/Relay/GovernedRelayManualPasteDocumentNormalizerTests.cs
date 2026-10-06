using System.Text.RegularExpressions;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayManualPasteDocumentNormalizerTests
{
    private readonly ProjectArchitectManualAdapter _paAdapter = new();
    private readonly EngineeringAgentManualRelayBridge _eaBridge = new();

    [Fact]
    public void PaContractTemplate_UsesOuterPlainTextCopyFence_WithCompleteCanonicalInside()
    {
        var reviewPackage = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            Kind = GovernedPackageKind.PaReviewExport,
        };
        var wrapped = GovernedRelayPaHandoverOutputContract.RenderStructuralTemplateWithPlaceholders(
            reviewPackage,
            PaHandoverResponseProfile.PlanningEntry);

        Assert.StartsWith($"```{GovernedRelayManualPasteCopyFence.OuterFenceLanguage}", wrapped, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, wrapped, StringComparison.Ordinal);
        Assert.Contains($"```{GovernedRelayV1Format.MachineBlockFenceLanguage}", wrapped, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.GovernanceCriticalHeading, wrapped, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.StopHeading, wrapped, StringComparison.Ordinal);
    }

    [Fact]
    public void EngineeringResultContractTemplate_UsesSameOuterCopyFence()
    {
        var (handover, _) = ImportValidImplementationHandover();
        var wrapped = GovernedRelayEngineeringResultOutputContract.RenderStructuralTemplateWithPlaceholders(handover);

        Assert.StartsWith($"```{GovernedRelayManualPasteCopyFence.OuterFenceLanguage}", wrapped, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, wrapped, StringComparison.Ordinal);
        Assert.DoesNotContain("Cursor", wrapped, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WrapForManualCopy_PreservesCanonicalRelayBytes()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical);

        Assert.True(
            GovernedRelayManualPasteDocumentNormalizer.TryNormalizeToCanonicalRelayDocument(
                wrapped,
                out var normalized,
                out _));
        Assert.Equal(canonical.Trim(), normalized.Trim());
    }

    [Fact]
    public void AlreadyCanonical_UnwrappedDocument_RemainsAccepted()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var result = _paAdapter.TryParsePaHandoverImport(canonical);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
    }

    [Fact]
    public void SingleOuterWrapper_NormalizesThenImports_ValidPlanningEntry()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical);
        var result = _paAdapter.TryParsePaHandoverImport(wrapped);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.True(
            PlanningEntryPaHandoverContract.SatisfiesPlanningEntryContract(
                result.Package!,
                result.Validation,
                out _));
    }

    [Fact]
    public void JsonOnlyPaste_IsRejected()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var jsonOnly = ExtractMachineJson(canonical);

        var result = _paAdapter.TryParsePaHandoverImport(jsonOnly);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.Validation.State);
        Assert.Contains(
            RelayValidationCodes.ManualPasteJsonOnlyRejected,
            result.Validation.Diagnostics.Select(d => d.Code),
            StringComparer.Ordinal);
    }

    [Fact]
    public void MachineBlockOnlyPaste_IsRejected()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var match = Regex.Match(
            canonical,
            $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\r?\\n([\\s\\S]*?)\\r?\\n```",
            RegexOptions.CultureInvariant);
        Assert.True(match.Success);
        var machineOnly = $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\n{match.Groups[1].Value}\n```";

        var result = _paAdapter.TryParsePaHandoverImport(machineOnly);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.Validation.State);
        Assert.Contains(
            RelayValidationCodes.ManualPasteMachineBlockOnlyRejected,
            result.Validation.Diagnostics.Select(d => d.Code),
            StringComparer.Ordinal);
    }

    [Fact]
    public void MultipleRenderMarkers_AreRejected()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var doubled = canonical + "\n\n" + canonical;

        var result = _paAdapter.TryParsePaHandoverImport(doubled);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.Validation.State);
        Assert.Contains(
            RelayValidationCodes.ManualPasteRenderMarkerAmbiguous,
            result.Validation.Diagnostics.Select(d => d.Code),
            StringComparer.Ordinal);
    }

    [Fact]
    public void NestedCompetingOuterWrappers_AreRejected()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var doubleWrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(
            GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical));

        var result = _paAdapter.TryParsePaHandoverImport(doubleWrapped);

        Assert.NotEqual(RelayValidationState.Valid, result.Validation.State);
    }

    [Fact]
    public void CompletedNegativePlanningEntry_Wrapped_ImportsValid_ButNotEligible()
    {
        var reviewPackage = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            Kind = GovernedPackageKind.PaReviewExport,
        };
        var completed = GovernedRelayPaHandoverOutputContract.RenderCompletedPlanningEntryHandover(
            reviewPackage,
            planningAuthorized: false,
            "Planning entry not authorized.");
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(completed);

        var result = _paAdapter.TryParsePaHandoverImport(wrapped);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.False(
            PlanningEntryPaHandoverContract.SatisfiesPlanningEntryContract(
                result.Package!,
                result.Validation,
                out _));
    }

    [Fact]
    public void EngineeringResultImport_UsesSameNormalizationPath()
    {
        var (handover, validation) = ImportValidImplementationHandover();
        var prepared = _eaBridge.TryRenderValidatedHandover(handover, validation);
        var completed = GovernedRelayEngineeringResultOutputContract.RenderCompletedValidExample(prepared.ExportPackage!);
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(completed);

        var result = _eaBridge.TryParseEngineeringResult(wrapped);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.Equal(GovernedPackageKind.EngineeringResultImport, result.Package!.Kind);
    }

    [Fact]
    public void Normalization_PreservesRequiredProjections_WhenPresent()
    {
        var canonical = LoadMvr0005PlanningEntryFixture();
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical);

        Assert.True(
            GovernedRelayManualPasteDocumentNormalizer.TryNormalizeToCanonicalRelayDocument(
                wrapped,
                out var normalized,
                out _));

        Assert.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, normalized, StringComparison.Ordinal);
        Assert.Contains($"```{GovernedRelayV1Format.MachineBlockFenceLanguage}", normalized, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.GovernanceCriticalHeading, normalized, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.StopHeading, normalized, StringComparison.Ordinal);
        Assert.Contains("## Authorization-Disposition", normalized, StringComparison.Ordinal);
        Assert.Equal(ExtractMachineJson(canonical), ExtractMachineJson(normalized));
    }

    private static string LoadMvr0005PlanningEntryFixture()
    {
        var path = Path.Combine(
            RepoRoot.Find(),
            "docs",
            "Verification",
            "Fixtures",
            "MVR-0005",
            "pa-handover-planning-entry-valid.relay.txt");
        return File.ReadAllText(path);
    }

    private static string ExtractMachineJson(string canonical)
    {
        var match = Regex.Match(
            canonical,
            $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\r?\\n([\\s\\S]*?)\\r?\\n```",
            RegexOptions.CultureInvariant);
        Assert.True(match.Success);
        return match.Groups[1].Value.Trim();
    }

    private (GovernedRelayPackage Package, RelayValidationResult Validation) ImportValidImplementationHandover()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover();
        var rendered = _paAdapter.RenderPaReviewPackage(package);
        var imported = _paAdapter.TryParsePaHandoverImport(rendered);
        Assert.True(imported.Validation.IsEligibleForValidatedEngineeringAgentHandover);
        return (imported.Package!, imported.Validation);
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
