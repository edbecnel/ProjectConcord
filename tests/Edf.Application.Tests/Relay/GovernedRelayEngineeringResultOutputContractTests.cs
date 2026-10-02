using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayEngineeringResultOutputContractTests
{
    private readonly EngineeringAgentManualRelayBridge _bridge = new();
    private readonly ProjectArchitectManualAdapter _paAdapter = new();

    [Fact]
    public void InstructionalTemplate_IsNotAcceptedAsValidEngineeringResultImport()
    {
        var (package, _) = ImportValidPaHandover();
        var template = GovernedRelayEngineeringResultOutputContract.RenderStructuralTemplateWithPlaceholders(package);

        var import = _bridge.TryParseEngineeringResult(template);

        Assert.NotEqual(RelayValidationState.Valid, import.Validation.State);
        Assert.Null(import.Package);
    }

    [Fact]
    public void CompletedExample_PassesRealTryParseEngineeringResult()
    {
        var (package, _) = ImportValidPaHandover();
        var completed = GovernedRelayEngineeringResultOutputContract.RenderCompletedValidExample(package);

        var import = _bridge.TryParseEngineeringResult(completed);

        Assert.Equal(RelayValidationState.Valid, import.Validation.State);
        Assert.NotNull(import.Package);
        Assert.Equal(GovernedPackageKind.EngineeringResultImport, import.Package!.Kind);
        Assert.Equal(package.ProjectId, import.Package.ProjectId);
        Assert.Equal(package.CorrelationId, import.Package.CorrelationId);
    }

    [Fact]
    public void CompleteContract_ContainsStructuralTemplate_RenderHeader_AndIsolationRules()
    {
        var (package, validation) = ImportValidPaHandover();
        var prepared = _bridge.TryRenderValidatedHandover(package, validation);
        var contract = _bridge.RenderEngineeringResultResponseInstruction(prepared.ExportPackage!);

        Assert.Contains(GovernedRelayEngineeringResultOutputContract.SectionHeading, contract, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, contract, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, contract, StringComparison.Ordinal);
        Assert.Contains("engineeringResultImport", contract, StringComparison.Ordinal);
        Assert.Contains(package.ProjectId.Value.ToString(), contract, StringComparison.Ordinal);
        Assert.Contains(package.CorrelationId.Value.ToString(), contract, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayEngineeringResultOutputContract.PlaceholderPackageId, contract, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayEngineeringResultOutputContract.PlaceholderUtcTimestamp, contract, StringComparison.Ordinal);
        Assert.Contains("RETURN ONLY THE COMPLETED PROJECTCONCORD RELAY DOCUMENT", contract, StringComparison.Ordinal);
        Assert.Contains("Do **not** include a preamble", contract, StringComparison.Ordinal);
        Assert.Contains("Machine / projection agreement", contract, StringComparison.Ordinal);
        Assert.Contains("Authorization safety", contract, StringComparison.Ordinal);
        Assert.DoesNotContain("Cursor", contract, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ACP", contract, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompleteContract_ExplicitSchemaVersion_AndGovernanceCriticalStructure()
    {
        var (package, _) = ImportValidPaHandover();
        var contract = GovernedRelayEngineeringResultOutputContract.RenderCompleteContract(package);

        Assert.Contains($"\"schemaVersionMajor\": {package.SchemaVersion.Major}", contract, StringComparison.Ordinal);
        Assert.Contains($"\"schemaVersionMinor\": {package.SchemaVersion.Minor}", contract, StringComparison.Ordinal);
        Assert.Contains("\"governanceCritical\"", contract, StringComparison.Ordinal);
        Assert.Contains("\"sessionContinuity\"", contract, StringComparison.Ordinal);
        Assert.Contains("\"tier0Snapshot\"", contract, StringComparison.Ordinal);
        Assert.Contains("\"softwareDevelopmentProfile\"", contract, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.GovernanceCriticalHeading, contract, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.StopHeading, contract, StringComparison.Ordinal);
    }

    private (GovernedRelayPackage Package, RelayValidationResult Validation) ImportValidPaHandover()
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover();
        var rendered = _paAdapter.RenderPaReviewPackage(package);
        var imported = _paAdapter.TryParsePaHandoverImport(rendered);
        Assert.True(imported.Validation.IsEligibleForValidatedEngineeringAgentHandover);
        return (imported.Package!, imported.Validation);
    }
}
