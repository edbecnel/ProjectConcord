namespace Edf.Application.Projects.InMemory;

using Edf.Application.Relay;

public sealed class InMemoryUserApplicationStatePersistence : IUserApplicationStatePersistence
{
    private readonly object _sync = new();

    public InMemoryUserApplicationStatePersistence()
        : this(new InMemoryProjectRegistry(), new InMemoryUserPreferencesStore(), new InMemoryRelayOperationalStore())
    {
    }

    public InMemoryUserApplicationStatePersistence(
        InMemoryProjectRegistry projectRegistry,
        InMemoryUserPreferencesStore userPreferences)
        : this(projectRegistry, userPreferences, new InMemoryRelayOperationalStore())
    {
    }

    public InMemoryUserApplicationStatePersistence(
        InMemoryProjectRegistry projectRegistry,
        InMemoryUserPreferencesStore userPreferences,
        InMemoryRelayOperationalStore relayOperational)
    {
        ProjectRegistry = projectRegistry ?? throw new ArgumentNullException(nameof(projectRegistry));
        UserPreferences = userPreferences ?? throw new ArgumentNullException(nameof(userPreferences));
        RelayOperational = relayOperational ?? throw new ArgumentNullException(nameof(relayOperational));
    }

    public IProjectRegistry ProjectRegistry { get; }

    public IUserPreferencesStore UserPreferences { get; }

    public IRelayOperationalStore RelayOperational { get; }

    public void ExecuteInTransaction(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        lock (_sync)
        {
            work();
        }
    }
}
