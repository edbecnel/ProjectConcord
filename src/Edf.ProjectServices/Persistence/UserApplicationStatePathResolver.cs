namespace Edf.ProjectServices.Persistence;

public static class UserApplicationStatePathResolver
{
    public const string DatabaseFileName = "user-state.db";

    public static string ResolveDatabaseFilePath()
    {
        var baseDirectory = ResolveApplicationDataDirectory();
        Directory.CreateDirectory(baseDirectory);
        return Path.Combine(baseDirectory, DatabaseFileName);
    }

    internal static string ResolveApplicationDataDirectory()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ProjectConcord");
        }

        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProjectConcord");
    }
}
