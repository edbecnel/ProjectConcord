using Edf.Application.Composition;
using Edf.Application.Operator;
using Edf.Application.Operator.Relay;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Tests.Relay;
using Edf.Application.Tests.Relay.EngineeringAgent;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Operator;

public class EngineeringAgentA4T5OperatorProjectionTests
{
    [Fact]
    public void NoProviderSelected_EmitsPluginUnavailableAttention()
    {
        var projectId = ProjectConcordProjectId.New();
        var service = CreateProjectionService(EngineeringAgentPluginCatalog.CreateEmpty());

        var projection = service.Project(new RelayWorkflowOperatorProjectionInput(
            projectId,
            EngineeringAgentMode.Plan,
            null,
            null,
            null));

        Assert.Contains(
            projection.AttentionItems,
            a => a.Code == OperatorAttentionCodes.EngineeringAgentPluginUnavailable);
    }

    [Fact]
    public void EligiblePaHandover_WithPluginUnavailable_RecommendsManualP0()
    {
        var projectId = ProjectConcordProjectId.New();
        var (package, validation) = CreateEligibleImport(projectId);
        var service = CreateProjectionService(EngineeringAgentPluginCatalog.CreateEmpty());

        var projection = service.Project(new RelayWorkflowOperatorProjectionInput(
            projectId,
            EngineeringAgentMode.Plan,
            package,
            validation,
            null));

        var recommended = projection.NextActions.Single(a => a.ActionClass == OperatorNextActionClass.Recommended);
        Assert.Equal(OperatorNextActionCodes.GovernedRelayManualP0, recommended.Code);
    }

    [Fact]
    public void ForwardFailure_EmitsForwardAttention_DoesNotMutateGovernanceValidation()
    {
        var projectId = ProjectConcordProjectId.New();
        var (package, validation) = CreateEligibleImport(projectId);
        var transportResult = new EngineeringAgentAutomatedTransportResult(
            EngineeringAgentAutomatedTransportOutcome.ForwardFailed,
            null,
            "forward failed",
            validation,
            new EngineeringAgentProviderFailure(
                EngineeringAgentProviderFailureKind.ForwardFailed,
                "forward failed"));

        var service = CreateProjectionService(EngineeringAgentPluginCatalog.CreateEmpty());
        var projection = service.Project(new RelayWorkflowOperatorProjectionInput(
            projectId,
            EngineeringAgentMode.Plan,
            package,
            validation,
            transportResult));

        Assert.Contains(
            projection.AttentionItems,
            a => a.Code == OperatorAttentionCodes.EngineeringAgentForwardFailure);
        Assert.Equal(RelayValidationState.Valid, validation.State);
        Assert.True(validation.IsEligibleForValidatedEngineeringAgentHandover);
    }

    [Fact]
    public async Task AuthenticationFailure_EmitsAuthenticationAttention()
    {
        var projectId = ProjectConcordProjectId.New();
        var fake = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("fake.t5"),
            reportsAuthenticatedWhenInitialized: false);
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty().Register(fake);
        var persistence = new InMemoryUserApplicationStatePersistence();
        var preferences = new EngineeringAgentPluginProjectPreferencesStore();
        preferences.SetPluginEnabled(projectId, fake.PluginId, enabled: true);
        preferences.SetSelectedPlugin(projectId, fake.PluginId);
        var host = new EngineeringAgentPluginHost(catalog);
        await host.InitializePluginAsync(fake.PluginId);
        var selection = new EngineeringAgentPluginSelectionService(catalog, host, preferences);
        var service = new RelayWorkflowOperatorProjectionService(
            new EngineeringAgentTransportOperatorAttentionContributor(selection, persistence.TransportOperations));

        var projection = service.Project(new RelayWorkflowOperatorProjectionInput(
            projectId,
            EngineeringAgentMode.Plan,
            null,
            null,
            null));

        Assert.Contains(
            projection.AttentionItems,
            a => a.Code == OperatorAttentionCodes.EngineeringAgentAuthenticationFailure);
    }

    [Fact]
    public void AmbiguousTransport_EmitsAmbiguousAttention()
    {
        var projectId = ProjectConcordProjectId.New();
        var service = CreateProjectionService(EngineeringAgentPluginCatalog.CreateEmpty());
        var transportResult = new EngineeringAgentAutomatedTransportResult(
            EngineeringAgentAutomatedTransportOutcome.Ambiguous,
            null,
            "uncertain",
            null,
            null);

        var projection = service.Project(new RelayWorkflowOperatorProjectionInput(
            projectId,
            EngineeringAgentMode.Plan,
            null,
            null,
            transportResult));

        Assert.Contains(
            projection.AttentionItems,
            a => a.Code == OperatorAttentionCodes.EngineeringAgentAmbiguousTransport);
    }

    [Fact]
    public void ProductionComposition_HasZeroProviders_AndProjectionService()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        Assert.Empty(services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds);
        Assert.NotNull(services.RelayOperatorProjections);
    }

    private static RelayWorkflowOperatorProjectionService CreateProjectionService(
        EngineeringAgentPluginCatalog catalog,
        ProjectConcordProjectId? projectId = null,
        EngineeringAgentProviderPluginId? selectedId = null)
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var preferences = new EngineeringAgentPluginProjectPreferencesStore();
        if (projectId is not null && selectedId is not null)
        {
            preferences.SetPluginEnabled(projectId.Value, selectedId.Value, enabled: true);
            preferences.SetSelectedPlugin(projectId.Value, selectedId.Value);
        }

        var host = new EngineeringAgentPluginHost(catalog);
        var selection = new EngineeringAgentPluginSelectionService(catalog, host, preferences);
        return new RelayWorkflowOperatorProjectionService(
            new EngineeringAgentTransportOperatorAttentionContributor(selection, persistence.TransportOperations));
    }

    private static (GovernedRelayPackage Package, RelayValidationResult Validation) CreateEligibleImport(
        ProjectConcordProjectId projectId)
    {
        var pa = new ProjectArchitectManualAdapter();
        var handover = RelaySerializationFixtures.ValidImplementationHandover() with { ProjectId = projectId };
        var rendered = pa.RenderPaReviewPackage(handover);
        var imported = pa.TryParsePaHandoverImport(rendered);
        Assert.NotNull(imported.Package);
        return (imported.Package, imported.Validation);
    }
}
