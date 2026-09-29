namespace Edf.Application.Projects;

using Edf.Application.Relay;

/// <summary>
/// Coordinates project registry and user preferences persistence (A1b: SQLite; A1a: in-memory).
/// </summary>
public interface IUserApplicationStatePersistence
{
    IProjectRegistry ProjectRegistry { get; }

    IUserPreferencesStore UserPreferences { get; }

    IRelayOperationalStore RelayOperational { get; }

    void ExecuteInTransaction(Action work);
}
