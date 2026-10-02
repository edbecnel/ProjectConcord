using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;
using Edf.Application.Relay.EngineeringAgent.Providers.Cursor.Models;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

public class CursorAcpPromptPumpTests
{
    private const string TestRoot = "/tmp/projectconcord-acp-pump-root";

    [Fact]
    public async Task RunPromptAsync_UpdatesBeforePromptResponse_ArePreservedAndEndTurnCompletes()
    {
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptLiveCursorOrderScript());
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(5));
        await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        var result = await client.RunPromptAsync(
            "plan",
            "handover",
            TestRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            existingSessionId: null,
            CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Contains("engineering-result", result.ResultText, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunPromptAsync_ServerPermissionRequest_DoesNotDeadlock_DefaultDeny()
    {
        var script = new List<string>(CursorAcpFixtures.PromptSuccessScript().Take(5))
        {
            """{"jsonrpc":"2.0","id":99,"method":"session/request_permission","params":{"requestId":"perm-1","permission":"tool"}}""",
            """{"jsonrpc":"2.0","id":5,"result":{"stopReason":"end_turn"}}""",
        };
        var transport = new ScriptedCursorAcpTransport(script);
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(5));
        await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        var result = await client.RunPromptAsync(
            "agent",
            "x",
            TestRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);
        Assert.True(result.Succeeded);
        var written = string.Join('\n', transport.WrittenLines);
        Assert.Contains("\"id\":99", written, StringComparison.Ordinal);
        Assert.Contains("deny", written, StringComparison.Ordinal);
    }

    [Fact]
    public async Task RunPromptAsync_AllowOncePolicy_WritesAllowOncePermissionResponse()
    {
        var script = new List<string>(CursorAcpFixtures.PromptSuccessScript().Take(5))
        {
            """{"jsonrpc":"2.0","id":99,"method":"session/request_permission","params":{"requestId":"perm-1","permission":"tool"}}""",
            """{"jsonrpc":"2.0","id":5,"result":{"stopReason":"end_turn"}}""",
        };
        var transport = new ScriptedCursorAcpTransport(script);
        var policy = new CursorAcpPermissionPolicy
        {
            Evaluate = _ => new EngineeringAgentProviderPermissionDecision(
                EngineeringAgentProviderPermissionDisposition.AllowOnce),
        };
        var client = new CursorAcpClient(transport, policy, TimeSpan.FromSeconds(5));
        await client.InitializeAndAuthenticateAsync(CancellationToken.None);

        _ = await client.RunPromptAsync(
            "agent",
            "x",
            TestRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);
        Assert.Contains("allow-once", string.Join('\n', transport.WrittenLines), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Forward_UsesGovernedProjectRoot_InSessionNewCwd()
    {
        var root = "/tmp/projectconcord-governed-root-xyz";
        var transport = new ScriptedCursorAcpTransport(CursorAcpFixtures.PromptSuccessScript());
        var plugin = new CursorEngineeringAgentProviderPlugin(() => transport);
        await plugin.InitializeAsync();

        var request = new EngineeringAgentForwardRequest(
            ProjectConcordProjectId.New(),
            TransportOperationId.New(),
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            EngineeringAgentMode.Agent,
            "body",
            RelayTestFixtures.SessionWithBothIntents(),
            null,
            ProjectLocator.FromPath(root));
        _ = plugin.Forward(request);

        var sessionNew = transport.WrittenLines.First(l => l.Contains("session/new", StringComparison.Ordinal));
        Assert.Contains(root, sessionNew, StringComparison.Ordinal);
        Assert.DoesNotContain(Environment.CurrentDirectory, sessionNew, StringComparison.Ordinal);
    }

    [Trait("RequiresCursor", "true")]
    [Fact]
    public async Task LiveCursorAcp_RunPrompt_CompletesOnEndTurn()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("PROJECTCONCORD_RUN_LIVE_CURSOR"),
                "1",
                StringComparison.Ordinal))
        {
            return;
        }

        var inner = new CursorAcpSubprocessTransport();
        var transport = new RecordingCursorAcpTransport(inner);
        var client = new CursorAcpClient(transport, ioTimeout: TimeSpan.FromSeconds(120));
        var init = await client.InitializeAndAuthenticateAsync(CancellationToken.None);
        if (init.Failure?.Kind == EngineeringAgentProviderFailureKind.InitializationFailed)
        {
            Assert.Fail(init.Failure.Message);
        }

        if (init.Failure?.Kind == EngineeringAgentProviderFailureKind.AuthenticationUnavailable)
        {
            await client.DisposeAsync();
            return;
        }

        var mvrRoot = "/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A";
        var result = await client.RunPromptAsync(
            "agent",
            "Reply with exactly: PROJECTCONCORD_ACP_OK",
            mvrRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);

        Assert.True(result.Succeeded, result.Failure?.Message);
        var sessionNew = transport.WrittenLines.First(l => l.Contains("session/new", StringComparison.Ordinal));
        Assert.Contains(mvrRoot, sessionNew, StringComparison.Ordinal);
        await client.DisposeAsync();
    }

    [Trait("RequiresCursor", "true")]
    [Fact]
    public async Task LiveCursorAcp_ComposedExecutionPrompt_AcpCompletes_AndParseReportsValidation()
    {
        if (!string.Equals(
                Environment.GetEnvironmentVariable("PROJECTCONCORD_RUN_LIVE_CURSOR"),
                "1",
                StringComparison.Ordinal))
        {
            return;
        }

        var bridge = new EngineeringAgentManualRelayBridge();
        var pa = new ProjectArchitectManualAdapter();
        var handover = RelaySerializationFixtures.ValidImplementationHandover();
        var paRendered = pa.RenderPaReviewPackage(handover);
        var imported = pa.TryParsePaHandoverImport(paRendered);
        Assert.True(imported.Validation.IsEligibleForValidatedEngineeringAgentHandover);
        var prepared = bridge.TryRenderValidatedHandover(imported.Package!, imported.Validation);
        var composed = bridge.ComposeAutomatedExecutionPrompt(
            prepared.RenderedHandover!,
            prepared.ExportPackage!);
        var prompt = composed.TrimEnd()
            + "\n\n"
            + "Engineering task (minimal): Confirm you read this handover. Make no repository file changes. "
            + "Your final message must be ONLY the completed Engineering Result relay document per the output contract.";

        var diagnosticDir = "/Users/edbecnel/tmp/ProjectConcord-A4-ACP-Diagnostic";
        Directory.CreateDirectory(diagnosticDir);
        var bodyPath = Path.Combine(diagnosticDir, "remediation-3b-live-result.txt");

        var inner = new CursorAcpSubprocessTransport();
        var client = new CursorAcpClient(inner, ioTimeout: TimeSpan.FromSeconds(180));
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var init = await client.InitializeAndAuthenticateAsync(CancellationToken.None);
        if (init.Failure?.Kind == EngineeringAgentProviderFailureKind.AuthenticationUnavailable)
        {
            await client.DisposeAsync();
            return;
        }

        if (init.Failure is not null)
        {
            Assert.Fail(init.Failure.Message);
        }

        var mvrRoot = "/Users/edbecnel/tmp/ProjectConcord-A4-MVR/Root-A";
        var promptResult = await client.RunPromptAsync(
            "agent",
            prompt,
            mvrRoot,
            CursorEngineeringAgentModelSelection.DefaultComposer25NonFast,
            null,
            CancellationToken.None);
        sw.Stop();

        Assert.True(promptResult.Succeeded, promptResult.Failure?.Message);
        Assert.False(string.IsNullOrWhiteSpace(promptResult.ResultText));
        await File.WriteAllTextAsync(bodyPath, promptResult.ResultText!);

        var parse = bridge.TryParseEngineeringResult(promptResult.ResultText!);
        if (parse.Validation.State != RelayValidationState.Valid || parse.Package is null)
        {
            var diagnostics = string.Join(
                "; ",
                parse.Validation.Diagnostics.Select(d => $"{d.Code}: {d.Message}"));
            Assert.Fail(
                $"Remediation #3B live diagnostic failed after {sw.Elapsed.TotalSeconds:F1}s. "
                + $"State={parse.Validation.State}. Diagnostics={diagnostics}. Body saved to {bodyPath}.");
        }

        Assert.Equal(GovernedPackageKind.EngineeringResultImport, parse.Package.Kind);
        await client.DisposeAsync();
    }
}
