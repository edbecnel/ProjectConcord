namespace Edf.Application.Projects;

/// <summary>
/// Coordinates project registry and user preferences persistence (A1b: SQLite; A1a: in-memory).
/// </summary>
public interface IUserApplicationStatePersistence
{
    IProjectRegistry ProjectRegistry { get; }

    IUserPreferencesStore UserPreferences { get; }

    void ExecuteInTransaction(Action work);
}
