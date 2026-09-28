using System.Windows.Input;
using Edf.Application.Projects;

namespace Edf.Desktop.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectWorkspaceService _workspace;
    private string? _projectRootPath;
    private string? _statusMessage;
    private string _actorDisplayName;

    public MainWindowViewModel(IProjectWorkspaceService workspace, Func<Task<string?>> pickProjectRootFolderAsync)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        PickProjectRootFolderAsync = pickProjectRootFolderAsync ?? throw new ArgumentNullException(nameof(pickProjectRootFolderAsync));
        _actorDisplayName = workspace.CurrentActor.DisplayName;
        OpenProjectFolderCommand = new AsyncRelayCommand(OpenProjectFolderAsync);
    }

    public ICommand OpenProjectFolderCommand { get; }

    public Func<Task<string?>> PickProjectRootFolderAsync { get; }

    public string? ProjectRootPath
    {
        get => _projectRootPath;
        private set => SetProperty(ref _projectRootPath, value);
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string ActorDisplayName
    {
        get => _actorDisplayName;
        private set => SetProperty(ref _actorDisplayName, value);
    }

    private async Task OpenProjectFolderAsync()
    {
        StatusMessage = null;
        var selectedPath = await PickProjectRootFolderAsync().ConfigureAwait(true);
        if (selectedPath is null)
        {
            StatusMessage = "Folder selection cancelled.";
            return;
        }

        var result = _workspace.OpenProjectRoot(selectedPath);
        if (result.Success && result.Root is not null)
        {
            ProjectRootPath = result.Root.AbsolutePath;
            ActorDisplayName = _workspace.CurrentActor.DisplayName;
            StatusMessage = "Project root opened.";
            return;
        }

        ProjectRootPath = null;
        StatusMessage = result.ErrorMessage ?? "Unable to open project root.";
    }
}
