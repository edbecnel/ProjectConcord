namespace Edf.Application.Projects.InMemory;

using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Transport;

public sealed class InMemoryUserApplicationStatePersistence : IUserApplicationStatePersistence
{
    private readonly object _sync = new();

    public InMemoryUserApplicationStatePersistence()
        : this(
            new InMemoryProjectRegistry(),
            new InMemoryUserPreferencesStore(),
            new InMemoryRelayOperationalStore(),
            null)
    {
    }

    public InMemoryUserApplicationStatePersistence(
        InMemoryProjectRegistry projectRegistry,
        InMemoryUserPreferencesStore userPreferences)
        : this(projectRegistry, userPreferences, new InMemoryRelayOperationalStore(), null)
    {
    }

    public InMemoryUserApplicationStatePersistence(
        InMemoryProjectRegistry projectRegistry,
        InMemoryUserPreferencesStore userPreferences,
        InMemoryRelayOperationalStore relayOperational)
        : this(projectRegistry, userPreferences, relayOperational, null)
    {
    }

    public InMemoryUserApplicationStatePersistence(
        InMemoryProjectRegistry projectRegistry,
        InMemoryUserPreferencesStore userPreferences,
        InMemoryRelayOperationalStore relayOperational,
        InMemoryTransportOperationStore? transportOperations,
        InMemoryWorkflowInstanceStore? workflowInstances = null,
        InMemoryWorkflowOriginStore? workflowOrigins = null,
        InMemoryWorkflowDependencyStore? workflowDependencies = null,
        InMemoryDevelopmentWorkAuthorizationStore? developmentWorkAuthorizations = null,
        InMemoryWorkflowInstanceStopStore? workflowInstanceStops = null)
    {
        ProjectRegistry = projectRegistry ?? throw new ArgumentNullException(nameof(projectRegistry));
        UserPreferences = userPreferences ?? throw new ArgumentNullException(nameof(userPreferences));
        RelayOperational = relayOperational ?? throw new ArgumentNullException(nameof(relayOperational));
        TransportOperations = transportOperations
            ?? new InMemoryTransportOperationStore(relayOperational);
        WorkflowInstances = workflowInstances ?? new InMemoryWorkflowInstanceStore();
        WorkflowOrigins = workflowOrigins ?? new InMemoryWorkflowOriginStore();
        WorkflowDependencies = workflowDependencies ?? new InMemoryWorkflowDependencyStore();
        DevelopmentWorkAuthorizations = developmentWorkAuthorizations ?? new InMemoryDevelopmentWorkAuthorizationStore();
        WorkflowInstanceStops = workflowInstanceStops ?? new InMemoryWorkflowInstanceStopStore();
    }

    public IProjectRegistry ProjectRegistry { get; }

    public IUserPreferencesStore UserPreferences { get; }

    public IRelayOperationalStore RelayOperational { get; }

    public ITransportOperationStore TransportOperations { get; }

    public IWorkflowInstanceStore WorkflowInstances { get; }

    public IWorkflowOriginStore WorkflowOrigins { get; }

    public IWorkflowDependencyStore WorkflowDependencies { get; }

    public IDevelopmentWorkAuthorizationStore DevelopmentWorkAuthorizations { get; }

    public IWorkflowInstanceStopStore WorkflowInstanceStops { get; }

    public void ExecuteInTransaction(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        lock (_sync)
        {
            work();
        }
    }
}
