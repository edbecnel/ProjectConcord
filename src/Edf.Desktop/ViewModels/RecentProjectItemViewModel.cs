using System.Windows.Input;
using Edf.Domain.Projects;

namespace Edf.Desktop.ViewModels;

public sealed class RecentProjectItemViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _host;

    public RecentProjectItemViewModel(RecentProjectEntry entry, MainWindowViewModel host)
    {
        Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        _host = host ?? throw new ArgumentNullException(nameof(host));

        OpenCommand = new AsyncRelayCommand(
            () => _host.OpenRecentProjectAsync(Entry.ProjectId),
            () => Entry.LocatorAvailability == LocatorAvailability.Available);

        RelocateCommand = new AsyncRelayCommand(
            () => _host.RelocateRecentProjectAsync(Entry.ProjectId),
            () => CanRelocateProject(Entry.LocatorAvailability));

        RemoveFromRecentCommand = new RelayCommand(
            () => _host.RemoveRecentProject(Entry.ProjectId));

        CopyLocatorPathCommand = new AsyncRelayCommand(
            () => _host.CopyLocatorPathAsync(LocatorPath));
    }

    public RecentProjectEntry Entry { get; }

    public string DisplayName => Entry.DisplayName;

    public string LocatorPath => Entry.RegisteredLocator.NormalizedAbsolutePath;

    public string AvailabilityLabel => Entry.LocatorAvailability switch
    {
        LocatorAvailability.Available => "Available",
        LocatorAvailability.MissingOnDisk => "Missing on disk",
        _ => Entry.LocatorAvailability.ToString(),
    };

    public bool IsLastActive => Entry.IsLastActive;

    public bool IsMissingOnDisk => Entry.LocatorAvailability == LocatorAvailability.MissingOnDisk;

    public string LastOpenedLabel => Entry.LastOpenedUtc.ToLocalTime().ToString("g");

    public ICommand OpenCommand { get; }

    public ICommand RelocateCommand { get; }

    public ICommand RemoveFromRecentCommand { get; }

    public ICommand CopyLocatorPathCommand { get; }

    public static bool CanRelocateProject(LocatorAvailability availability) =>
        availability == LocatorAvailability.MissingOnDisk;
}
