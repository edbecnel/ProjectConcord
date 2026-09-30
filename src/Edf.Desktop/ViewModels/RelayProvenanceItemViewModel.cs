namespace Edf.Desktop.ViewModels;

public sealed class RelayProvenanceItemViewModel
{
    public RelayProvenanceItemViewModel(string summary, string recordedUtcLabel)
    {
        Summary = summary;
        RecordedUtcLabel = recordedUtcLabel;
    }

    public string Summary { get; }

    public string RecordedUtcLabel { get; }

    public string DisplayLine => $"{RecordedUtcLabel} — {Summary}";
}
