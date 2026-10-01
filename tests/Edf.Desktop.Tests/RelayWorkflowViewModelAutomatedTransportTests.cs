using Edf.Application.Composition;
using Edf.Application.Operator;
using Edf.Application.Projects;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Desktop.ViewModels;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Desktop.Tests;

public class RelayWorkflowViewModelAutomatedTransportTests
{
    [Fact]
    public async Task ValidImport_WithNoProvider_ShowsAttention_And_RecommendedManual_And_ManualPrepareStillWorks()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var (vm, projectId) = CreateRelayViewModel(services);

        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        vm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        var package = RelaySerializationFixtures.ValidImplementationHandover() with { ProjectId = projectId };
        vm.PaImportText = new ProjectArchitectManualAdapterShim().Render(package);
        ((AsyncRelayCommand)vm.ImportPaHandoverCommand).Execute(null);

        Assert.True(vm.HasAutomatedTransportIntegration);
        Assert.Contains(
            vm.AttentionItems,
            a => a.Code == OperatorAttentionCodes.EngineeringAgentPluginUnavailable);
        Assert.True(vm.HasRecommendedManualRelay);
        Assert.True(vm.CanPrepareEngineeringHandover);

        ((AsyncRelayCommand)vm.PrepareEngineeringHandoverCommand).Execute(null);
        await Task.Delay(50);
        Assert.True(vm.HasEngineeringHandoverRendered);
    }

    [Fact]
    public async Task AutomatedForward_WithoutProvider_UpdatesStatus_ManualPathUnchanged()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        var (vm, projectId) = CreateRelayViewModel(services);
        vm.OnActiveProjectChanged(projectId, hasActiveProject: true);
        vm.ProjectArchitectSessionIntent = AgentSessionIntent.New;
        vm.EngineeringAgentSessionIntent = AgentSessionIntent.Continue;

        var package = RelaySerializationFixtures.ValidImplementationHandover() with { ProjectId = projectId };
        vm.PaImportText = new ProjectArchitectManualAdapterShim().Render(package);
        ((AsyncRelayCommand)vm.ImportPaHandoverCommand).Execute(null);
        var validationBefore = vm.PaImportValidationSummary;

        ((AsyncRelayCommand)vm.ForwardAutomatedHandoverCommand).Execute(null);
        await Task.Delay(100);

        Assert.Contains("ProviderUnavailable", vm.AutomatedTransportStatus ?? vm.RelayStatusMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(validationBefore, vm.PaImportValidationSummary);
        Assert.True(vm.CanPrepareEngineeringHandover);
    }

    private static (RelayWorkflowViewModel Vm, ProjectConcordProjectId ProjectId) CreateRelayViewModel(
        DesktopApplicationServices services)
    {
        var actor = new DegenerateAdministratorActor();
        var resolver = new ProjectRootResolver();
        var runtime = new LocalProjectRuntime();
        var workspace = services.Workspace;

        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-t5-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);

        var vm = new RelayWorkflowViewModel(
            services.RelayWorkflow,
            workspace,
            _ => Task.CompletedTask,
            services.AutomatedTransport,
            services.RelayOperatorProjections);

        return (vm, open.ProjectId!.Value);
    }

    private sealed class ProjectArchitectManualAdapterShim
    {
        private readonly Edf.Application.Relay.ProjectArchitect.ProjectArchitectManualAdapter _inner = new();

        public string Render(GovernedRelayPackage package) => _inner.RenderPaReviewPackage(package);
    }
}
