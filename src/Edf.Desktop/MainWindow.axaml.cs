using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Platform.Storage;
using Edf.Application.Composition;
using Edf.Desktop.ViewModels;

namespace Edf.Desktop;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    public MainWindow()
    {
        InitializeComponent();
        var services = ApplicationCompositionRoot.CreateDefaultDesktopServices();
        DataContext = new MainWindowViewModel(
            services.Workspace,
            services.RelayWorkflow,
            PickFolderAsync,
            CopyTextToClipboardAsync);
    }

    private void FileOpenProjectFolder_OnClick(object? sender, EventArgs e)
    {
        _ = ViewModel.ExecuteOpenProjectFolderAsync();
    }

    private void FileGoToFolder_OnClick(object? sender, EventArgs e)
    {
        _ = PromptGoToFolderAsync();
    }

    private async Task PromptGoToFolderAsync()
    {
        var path = await PromptForFolderPathAsync().ConfigureAwait(true);
        if (path is null)
        {
            return;
        }

        ViewModel.OpenProjectRootAtPath(path);
    }

    private async Task<string?> PromptForFolderPathAsync()
    {
        var pathBox = new TextBox
        {
            PlaceholderText = "Paste absolute path to Project Root folder",
            MinWidth = 480,
        };

        string? result = null;
        var dialog = new Window
        {
            Title = "Go to Folder",
            Width = 520,
            Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
        };

        var cancelButton = new Button { Content = "Cancel" };
        var openButton = new Button { Content = "Open" };
        cancelButton.Click += (_, _) => dialog.Close();
        openButton.Click += (_, _) =>
        {
            result = pathBox.Text?.Trim();
            dialog.Close();
        };

        dialog.Content = new StackPanel
        {
            Margin = new Thickness(16),
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = "Enter the absolute path to a Project Root folder:",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                },
                pathBox,
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Spacing = 8,
                    Children = { cancelButton, openButton },
                },
            },
        };

        await dialog.ShowDialog(this).ConfigureAwait(true);
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    private async Task CopyTextToClipboardAsync(string text)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        await clipboard.SetTextAsync(text).ConfigureAwait(true);
    }

    private async Task<string?> PickFolderAsync(string title, string? suggestedStartPath)
    {
        IStorageFolder? suggestedStart = null;
        if (!string.IsNullOrWhiteSpace(suggestedStartPath))
        {
            suggestedStart = await StorageProvider.TryGetFolderFromPathAsync(suggestedStartPath).ConfigureAwait(true);
        }

        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            SuggestedStartLocation = suggestedStart,
        }).ConfigureAwait(true);

        var folder = folders.FirstOrDefault();
        return folder?.TryGetLocalPath();
    }
}
