using System.Collections.ObjectModel;
using System.Windows.Input;
using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Domain.Projects;

namespace Edf.Desktop.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IProjectWorkspaceService _workspace;
    private readonly Func<string, string?, Task<string?>> _pickFolderAsync;
    private readonly Func<string, Task> _copyTextAsync;
    private string? _projectRootPath;
    private string? _currentProjectId;
    private string? _statusMessage;
    private string _actorDisplayName;
    private bool _hasActiveProject;

    public MainWindowViewModel(
        IProjectWorkspaceService workspace,
        Func<string, string?, Task<string?>> pickFolderAsync,
        Func<string, Task>? copyTextAsync = null)
        : this(workspace, null, pickFolderAsync, copyTextAsync)
    {
    }

    public MainWindowViewModel(
        IProjectWorkspaceService workspace,
        IGovernedRelayP0WorkflowService? relayWorkflow,
        Func<string, string?, Task<string?>> pickFolderAsync,
        Func<string, Task>? copyTextAsync = null)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _pickFolderAsync = pickFolderAsync ?? throw new ArgumentNullException(nameof(pickFolderAsync));
        _copyTextAsync = copyTextAsync ?? (_ => Task.CompletedTask);
        _actorDisplayName = workspace.CurrentActor.DisplayName;

        RecentProjects = new ObservableCollection<RecentProjectItemViewModel>();
        OpenProjectFolderCommand = new AsyncRelayCommand(OpenProjectFolderAsync);
        CloseProjectCommand = new RelayCommand(CloseProject, () => HasActiveProject);
        CopyActivePathCommand = new AsyncRelayCommand(CopyActivePathAsync, () => HasActiveProject && !string.IsNullOrWhiteSpace(ProjectRootPath));
        CopyProjectIdCommand = new AsyncRelayCommand(CopyProjectIdAsync, () => HasActiveProject && !string.IsNullOrWhiteSpace(CurrentProjectId));

        Relay = relayWorkflow is null
            ? null
            : new RelayWorkflowViewModel(relayWorkflow, workspace, _copyTextAsync);

        InitializeFromWorkspace();
    }

    public RelayWorkflowViewModel? Relay { get; }

    public ObservableCollection<RecentProjectItemViewModel> RecentProjects { get; }

    public ICommand OpenProjectFolderCommand { get; }

    public ICommand CloseProjectCommand { get; }

    public ICommand CopyActivePathCommand { get; }

    public ICommand CopyProjectIdCommand { get; }

    public bool HasRecentProjects => RecentProjects.Count > 0;

    public string RecentEmptyMessage => "No recent projects. Use Open project folder… to register a Project Root.";

    public string? ProjectRootPath
    {
        get => _projectRootPath;
        private set
        {
            if (SetProperty(ref _projectRootPath, value))
            {
                (CopyActivePathCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(ActiveProjectSummary));
            }
        }
    }

    public string? CurrentProjectId
    {
        get => _currentProjectId;
        private set
        {
            if (SetProperty(ref _currentProjectId, value))
            {
                (CopyProjectIdCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(ActiveProjectSummary));
            }
        }
    }

    public string ActiveProjectSummary => HasActiveProject
        ? $"Active: {ProjectRootPath} (Project ID: {CurrentProjectId})"
        : "No project open.";

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

    public bool HasActiveProject
    {
        get => _hasActiveProject;
        private set
        {
            if (SetProperty(ref _hasActiveProject, value))
            {
                (CloseProjectCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CopyActivePathCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                (CopyProjectIdCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(ActiveProjectSummary));
            }
        }
    }

    internal void InitializeFromWorkspace()
    {
        RefreshActiveSessionFromWorkspace();
        RefreshRecentProjects();
    }

    internal Task OpenRecentProjectAsync(ProjectConcordProjectId projectId)
    {
        StatusMessage = null;
        var result = _workspace.OpenProjectById(projectId);
        if (result.Success && result.Root is not null)
        {
            ApplySuccessfulOpen(result);
            StatusMessage = "Project reopened from Recent.";
            return Task.CompletedTask;
        }

        StatusMessage = result.ErrorMessage ?? "Unable to reopen project.";
        return Task.CompletedTask;
    }

    internal async Task RelocateRecentProjectAsync(ProjectConcordProjectId projectId)
    {
        StatusMessage = null;
        var entry = _workspace.ListRecentProjects().FirstOrDefault(e => e.ProjectId.Value == projectId.Value);
        if (entry is null)
        {
            StatusMessage = "Project was not found in Recent.";
            RefreshRecentProjects();
            return;
        }

        if (!RecentProjectItemViewModel.CanRelocateProject(entry.LocatorAvailability))
        {
            StatusMessage = "Relocate Project is only available when the registered locator is missing on disk.";
            return;
        }

        var selectedPath = await _pickFolderAsync(
            "Relocate project — select new Project Root folder",
            SuggestPickerStartPath(entry.RegisteredLocator.NormalizedAbsolutePath)).ConfigureAwait(true);
        if (selectedPath is null)
        {
            StatusMessage = "Folder selection cancelled.";
            return;
        }

        var result = _workspace.ReconcileProjectLocator(projectId, selectedPath);
        if (!result.Success || result.Project is null)
        {
            StatusMessage = result.ErrorMessage ?? "Unable to relocate project.";
            RefreshRecentProjects();
            return;
        }

        RefreshRecentProjects();
        StatusMessage = $"Project relocated. Project ID unchanged: {projectId}.";
    }

    internal void RemoveRecentProject(ProjectConcordProjectId projectId)
    {
        StatusMessage = null;
        _workspace.RemoveFromRecent(projectId);
        RefreshRecentProjects();
        StatusMessage = "Removed from Recent. Project registration retained.";
    }

    internal Task ExecuteOpenProjectFolderAsync() => OpenProjectFolderAsync();

    internal void OpenProjectRootAtPath(string absolutePath)
    {
        StatusMessage = null;
        if (string.IsNullOrWhiteSpace(absolutePath))
        {
            StatusMessage = "Enter a folder path.";
            return;
        }

        var normalized = absolutePath.Trim();
        if (!Directory.Exists(normalized))
        {
            StatusMessage = "Folder not found on disk.";
            return;
        }

        var result = _workspace.OpenProjectRoot(normalized);
        if (result.Success && result.Root is not null)
        {
            ApplySuccessfulOpen(result);
            StatusMessage = "Project root opened.";
            return;
        }

        StatusMessage = result.ErrorMessage ?? "Unable to open project root.";
    }

    internal async Task CopyLocatorPathAsync(string locatorPath)
    {
        if (string.IsNullOrWhiteSpace(locatorPath))
        {
            return;
        }

        await _copyTextAsync(locatorPath).ConfigureAwait(true);
        StatusMessage = "Locator path copied to clipboard.";
    }

    private async Task CopyActivePathAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectRootPath))
        {
            return;
        }

        await _copyTextAsync(ProjectRootPath).ConfigureAwait(true);
        StatusMessage = "Active project path copied to clipboard.";
    }

    private async Task CopyProjectIdAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentProjectId))
        {
            return;
        }

        await _copyTextAsync(CurrentProjectId).ConfigureAwait(true);
        StatusMessage = "Project ID copied to clipboard.";
    }

    private static string? SuggestPickerStartPath(string? registeredPath)
    {
        if (string.IsNullOrWhiteSpace(registeredPath))
        {
            return null;
        }

        if (Directory.Exists(registeredPath))
        {
            return registeredPath;
        }

        var parent = Path.GetDirectoryName(registeredPath);
        return parent is not null && Directory.Exists(parent) ? parent : null;
    }

    private async Task OpenProjectFolderAsync()
    {
        StatusMessage = null;
        var suggestedStart = HasActiveProject ? SuggestPickerStartPath(ProjectRootPath) : null;
        var selectedPath = await _pickFolderAsync("Select project root folder", suggestedStart).ConfigureAwait(true);
        if (selectedPath is null)
        {
            StatusMessage = "Folder selection cancelled.";
            return;
        }

        OpenProjectRootAtPath(selectedPath);
    }

    private void CloseProject()
    {
        StatusMessage = null;
        _workspace.CloseProject();
        RefreshActiveSessionFromWorkspace();
        StatusMessage = "Project closed. Recent projects unchanged.";
    }

    private void ApplySuccessfulOpen(OpenProjectResult result)
    {
        ProjectRootPath = result.Root!.AbsolutePath;
        CurrentProjectId = result.ProjectId?.ToString();
        ActorDisplayName = _workspace.CurrentActor.DisplayName;
        HasActiveProject = true;
        RefreshRecentProjects();
        NotifyRelayProjectChanged(result.ProjectId);
    }

    private void RefreshActiveSessionFromWorkspace()
    {
        if (_workspace.CurrentRoot is { } root && _workspace.CurrentProjectId is { } id)
        {
            ProjectRootPath = root.AbsolutePath;
            CurrentProjectId = id.ToString();
            HasActiveProject = true;
            NotifyRelayProjectChanged(id);
            return;
        }

        ProjectRootPath = null;
        CurrentProjectId = null;
        HasActiveProject = false;
        NotifyRelayProjectChanged(null);
    }

    private void NotifyRelayProjectChanged(ProjectConcordProjectId? projectId)
    {
        Relay?.OnActiveProjectChanged(projectId, HasActiveProject);
    }

    private void RefreshRecentProjects()
    {
        RecentProjects.Clear();
        foreach (var entry in _workspace.ListRecentProjects())
        {
            RecentProjects.Add(new RecentProjectItemViewModel(entry, this));
        }

        RaisePropertyChanged(nameof(HasRecentProjects));
        RaisePropertyChanged(nameof(RecentEmptyMessage));
    }
}
