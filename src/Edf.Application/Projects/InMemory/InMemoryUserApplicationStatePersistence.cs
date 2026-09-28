namespace Edf.Application.Projects.InMemory;

public sealed class InMemoryUserApplicationStatePersistence : IUserApplicationStatePersistence
{
    private readonly object _sync = new();

    public InMemoryUserApplicationStatePersistence()
        : this(new InMemoryProjectRegistry(), new InMemoryUserPreferencesStore())
    {
    }

    public InMemoryUserApplicationStatePersistence(
        InMemoryProjectRegistry projectRegistry,
        InMemoryUserPreferencesStore userPreferences)
    {
        ProjectRegistry = projectRegistry ?? throw new ArgumentNullException(nameof(projectRegistry));
        UserPreferences = userPreferences ?? throw new ArgumentNullException(nameof(userPreferences));
    }

    public IProjectRegistry ProjectRegistry { get; }

    public IUserPreferencesStore UserPreferences { get; }

    public void ExecuteInTransaction(Action work)
    {
        ArgumentNullException.ThrowIfNull(work);
        lock (_sync)
        {
            work();
        }
    }
}
