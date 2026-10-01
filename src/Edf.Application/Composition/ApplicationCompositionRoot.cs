using Edf.Application.Operator.Relay;
using Edf.Application.Projects;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Hosting;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Composition;

public sealed record DesktopApplicationServices(
    IProjectWorkspaceService Workspace,
    IGovernedRelayP0WorkflowService RelayWorkflow,
    ITransportOperationStore TransportOperations,
    EngineeringAgentPluginHostingServices EngineeringAgentPluginHosting,
    IEngineeringAgentAutomatedTransportService AutomatedTransport,
    IEngineeringAgentTransportRecoveryService TransportRecovery,
    RelayWorkflowOperatorProjectionService RelayOperatorProjections);

public static class ApplicationCompositionRoot
{
    public static DesktopApplicationServices CreateDefaultDesktopServices()
    {
        var persistence = UserApplicationStatePersistenceFactory.CreateDefaultSqlite();
        return CreateDesktopServices(persistence);
    }

    public static DesktopApplicationServices CreateInMemoryDesktopServices()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        return CreateDesktopServices(persistence);
    }

    public static IProjectWorkspaceService CreateDefaultWorkspaceService() =>
        CreateDefaultDesktopServices().Workspace;

    public static IProjectWorkspaceService CreateInMemoryWorkspaceService() =>
        CreateInMemoryDesktopServices().Workspace;

    private static DesktopApplicationServices CreateDesktopServices(IUserApplicationStatePersistence persistence)
    {
        var actor = new DegenerateAdministratorActor();
        var resolver = new ProjectRootResolver();
        var runtime = new LocalProjectRuntime();
        var workspace = new ProjectWorkspaceService(resolver, actor, persistence, runtime);
        var relayWorkflow = GovernedRelayP0WorkflowService.Create(persistence);
        var pluginHosting = EngineeringAgentPluginHostingFactory.Create(persistence);
        var automatedTransport = new EngineeringAgentTransportOrchestrator(
            persistence,
            relayWorkflow,
            pluginHosting.Catalog,
            pluginHosting.Host,
            pluginHosting.Selection);
        var transportRecovery = new EngineeringAgentTransportRecoveryService(
            persistence,
            relayWorkflow,
            pluginHosting.Catalog,
            pluginHosting.Host,
            pluginHosting.Preferences);
        var relayOperatorProjections = new RelayWorkflowOperatorProjectionService(
            new EngineeringAgentTransportOperatorAttentionContributor(
                pluginHosting.Selection,
                persistence.TransportOperations));
        return new DesktopApplicationServices(
            workspace,
            relayWorkflow,
            persistence.TransportOperations,
            pluginHosting,
            automatedTransport,
            transportRecovery,
            relayOperatorProjections);
    }
}
