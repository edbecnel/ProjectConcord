using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

// Deterministic MVR-0002 paste fixtures (automation support — not human attestation).
// Regenerate: dotnet run --project docs/Verification/Fixtures/MVR-0002/GenerateFixtures

var outputDir = args.Length > 0
    ? args[0]
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

Directory.CreateDirectory(outputDir);

var paAdapter = new ProjectArchitectManualAdapter();
var renderer = new GovernedRelayV1Renderer();
var bridge = new Edf.Application.Relay.EngineeringAgent.EngineeringAgentManualRelayBridge();

Write("README.md", """
# MVR-0002 relay paste fixtures

Automation-prepared **test data** for [MVR-0002](../../Records/MVR-0002-a2-p0-manual-governed-relay-workflow.md).  
Copy file contents into the Desktop **PA handover import** or **Engineering result import** fields as directed by each MVT.

**Regenerate** (from ProjectConcord repository root, SDK 10.0.401):

```bash
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$PATH"
dotnet run --project docs/Verification/Fixtures/MVR-0002/GenerateFixtures -- \
  docs/Verification/Fixtures/MVR-0002
```

""");

var valid = BuildValidPaHandoverPackage();
var validRendered = paAdapter.RenderPaReviewPackage(valid);
Write("pa-handover-valid.relay.txt", validRendered);
Write("ui-session-settings-for-valid-fixtures.md", """
# UI control settings before pasting `pa-handover-valid.relay.txt` (and derivatives)

Set these **before** MVT-10 / MVT-11 / MVT-12:

| Control | Value |
| --- | --- |
| Project Architect session | **CONTINUE** |
| Engineering Agent session | **NEW** |
| Engineering Agent mode | **AGENT** |
| Prior Engineering Agent mode | **PLAN** |

""");

var incompletePackage = valid with
{
    GovernanceCritical = valid.GovernanceCritical with
    {
        EngineeringAgentMode = null,
        PriorEngineeringAgentMode = null,
        ModeTransition = null,
    },
};
Write("pa-handover-incomplete.relay.txt", paAdapter.RenderPaReviewPackage(incompletePackage));

var rejectedMissingBlock = """
ProjectConcord-Relay-Render: 1

## Governance-Critical
Engineering-Agent-Mode: AGENT
""";
Write("pa-handover-rejected-malformed-missing-machine-block.relay.txt", rejectedMissingBlock);

var rejectedModeMismatch = validRendered.Replace(
    $"{GovernedRelayV1Format.EngineeringAgentModeField}: AGENT",
    $"{GovernedRelayV1Format.EngineeringAgentModeField}: PLAN",
    StringComparison.Ordinal);
Write("pa-handover-rejected-malformed-mode-mismatch.relay.txt", rejectedModeMismatch);

var stopPackage = valid with
{
    GovernanceCritical = valid.GovernanceCritical with
    {
        Stop = new RelayStopMetadata(RelayStopState.Active, "STOP"),
    },
};
Write("pa-handover-valid-stop-active.relay.txt", paAdapter.RenderPaReviewPackage(stopPackage));

var validImport = paAdapter.TryParsePaHandoverImport(validRendered);
var handover = bridge.TryRenderValidatedHandover(validImport.Package!, validImport.Validation);
Write("engineering-agent-handover-from-valid.relay.txt", handover.RenderedHandover ?? "(handover not ready)");

var resultPackage = RelaySerializationFixturesThin.CreateEngineeringResultImport();
Write("engineering-result-import-thin.relay.txt", renderer.Render(resultPackage));

Console.WriteLine($"Wrote MVR-0002 fixtures to: {outputDir}");

void Write(string name, string content) =>
    File.WriteAllText(Path.Combine(outputDir, name), content.TrimEnd() + Environment.NewLine);

static GovernedRelayPackage BuildValidPaHandoverPackage()
{
    var tier0 = new Tier0RelaySnapshot(
        "abc123",
        ["docs/Program/Gate_Reviews/"],
        new Dictionary<string, string> { ["gate"] = "G1" });

    var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
    {
        AuthorizationDisposition = new AuthorizationDispositionProjection(
            "IMPLEMENTATION AUTHORIZED",
            false,
            true,
            "A2-MVR"),
        DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
            SoftwareDevelopmentAuthorizationKind.Implementation,
            "A2-MVR",
            ["A2-MVR-scope"],
            "dwa-projection-ref",
            false),
        WorkContext = new WorkContextProjection("A2-MVR", "MVR disposable verification tranche"),
    };

    var bytes = SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload);

    var governance = new RelayGovernanceCriticalState(
        EngineeringAgentMode.Agent,
        EngineeringAgentMode.Plan,
        new EngineeringAgentModeTransition(EngineeringAgentMode.Plan, EngineeringAgentMode.Agent),
        new RelaySessionContinuity(
            ProjectArchitectSessionIntent: AgentSessionIntent.Continue,
            ProjectArchitectSessionAdvisory: AgentSessionAdvisory.None,
            EngineeringAgentSessionIntent: AgentSessionIntent.New,
            EngineeringAgentSessionAdvisory: AgentSessionAdvisory.None),
        RelayStopMetadata.None,
        true,
        true,
        new RelayGovernanceDirectiveFlags(true, true),
        new EdfGovernanceCorrelation(["ADR-0013", "SPEC-004"]));

    return new GovernedRelayPackage(
        GovernedPackageId.New(),
        GovernedCorrelationId.New(),
        GovernedPackageKind.PaHandoverImport,
        RelaySchemaVersion.Current,
        RelayRenderVersion.V1,
        ProjectConcordProjectId.New(),
        DateTimeOffset.UtcNow,
        DateTimeOffset.UtcNow,
        governance,
        tier0,
        bytes,
        GovernedRelayPackage.DefaultStructuralAgreement);
}

internal static class RelaySerializationFixturesThin
{
    public static GovernedRelayPackage CreateEngineeringResultImport()
    {
        var bytes = SoftwareDevelopmentProfilePayloadSerializer.Serialize(
            SoftwareDevelopmentProfilePayloadSerializer.Empty);

        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode.Plan,
            null,
            null,
            new RelaySessionContinuity(
                ProjectArchitectSessionIntent: AgentSessionIntent.Continue,
                ProjectArchitectSessionAdvisory: AgentSessionAdvisory.None,
                EngineeringAgentSessionIntent: AgentSessionIntent.New,
                EngineeringAgentSessionAdvisory: AgentSessionAdvisory.None),
            RelayStopMetadata.None,
            false,
            false,
            new RelayGovernanceDirectiveFlags(false, false),
            null);

        return new GovernedRelayPackage(
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            GovernedPackageKind.EngineeringResultImport,
            RelaySchemaVersion.Current,
            RelayRenderVersion.V1,
            ProjectConcordProjectId.New(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            governance,
            Tier0RelaySnapshot.Empty,
            bytes,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }
}
