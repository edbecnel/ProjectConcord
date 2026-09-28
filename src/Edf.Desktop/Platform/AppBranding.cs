using Avalonia.Controls;
using Avalonia.Platform;

namespace Edf.Desktop.Platform;

internal static class AppBranding
{
    public const string IconFileName = "ProjectConcord-Icon-WhiteBackground.png";
    public const string IconResourcePath = "avares://Edf.Desktop/Assets/ProjectConcord-Icon-WhiteBackground.png";

    public static byte[] LoadIconBytes()
    {
        var outputPath = Path.Combine(AppContext.BaseDirectory, "Assets", IconFileName);
        if (File.Exists(outputPath))
        {
            return File.ReadAllBytes(outputPath);
        }

        using var resource = AssetLoader.Open(new Uri(IconResourcePath));
        using var memory = new MemoryStream();
        resource.CopyTo(memory);
        return memory.ToArray();
    }

    public static void ApplyDockAndWindowIcon(Window mainWindow)
    {
        var iconBytes = LoadIconBytes();
        if (OperatingSystem.IsMacOS())
        {
            MacOSDockIcon.TrySetFromPngBytes(iconBytes);
        }

        using var iconStream = new MemoryStream(iconBytes);
        mainWindow.Icon = new WindowIcon(iconStream);
    }
}
