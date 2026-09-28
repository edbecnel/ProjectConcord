using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Edf.Application.Composition;
using Edf.Desktop.ViewModels;

namespace Edf.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var workspace = ApplicationCompositionRoot.CreateDefaultWorkspaceService();
        DataContext = new MainWindowViewModel(workspace, PickProjectRootFolderAsync);
    }

    private async Task<string?> PickProjectRootFolderAsync()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select project root folder",
            AllowMultiple = false,
        }).ConfigureAwait(true);

        var folder = folders.FirstOrDefault();
        return folder?.TryGetLocalPath();
    }
}
