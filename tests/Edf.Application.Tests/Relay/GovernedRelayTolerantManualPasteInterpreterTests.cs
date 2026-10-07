using System.Text.Json;
using System.Text.RegularExpressions;
using Edf.Application.Relay;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Workflow.PlanningAuthorization;
using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Application.Tests.Workflow.PlanningAuthorization;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayTolerantManualPasteInterpreterTests
{
    private readonly ProjectArchitectManualAdapter _paAdapter = new();

    [Fact]
    public void PerfectCanonical_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(canonical, canonical);
    }

    [Fact]
    public void NoOuterFence_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(canonical, canonical);
    }

    [Fact]
    public void NormalOuterFence_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical);
        AssertImportValid(canonical, wrapped);
    }

    [Fact]
    public void NestedEqualLengthOuterFences_Accept()
    {
        var canonical = LoadPlanningEntryFixture();
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(
            GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical));
        AssertImportValid(canonical, wrapped);
    }

    [Fact]
    public void LongerOuterFence_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        var outer = $"````text{Environment.NewLine}{canonical.TrimEnd()}{Environment.NewLine}````";
        AssertImportValid(canonical, outer);
    }

    [Fact]
    public void MissingOuterClosingFence_AcceptsWhenCanonicalRecoverable()
    {
        var canonical = LoadPlanningEntryFixture();
        var damaged = $"```text{Environment.NewLine}{canonical.TrimEnd()}";
        AssertImportValid(canonical, damaged);
    }

    [Fact]
    public void ExtraOuterClosingFence_AcceptsWhenSingleArtifact()
    {
        var canonical = LoadPlanningEntryFixture();
        var wrapped = GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical);
        var damaged = wrapped + Environment.NewLine + "```";
        AssertImportValid(canonical, damaged);
    }

    [Fact]
    public void ProseBefore_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(canonical, $"Here is the response.{Environment.NewLine}{Environment.NewLine}{canonical}");
    }

    [Fact]
    public void ProseAfter_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(canonical, $"{canonical}{Environment.NewLine}Let me know if you need more.");
    }

    [Fact]
    public void ProseBeforeAndAfter_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(
            canonical,
            $"Heading{Environment.NewLine}{canonical}{Environment.NewLine}Thanks.");
    }

    [Fact]
    public void ExtraBlankLines_Accept()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(canonical, canonical.Replace("\n", "\n\n", StringComparison.Ordinal));
    }

    [Fact]
    public void CrlfVariation_Accepts()
    {
        var canonical = LoadPlanningEntryFixture();
        AssertImportValid(canonical, canonical.Replace("\n", "\r\n", StringComparison.Ordinal));
    }

    [Fact]
    public void MalformedMachineFence_RecoversStructurally()
    {
        var canonical = LoadPlanningEntryFixture();
        var json = ExtractMachineJson(canonical);
        var projections = canonical[(canonical.IndexOf(GovernedRelayV1Format.GovernanceCriticalHeading, StringComparison.Ordinal))..];
        var damaged =
            $"{GovernedRelayV1Format.RenderVersionLinePrefix} 1{Environment.NewLine}{Environment.NewLine}"
            + $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}{Environment.NewLine}{json}{Environment.NewLine}"
            + projections;

        AssertImportValid(canonical, damaged);
    }

    [Fact]
    public void TwoCompleteResponses_Reject()
    {
        var canonical = LoadPlanningEntryFixture();
        var doubled = canonical + Environment.NewLine + Environment.NewLine + canonical;
        var result = _paAdapter.TryParsePaHandoverImport(doubled);
        Assert.Equal(RelayValidationState.RejectedMalformed, result.Validation.State);
    }

    [Fact]
    public void JsonOnly_Reject()
    {
        var json = ExtractMachineJson(LoadPlanningEntryFixture());
        var result = _paAdapter.TryParsePaHandoverImport(json);
        Assert.Contains(
            RelayValidationCodes.ManualPasteJsonOnlyRejected,
            result.Validation.Diagnostics.Select(d => d.Code),
            StringComparer.Ordinal);
    }

    [Fact]
    public void MachineBlockOnly_Reject()
    {
        var canonical = LoadPlanningEntryFixture();
        var match = Regex.Match(
            canonical,
            $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\\s*\\r?\\n([\\s\\S]*?)\\r?\\n```",
            RegexOptions.CultureInvariant);
        var machineOnly = $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}\n{match.Groups[1].Value}\n```";
        var result = _paAdapter.TryParsePaHandoverImport(machineOnly);
        Assert.Contains(
            RelayValidationCodes.ManualPasteMachineBlockOnlyRejected,
            result.Validation.Diagnostics.Select(d => d.Code),
            StringComparer.Ordinal);
    }

    [Fact]
    public void TruncatedJson_Reject()
    {
        var canonical = LoadPlanningEntryFixture();
        var json = ExtractMachineJson(canonical);
        var truncated = json[..(json.Length / 2)];
        var damaged =
            $"{GovernedRelayV1Format.RenderVersionLinePrefix} 1{Environment.NewLine}{Environment.NewLine}"
            + $"```{GovernedRelayV1Format.MachineBlockFenceLanguage}{Environment.NewLine}{truncated}{Environment.NewLine}```"
            + canonical[(canonical.IndexOf(GovernedRelayV1Format.GovernanceCriticalHeading, StringComparison.Ordinal))..];
        var result = _paAdapter.TryParsePaHandoverImport(damaged);
        Assert.NotEqual(RelayValidationState.Valid, result.Validation.State);
    }

    [Fact]
    public void PlanningDwa_CompletedHandover_ToleratesProseWrapper()
    {
        var projectId = ProjectConcordProjectId.Parse("aff1297f-f0fe-475c-b2b4-ddf27663caff");
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId);
        var rendered = new GovernedRelayV1Renderer().Render(handover);
        var wrapped = $"PA note.{Environment.NewLine}{GovernedRelayManualPasteCopyFence.WrapForManualCopy(rendered)}";

        var result = _paAdapter.TryParsePaHandoverImport(wrapped);
        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.True(
            PlanningAuthorizationPaHandoverContract.SatisfiesPlanningAuthorizationGrantContract(
                result.Package!,
                result.Validation,
                out _));
    }

    [Fact]
    public void Normalization_PreservesGovernanceSignificantMachineJson()
    {
        var canonical = LoadPlanningEntryFixture();
        var proseWrapped =
            $"Intro{Environment.NewLine}{GovernedRelayManualPasteCopyFence.WrapForManualCopy(canonical)}";
        Assert.True(
            GovernedRelayManualPasteDocumentNormalizer.TryNormalizeToCanonicalRelayDocument(
                proseWrapped,
                out var normalized,
                out _));
        Assert.True(MachineJsonSemanticallyEqual(ExtractMachineJson(canonical), ExtractMachineJson(normalized)));
    }

    private void AssertImportValid(string canonical, string pasted)
    {
        Assert.True(
            GovernedRelayManualPasteDocumentNormalizer.TryNormalizeToCanonicalRelayDocument(
                pasted,
                out var normalized,
                out var failure),
            string.Join("; ", failure.Diagnostics.Select(d => d.Code)));

        var result = _paAdapter.TryParsePaHandoverImport(pasted);
        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.True(MachineJsonSemanticallyEqual(ExtractMachineJson(canonical), ExtractMachineJson(normalized)));
    }

    private static bool MachineJsonSemanticallyEqual(string left, string right)
    {
        using var leftDoc = JsonDocument.Parse(left);
        using var rightDoc = JsonDocument.Parse(right);
        return JsonSerializer.Serialize(leftDoc) == JsonSerializer.Serialize(rightDoc);
    }

    private static string LoadPlanningEntryFixture()
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
