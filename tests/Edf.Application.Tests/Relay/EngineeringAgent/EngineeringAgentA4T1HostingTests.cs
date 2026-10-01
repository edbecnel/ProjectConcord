using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Plugins;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay.EngineeringAgent;

public class EngineeringAgentA4T1HostingTests
{
    [Fact]
    public void EmptyCatalog_HasNoRegisteredPlugins()
    {
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty();
        Assert.Empty(catalog.RegisteredPluginIds);
        Assert.False(catalog.TryGetPlugin(EngineeringAgentProviderPluginId.Parse("any"), out _));
    }

    [Fact]
    public void Catalog_RegisterAndLookup_IsDeterministic()
    {
        var beta = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("z.beta"));
        var alpha = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("a.alpha"));
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty()
            .Register(beta)
            .Register(alpha);

        Assert.Equal(
            ["a.alpha", "z.beta"],
            catalog.RegisteredPluginIds.Select(id => id.Value).ToList());

        Assert.True(catalog.TryGetPlugin(EngineeringAgentProviderPluginId.Parse("a.alpha"), out var resolved));
        Assert.Same(alpha, resolved);
    }

    [Fact]
    public void Catalog_DuplicateIdentity_Throws()
    {
        var id = EngineeringAgentProviderPluginId.Parse("dup.test");
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty()
            .Register(new FakeEngineeringAgentProviderPlugin(id));

        Assert.Throws<EngineeringAgentPluginCatalogDuplicateIdentityException>(() =>
            catalog.Register(new FakeEngineeringAgentProviderPlugin(id)));
    }

    [Fact]
    public async Task Host_Initialize_InvokesProviderLifecycle()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("lifecycle.init"));
        var host = new EngineeringAgentPluginHost(EngineeringAgentPluginCatalog.CreateEmpty().Register(plugin));

        Assert.False(plugin.GetHealth().IsInitialized);
        Assert.Equal(0, plugin.InitializeAsyncCallCount);

        var result = await host.InitializePluginAsync(plugin.PluginId);

        Assert.Equal(1, plugin.InitializeAsyncCallCount);
        Assert.True(result.Succeeded);
        Assert.True(host.IsInitialized(plugin.PluginId));
    }

    [Fact]
    public async Task Host_InitializeAndShutdown_AreIdempotent()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("host.test"));
        var host = new EngineeringAgentPluginHost(EngineeringAgentPluginCatalog.CreateEmpty().Register(plugin));

        var first = await host.InitializePluginAsync(plugin.PluginId);
        var second = await host.InitializePluginAsync(plugin.PluginId);
        Assert.True(first.Succeeded);
        Assert.True(second.Succeeded);
        Assert.Equal(1, plugin.InitializeAsyncCallCount);

        await host.ShutdownPluginAsync(plugin.PluginId);
        await host.ShutdownPluginAsync(plugin.PluginId);
        Assert.Equal(1, plugin.ShutdownAsyncCallCount);
        Assert.False(host.IsInitialized(plugin.PluginId));
    }

    [Fact]
    public async Task Host_InitializeUnknownPlugin_DoesNotInvokeProviderLifecycle()
    {
        var host = new EngineeringAgentPluginHost(EngineeringAgentPluginCatalog.CreateEmpty());
        var result = await host.InitializePluginAsync(EngineeringAgentProviderPluginId.Parse("missing"));
        Assert.False(result.Succeeded);
        Assert.False(result.Health.IsInitialized);
    }

    [Fact]
    public async Task Host_InitializeFailure_DoesNotMarkHostInitialized()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("sick"),
            initializeSucceeds: false);
        var host = new EngineeringAgentPluginHost(
            EngineeringAgentPluginCatalog.CreateEmpty().Register(plugin));

        var result = await host.InitializePluginAsync(plugin.PluginId);
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.False(host.IsInitialized(plugin.PluginId));
    }

    [Fact]
    public async Task Host_Shutdown_InvokesProviderLifecycle()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(EngineeringAgentProviderPluginId.Parse("lifecycle.shutdown"));
        var host = new EngineeringAgentPluginHost(EngineeringAgentPluginCatalog.CreateEmpty().Register(plugin));
        await host.InitializePluginAsync(plugin.PluginId);

        await host.ShutdownPluginAsync(plugin.PluginId);

        Assert.Equal(1, plugin.ShutdownAsyncCallCount);
        Assert.False(host.IsInitialized(plugin.PluginId));
    }

    [Fact]
    public async Task Host_ShutdownFailure_ClearsHostMarkerAndReturnsFailure()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("shutdown.fail"),
            shutdownSucceeds: false);
        var host = new EngineeringAgentPluginHost(EngineeringAgentPluginCatalog.CreateEmpty().Register(plugin));
        await host.InitializePluginAsync(plugin.PluginId);

        var result = await host.ShutdownPluginAsync(plugin.PluginId);

        Assert.NotNull(result.Failure);
        Assert.False(host.IsInitialized(plugin.PluginId));
    }

    [Fact]
    public void Preferences_EnablementAndSelection_AreProjectScoped()
    {
        var store = new EngineeringAgentPluginProjectPreferencesStore();
        var project = ProjectConcordProjectId.New();
        var pluginId = EngineeringAgentProviderPluginId.Parse("prefs.test");

        store.SetPluginEnabled(project, pluginId, enabled: true);
        store.SetSelectedPlugin(project, pluginId);

        var prefs = store.GetPreferences(project);
        Assert.Equal(pluginId, prefs.SelectedPluginId);
        Assert.Contains(pluginId, prefs.EnabledPluginIds);
    }

    [Fact]
    public void Selection_NoProviderSelected_IsUnavailable()
    {
        var services = CreateHostingServices(EngineeringAgentPluginCatalog.CreateEmpty());
        var evaluation = services.Selection.EvaluateForAutomatedTransport(
            ProjectConcordProjectId.New(),
            EngineeringAgentMode.Agent);

        Assert.False(evaluation.CanUseForAutomatedTransport);
        Assert.Equal(
            EngineeringAgentPluginSelectionUnavailableReason.NoProviderSelected,
            evaluation.UnavailableReason);
    }

    [Fact]
    public void Selection_DisabledProvider_IsUnavailable()
    {
        var pluginId = EngineeringAgentProviderPluginId.Parse("disabled");
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty()
            .Register(new FakeEngineeringAgentProviderPlugin(pluginId));
        var services = CreateHostingServices(catalog);
        var project = ProjectConcordProjectId.New();
        services.Preferences.SetSelectedPlugin(project, pluginId);

        var evaluation = services.Selection.EvaluateForAutomatedTransport(project, EngineeringAgentMode.Agent);
        Assert.Equal(
            EngineeringAgentPluginSelectionUnavailableReason.ProviderDisabled,
            evaluation.UnavailableReason);
    }

    [Fact]
    public async Task Selection_ValidProvider_AfterInit_Succeeds()
    {
        var pluginId = EngineeringAgentProviderPluginId.Parse("ready");
        var catalog = EngineeringAgentPluginCatalog.CreateEmpty()
            .Register(new FakeEngineeringAgentProviderPlugin(pluginId));
        var services = CreateHostingServices(catalog);
        var project = ProjectConcordProjectId.New();
        services.Preferences.SetPluginEnabled(project, pluginId, enabled: true);
        services.Preferences.SetSelectedPlugin(project, pluginId);
        await services.Host.InitializePluginAsync(pluginId);

        var evaluation = services.Selection.EvaluateForAutomatedTransport(project, EngineeringAgentMode.Plan);
        Assert.True(evaluation.CanUseForAutomatedTransport);
        Assert.NotNull(evaluation.ResolvedPlugin);
    }

    [Fact]
    public void Compatibility_DebugUnsupported_IsDetectable()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("no-debug"),
            supportsDebug: false);
        var assessment = EngineeringAgentPluginCompatibilityEvaluator.Assess(
            plugin,
            RelayRenderVersion.V1.Major,
            EngineeringAgentMode.Debug);

        Assert.False(assessment.RoutingIntent.IsCompatible);
    }

    [Fact]
    public void Compatibility_RenderMajorMismatch_IsDetectable()
    {
        var plugin = new FakeEngineeringAgentProviderPlugin(
            EngineeringAgentProviderPluginId.Parse("render"),
            renderProtocolMajor: 99);
        var assessment = EngineeringAgentPluginCompatibilityEvaluator.Assess(
            plugin,
            RelayRenderVersion.V1.Major,
            EngineeringAgentMode.Agent);

        Assert.False(assessment.RenderProtocol.IsCompatible);
    }

    [Fact]
    public void ProductionComposition_HasCursorProviderAndP0StillConstructs()
    {
        var services = ApplicationCompositionRoot.CreateInMemoryDesktopServices();
        Assert.Single(services.EngineeringAgentPluginHosting.Catalog.RegisteredPluginIds);
        Assert.NotNull(services.RelayWorkflow);
    }

    [Fact]
    public void P0Workflow_UnaffectedByCursorPluginRegistration()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var hosting = EngineeringAgentPluginHostingFactory.Create(persistence);
        var workflow = GovernedRelayP0WorkflowService.Create(persistence);

        Assert.Single(hosting.Catalog.RegisteredPluginIds);
        Assert.NotNull(workflow);
    }

    private static EngineeringAgentPluginHostingServices CreateHostingServices(EngineeringAgentPluginCatalog catalog)
    {
        var preferences = new EngineeringAgentPluginProjectPreferencesStore();
        var host = new EngineeringAgentPluginHost(catalog);
        var selection = new EngineeringAgentPluginSelectionService(catalog, host, preferences);
        return new EngineeringAgentPluginHostingServices(catalog, host, preferences, selection);
    }
}
