using Edf.Application.Projects;
using Edf.Application.Projects.Sqlite;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Composition;

public static class UserApplicationStatePersistenceFactory
{
    public static IUserApplicationStatePersistence CreateDefaultSqlite()
    {
        var databasePath = UserApplicationStatePathResolver.ResolveDatabaseFilePath();
        var store = new SqliteUserApplicationStateStore(databasePath);
        return new SqliteUserApplicationStatePersistence(store);
    }

    public static IUserApplicationStatePersistence CreateSqliteAtPath(string databaseFilePath)
    {
        var store = new SqliteUserApplicationStateStore(databaseFilePath);
        return new SqliteUserApplicationStatePersistence(store);
    }
}
