using Edf.Application.Composition;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Desktop.Tests;

public class RelayWorkflowViewModelTests
{
    [Fact]
    public void IncompleteImport_IsNotReadyForEngineeringHandover()
    {
        var (vm, _, projectId, root) = CreateRelayViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        vm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        var incomplete = RelaySerializationFixtures.CreateBasePackage(
            new RelayGovernanceDirectiveFlags(true, true),
            authorizationDispositionPresent: true,
            workContextPresent: true) with { ProjectId = projectId };

        vm.PaImportText = new ProjectArchitectManualAdapterShim().Render(incomplete);
        ((AsyncRelayCommand)vm.ImportPaHandoverCommand).Execute(null);

        Assert.Contains("Incomplete", vm.PaImportValidationSummary, StringComparison.Ordinal);
        Assert.False(vm.CanPrepareEngineeringHandover);
        Assert.Contains("cannot be marked ready", vm.EngineeringHandoverStatus ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        _ = root;
    }

    [Fact]
    public void ValidImport_WithActiveStop_BlocksEngineeringHandoverReadiness()
    {
        var (vm, _, projectId, _) = CreateRelayViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        vm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        var package = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            ProjectId = projectId,
            GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
            {
                Stop = new RelayStopMetadata(RelayStopState.Active, "STOP"),
            },
        };

        vm.PaImportText = new ProjectArchitectManualAdapterShim().Render(package);
        ((AsyncRelayCommand)vm.ImportPaHandoverCommand).Execute(null);

        Assert.Contains("Valid", vm.PaImportValidationSummary, StringComparison.Ordinal);
        Assert.False(vm.CanPrepareEngineeringHandover);
        Assert.Contains("STOP", vm.EngineeringHandoverStatus, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ValidImport_PreparesProjectConcordRelayV1Handover()
    {
        var (vm, _, projectId, _) = CreateRelayViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        vm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        var package = RelaySerializationFixtures.ValidImplementationHandover() with { ProjectId = projectId };
        vm.PaImportText = new ProjectArchitectManualAdapterShim().Render(package);
        ((AsyncRelayCommand)vm.ImportPaHandoverCommand).Execute(null);

        ((AsyncRelayCommand)vm.PrepareEngineeringHandoverCommand).Execute(null);
        await Task.Delay(50);

        Assert.True(vm.HasEngineeringHandoverRendered);
        Assert.True(RelayWorkflowViewModel.RenderedHandoverUsesProjectConcordRelayV1(vm.EngineeringHandoverRendered!));
        Assert.Contains(
            $"{GovernedRelayV1Format.EngineeringAgentModeField}: AGENT",
            vm.EngineeringHandoverRendered,
            StringComparison.Ordinal);
        Assert.Contains("Engineering-Agent-Chat: CONTINUE", vm.EngineeringHandoverRendered, StringComparison.Ordinal);
        Assert.Contains("ChatGPT-Chat: NEW", vm.EngineeringHandoverRendered, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CopyPaReview_CopiesOnlyRenderedGovernedOutput()
    {
        var copied = new List<string>();
        var (vm, _, projectId, root) = CreateRelayViewModel(text =>
        {
            copied.Add(text);
            return Task.CompletedTask;
        });
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        vm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        ((AsyncRelayCommand)vm.GeneratePaReviewCommand).Execute(null);
        await Task.Delay(50);
        Assert.True(vm.HasPaReviewRendered);

        ((AsyncRelayCommand)vm.CopyPaReviewCommand).Execute(null);
        await Task.Delay(50);

        Assert.Single(copied);
        Assert.Equal(vm.PaReviewRendered, copied[0]);
        _ = root;
    }

    [Fact]
    public void RejectedMalformedImport_ShowsMalformedState()
    {
        var (vm, _, projectId, _) = CreateRelayViewModel();
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.PaImportText = """
            ProjectConcord-Relay-Render: 1

            ## Governance-Critical
            Engineering-Agent-Mode: AGENT
            """;
        ((AsyncRelayCommand)vm.ImportPaHandoverCommand).Execute(null);

        Assert.Contains("RejectedMalformed", vm.PaImportValidationSummary, StringComparison.Ordinal);
        Assert.True(
            vm.PaImportValidationSummary.Contains(RelayValidationCodes.ManualPasteMachineBlockMissing, StringComparison.Ordinal)
            || vm.PaImportValidationSummary.Contains(RelayValidationCodes.ManualPasteIncomplete, StringComparison.Ordinal),
            vm.PaImportValidationSummary);
        Assert.False(vm.CanPrepareEngineeringHandover);
        Assert.NotEmpty(vm.Diagnostics);
        Assert.Contains(RelayValidationCodes.ManualPasteMachineBlockMissing, vm.Diagnostics[0], StringComparison.Ordinal);
    }

    private static (RelayWorkflowViewModel Vm, IProjectWorkspaceService Workspace, ProjectConcordProjectId ProjectId, ProjectRoot Root) CreateRelayViewModel(
        Func<string, Task>? copyAsync = null)
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var actor = new DegenerateAdministratorActor();
        var resolver = new ProjectRootResolver();
        var runtime = new LocalProjectRuntime();
        var workspace = new ProjectWorkspaceService(resolver, actor, persistence, runtime);
        var workflow = GovernedRelayP0WorkflowService.Create(persistence);

        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-t7-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);

        var vm = new RelayWorkflowViewModel(
            workflow,
            workspace,
            copyAsync ?? (_ => Task.CompletedTask));

        return (vm, workspace, open.ProjectId!.Value, open.Root!);
    }

    private sealed class ProjectArchitectManualAdapterShim
    {
        private readonly Edf.Application.Relay.ProjectArchitect.ProjectArchitectManualAdapter _inner = new();

        public string Render(GovernedRelayPackage package) => _inner.RenderPaReviewPackage(package);
    }
}

internal static class RelaySerializationFixtures
{
    public static GovernedRelayPackage ValidImplementationHandover()
    {
        var tier0 = new Tier0RelaySnapshot(
            "abc123",
            ["docs/Program/Gate_Reviews/"],
            new Dictionary<string, string> { ["gate"] = "G1" });

        return CreateBasePackage(
            directives: new RelayGovernanceDirectiveFlags(true, true),
            authorizationDispositionPresent: true,
            workContextPresent: true,
            payload: BuildImplementationPayload("A2-T7"),
            tier0: tier0,
            configureGovernance: g => g with
            {
                EngineeringAgentMode = EngineeringAgentMode.Agent,
                PriorEngineeringAgentMode = EngineeringAgentMode.Plan,
                ModeTransition = new EngineeringAgentModeTransition(
                    EngineeringAgentMode.Plan,
                    EngineeringAgentMode.Agent),
                SessionContinuity = new RelaySessionContinuity(
                    AgentSessionIntent.New,
                    AgentSessionAdvisory.None,
                    AgentSessionIntent.Continue,
                    AgentSessionAdvisory.None),
                EdfCorrelation = new EdfGovernanceCorrelation(["ADR-0013", "SPEC-004"]),
            });
    }

    public static GovernedRelayPackage CreateBasePackage(
        RelayGovernanceDirectiveFlags directives,
        bool authorizationDispositionPresent = false,
        bool workContextPresent = false,
        Edf.Application.Relay.SoftwareDevelopment.SoftwareDevelopmentProfilePayload? payload = null,
        Tier0RelaySnapshot? tier0 = null,
        Func<RelayGovernanceCriticalState, RelayGovernanceCriticalState>? configureGovernance = null)
    {
        var bytes = Edf.Application.Relay.SoftwareDevelopment.SoftwareDevelopmentProfilePayloadSerializer.Serialize(
            payload ?? Edf.Application.Relay.SoftwareDevelopment.SoftwareDevelopmentProfilePayloadSerializer.Empty);

        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode.Plan,
            null,
            null,
            new RelaySessionContinuity(AgentSessionIntent.New, AgentSessionAdvisory.None, AgentSessionIntent.Continue, AgentSessionAdvisory.None),
            RelayStopMetadata.None,
            authorizationDispositionPresent,
            workContextPresent,
            directives,
            null);

        if (configureGovernance is not null)
        {
            governance = configureGovernance(governance);
        }

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
            tier0 ?? Tier0RelaySnapshot.Empty,
            bytes,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }

    private static Edf.Application.Relay.SoftwareDevelopment.SoftwareDevelopmentProfilePayload BuildImplementationPayload(string tranche) =>
        Edf.Application.Relay.SoftwareDevelopment.SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            AuthorizationDisposition = new Edf.Application.Relay.SoftwareDevelopment.AuthorizationDispositionProjection(
                "IMPLEMENTATION AUTHORIZED",
                false,
                true,
                tranche),
            DevelopmentWorkAuthorization = new Edf.Application.Relay.SoftwareDevelopment.DevelopmentWorkAuthorizationProjection(
                Edf.Application.Relay.SoftwareDevelopment.SoftwareDevelopmentAuthorizationKind.Implementation,
                tranche,
                ["A2-T7-scope"],
                "dwa-projection-ref",
                false),
            WorkContext = new Edf.Application.Relay.SoftwareDevelopment.WorkContextProjection(tranche, "Authorized tranche work"),
        };
}
