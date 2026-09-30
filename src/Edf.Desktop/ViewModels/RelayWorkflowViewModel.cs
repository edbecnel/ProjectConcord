namespace Edf.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public sealed class RelayWorkflowViewModel : ViewModelBase
{
    private const int ProvenanceListLimit = 12;

    private readonly IGovernedRelayP0WorkflowService _workflow;
    private readonly IProjectWorkspaceService _workspace;
    private readonly Func<string, Task> _copyTextAsync;

    private AgentSessionIntent? _projectArchitectSessionIntent;
    private AgentSessionIntent? _engineeringAgentSessionIntent;
    private EngineeringAgentMode _engineeringAgentMode = EngineeringAgentMode.Plan;
    private EngineeringAgentMode? _priorEngineeringAgentMode;
    private string? _paReviewRendered;
    private string _paImportText = string.Empty;
    private string? _paReviewValidationSummary;
    private string? _paImportValidationSummary;
    private string? _engineeringHandoverRendered;
    private string? _engineeringHandoverStatus;
    private string _engineeringResultText = string.Empty;
    private string? _engineeringResultValidationSummary;
    private string? _relayStatusMessage;
    private GovernedRelayPackage? _lastPaHandoverImport;
    private RelayValidationResult? _lastPaHandoverValidation;
    private bool _isRelaySectionEnabled;

    public RelayWorkflowViewModel(
        IGovernedRelayP0WorkflowService workflow,
        IProjectWorkspaceService workspace,
        Func<string, Task> copyTextAsync)
    {
        _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _copyTextAsync = copyTextAsync ?? throw new ArgumentNullException(nameof(copyTextAsync));

        Diagnostics = new ObservableCollection<string>();
        ProvenanceEvents = new ObservableCollection<RelayProvenanceItemViewModel>();

        GeneratePaReviewCommand = new AsyncRelayCommand(GeneratePaReviewAsync, () => IsRelaySectionEnabled);
        CopyPaReviewCommand = new AsyncRelayCommand(CopyPaReviewAsync, () => HasPaReviewRendered);
        ImportPaHandoverCommand = new AsyncRelayCommand(ImportPaHandoverAsync, () => IsRelaySectionEnabled);
        PrepareEngineeringHandoverCommand = new AsyncRelayCommand(
            PrepareEngineeringHandoverAsync,
            () => IsRelaySectionEnabled && CanPrepareEngineeringHandover);
        CopyEngineeringHandoverCommand = new AsyncRelayCommand(
            CopyEngineeringHandoverAsync,
            () => HasEngineeringHandoverRendered);
        ImportEngineeringResultCommand = new AsyncRelayCommand(
            ImportEngineeringResultAsync,
            () => IsRelaySectionEnabled);
    }

    public ObservableCollection<string> Diagnostics { get; }

    public ObservableCollection<RelayProvenanceItemViewModel> ProvenanceEvents { get; }

    public ICommand GeneratePaReviewCommand { get; }

    public ICommand CopyPaReviewCommand { get; }

    public ICommand ImportPaHandoverCommand { get; }

    public ICommand PrepareEngineeringHandoverCommand { get; }

    public ICommand CopyEngineeringHandoverCommand { get; }

    public ICommand ImportEngineeringResultCommand { get; }

    public bool IsRelaySectionEnabled
    {
        get => _isRelaySectionEnabled;
        private set
        {
            if (SetProperty(ref _isRelaySectionEnabled, value))
            {
                RaiseRelayCommandCanExecuteChanged();
            }
        }
    }

    public AgentSessionIntent? ProjectArchitectSessionIntent
    {
        get => _projectArchitectSessionIntent;
        set
        {
            if (!SetProperty(ref _projectArchitectSessionIntent, value))
            {
                return;
            }

            ApplySessionIntentIfProjectActive(isProjectArchitect: true, value);
        }
    }

    public AgentSessionIntent? EngineeringAgentSessionIntent
    {
        get => _engineeringAgentSessionIntent;
        set
        {
            if (!SetProperty(ref _engineeringAgentSessionIntent, value))
            {
                return;
            }

            ApplySessionIntentIfProjectActive(isProjectArchitect: false, value);
        }
    }

    public EngineeringAgentMode EngineeringAgentMode
    {
        get => _engineeringAgentMode;
        set => SetProperty(ref _engineeringAgentMode, value);
    }

    public EngineeringAgentMode? PriorEngineeringAgentMode
    {
        get => _priorEngineeringAgentMode;
        set => SetProperty(ref _priorEngineeringAgentMode, value);
    }

    public string? PaReviewRendered
    {
        get => _paReviewRendered;
        private set
        {
            if (SetProperty(ref _paReviewRendered, value))
            {
                RaisePropertyChanged(nameof(HasPaReviewRendered));
                (CopyPaReviewCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasPaReviewRendered => !string.IsNullOrWhiteSpace(PaReviewRendered);

    public string PaImportText
    {
        get => _paImportText;
        set => SetProperty(ref _paImportText, value);
    }

    public string? PaReviewValidationSummary
    {
        get => _paReviewValidationSummary;
        private set => SetProperty(ref _paReviewValidationSummary, value);
    }

    public string? PaImportValidationSummary
    {
        get => _paImportValidationSummary;
        private set => SetProperty(ref _paImportValidationSummary, value);
    }

    public string? EngineeringHandoverRendered
    {
        get => _engineeringHandoverRendered;
        private set
        {
            if (SetProperty(ref _engineeringHandoverRendered, value))
            {
                RaisePropertyChanged(nameof(HasEngineeringHandoverRendered));
                (CopyEngineeringHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasEngineeringHandoverRendered => !string.IsNullOrWhiteSpace(EngineeringHandoverRendered);

    public string? EngineeringHandoverStatus
    {
        get => _engineeringHandoverStatus;
        private set
        {
            if (SetProperty(ref _engineeringHandoverStatus, value))
            {
                RaisePropertyChanged(nameof(CanPrepareEngineeringHandover));
                (PrepareEngineeringHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool CanPrepareEngineeringHandover =>
        _lastPaHandoverImport is not null
        && _lastPaHandoverValidation is not null
        && _lastPaHandoverValidation.IsEligibleForValidatedEngineeringAgentHandover
        && _lastPaHandoverImport.GovernanceCritical.Stop.State != RelayStopState.Active;

    public string EngineeringResultText
    {
        get => _engineeringResultText;
        set => SetProperty(ref _engineeringResultText, value);
    }

    public string? EngineeringResultValidationSummary
    {
        get => _engineeringResultValidationSummary;
        private set => SetProperty(ref _engineeringResultValidationSummary, value);
    }

    public string? RelayStatusMessage
    {
        get => _relayStatusMessage;
        private set => SetProperty(ref _relayStatusMessage, value);
    }

    public string SessionIntentHint =>
        "Session intents are explicit user choices (NEW/CONTINUE). They are not inferred from package prose.";

    public string AuthorizationBoundaryNotice =>
        "Governed relay packages present state only. Handover and PA acceptance do not grant implementation authorization.";

    public IReadOnlyList<AgentSessionIntent> SessionIntentOptions { get; } =
        [AgentSessionIntent.New, AgentSessionIntent.Continue];

    public IReadOnlyList<EngineeringAgentMode> EngineeringAgentModeOptions { get; } =
        [EngineeringAgentMode.Plan, EngineeringAgentMode.Agent];

    public IReadOnlyList<EngineeringAgentMode?> PriorEngineeringAgentModeOptions { get; } =
        [null, EngineeringAgentMode.Plan, EngineeringAgentMode.Agent];

    internal void OnActiveProjectChanged(ProjectConcordProjectId? projectId, bool hasActiveProject)
    {
        IsRelaySectionEnabled = hasActiveProject && projectId is not null;
        ClearRelayOutputs();

        if (!IsRelaySectionEnabled || projectId is not { } activeProjectId)
        {
            ProjectArchitectSessionIntent = null;
            EngineeringAgentSessionIntent = null;
            return;
        }

        var session = _workflow.GetSessionState(activeProjectId);
        _projectArchitectSessionIntent = session.ProjectArchitectSessionIntent;
        _engineeringAgentSessionIntent = session.EngineeringAgentSessionIntent;
        RaisePropertyChanged(nameof(ProjectArchitectSessionIntent));
        RaisePropertyChanged(nameof(EngineeringAgentSessionIntent));
        RefreshProvenance(activeProjectId);
    }

    private void ApplySessionIntentIfProjectActive(bool isProjectArchitect, AgentSessionIntent? intent)
    {
        if (!IsRelaySectionEnabled || _workspace.CurrentProjectId is not { } projectId || intent is null)
        {
            return;
        }

        if (isProjectArchitect)
        {
            _workflow.SetProjectArchitectSessionIntent(projectId, intent.Value);
        }
        else
        {
            _workflow.SetEngineeringAgentSessionIntent(projectId, intent.Value);
        }

        RefreshProvenance(projectId);
        RelayStatusMessage = "Session intent recorded.";
    }

    private async Task GeneratePaReviewAsync()
    {
        RelayStatusMessage = null;
        ClearDiagnostics();
        EngineeringHandoverRendered = null;
        EngineeringHandoverStatus = null;

        if (_workspace.CurrentProjectId is not { } projectId || _workspace.CurrentRoot is not { } root)
        {
            RelayStatusMessage = "Open a project before generating a PA review package.";
            return;
        }

        var result = _workflow.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(EngineeringAgentMode, PriorEngineeringAgentMode));

        ApplyValidationPresentation(result.Validation);
        PaReviewValidationSummary = FormatValidationHeading("PA review export", result.Validation);

        if (result.RenderedPackage is null)
        {
            PaReviewRendered = null;
            RelayStatusMessage = "PA review package was not rendered.";
            RefreshProvenance(projectId);
            return;
        }

        PaReviewRendered = result.RenderedPackage;
        RelayStatusMessage = "PA review package generated. Use Copy when ready to transfer manually.";
        RefreshProvenance(projectId);
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private async Task CopyPaReviewAsync()
    {
        if (string.IsNullOrWhiteSpace(PaReviewRendered))
        {
            return;
        }

        await _copyTextAsync(PaReviewRendered).ConfigureAwait(true);
        RelayStatusMessage = "PA review package copied to clipboard (manual transfer only).";
    }

    private async Task ImportPaHandoverAsync()
    {
        RelayStatusMessage = null;
        ClearDiagnostics();
        EngineeringHandoverRendered = null;
        EngineeringHandoverStatus = null;
        _lastPaHandoverImport = null;
        _lastPaHandoverValidation = null;
        RaisePropertyChanged(nameof(CanPrepareEngineeringHandover));
        (PrepareEngineeringHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();

        if (_workspace.CurrentProjectId is not { } projectId)
        {
            RelayStatusMessage = "Open a project before importing a PA handover.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PaImportText))
        {
            RelayStatusMessage = "Paste a PA handover response before importing.";
            return;
        }

        var result = _workflow.ImportPaHandover(projectId, PaImportText);
        if (!result.ProjectIdMatched)
        {
            PaImportValidationSummary = result.ProjectIdMismatchMessage;
            RelayStatusMessage = result.ProjectIdMismatchMessage;
            return;
        }

        ApplyValidationPresentation(result.Import.Validation);
        PaImportValidationSummary = FormatValidationHeading("PA handover import", result.Import.Validation);

        if (result.Import.Package is not null)
        {
            _lastPaHandoverImport = result.Import.Package;
            _lastPaHandoverValidation = result.Import.Validation;
            EngineeringHandoverStatus = DescribeEngineeringHandoverEligibility(result.Import);
        }

        RaisePropertyChanged(nameof(CanPrepareEngineeringHandover));
        (PrepareEngineeringHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        RefreshProvenance(projectId);
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private async Task PrepareEngineeringHandoverAsync()
    {
        RelayStatusMessage = null;
        ClearDiagnostics();
        EngineeringHandoverRendered = null;

        if (_workspace.CurrentProjectId is not { } projectId)
        {
            RelayStatusMessage = "Open a project before preparing an Engineering Agent handover.";
            return;
        }

        if (_lastPaHandoverImport is null || _lastPaHandoverValidation is null)
        {
            RelayStatusMessage = "Import a validated PA handover before preparing an Engineering Agent handover.";
            return;
        }

        var preparation = _workflow.PrepareEngineeringAgentHandover(_lastPaHandoverImport, _lastPaHandoverValidation);
        ApplyValidationPresentation(preparation.Validation);
        EngineeringHandoverStatus = FormatValidationHeading("Engineering Agent handover", preparation.Validation);

        if (!preparation.IsReadyForManualTransfer || preparation.RenderedHandover is null)
        {
            RelayStatusMessage = "Engineering Agent handover is not ready for manual transfer.";
            RefreshProvenance(projectId);
            return;
        }

        AssertRenderedUsesProjectConcordRelayV1(preparation.RenderedHandover);
        EngineeringHandoverRendered = preparation.RenderedHandover;
        RelayStatusMessage = "Engineering Agent handover prepared. Use Copy for manual transfer only.";
        RefreshProvenance(projectId);
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private async Task CopyEngineeringHandoverAsync()
    {
        if (string.IsNullOrWhiteSpace(EngineeringHandoverRendered))
        {
            return;
        }

        await _copyTextAsync(EngineeringHandoverRendered).ConfigureAwait(true);
        RelayStatusMessage = "Engineering Agent handover copied to clipboard (manual transfer only).";
    }

    private async Task ImportEngineeringResultAsync()
    {
        RelayStatusMessage = null;
        ClearDiagnostics();

        if (_workspace.CurrentProjectId is not { } projectId)
        {
            RelayStatusMessage = "Open a project before importing an engineering result.";
            return;
        }

        if (string.IsNullOrWhiteSpace(EngineeringResultText))
        {
            RelayStatusMessage = "Paste engineering result relay text before importing.";
            return;
        }

        var result = _workflow.ImportEngineeringResult(projectId, EngineeringResultText);
        if (!result.ProjectIdMatched)
        {
            EngineeringResultValidationSummary = result.ProjectIdMismatchMessage;
            RelayStatusMessage = result.ProjectIdMismatchMessage;
            return;
        }

        ApplyValidationPresentation(result.Import.Validation);
        EngineeringResultValidationSummary = FormatValidationHeading("Engineering result import", result.Import.Validation);
        RefreshProvenance(projectId);
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private void RefreshProvenance(ProjectConcordProjectId projectId)
    {
        ProvenanceEvents.Clear();
        foreach (var item in _workflow.ListRecentProvenance(projectId, ProvenanceListLimit))
        {
            ProvenanceEvents.Add(new RelayProvenanceItemViewModel(
                FormatProvenanceSummary(item),
                item.RecordedUtc.ToString("u")));
        }
    }

    private void ClearRelayOutputs()
    {
        PaReviewRendered = null;
        PaReviewValidationSummary = null;
        PaImportValidationSummary = null;
        EngineeringHandoverRendered = null;
        EngineeringHandoverStatus = null;
        EngineeringResultValidationSummary = null;
        _lastPaHandoverImport = null;
        _lastPaHandoverValidation = null;
        ClearDiagnostics();
        ProvenanceEvents.Clear();
        RaisePropertyChanged(nameof(CanPrepareEngineeringHandover));
        RaiseRelayCommandCanExecuteChanged();
    }

    private void ClearDiagnostics() => Diagnostics.Clear();

    private void ApplyValidationPresentation(RelayValidationResult validation)
    {
        ClearDiagnostics();
        foreach (var diagnostic in validation.Diagnostics)
        {
            Diagnostics.Add($"{diagnostic.Severity}: {diagnostic.Code} — {diagnostic.Message}");
        }
    }

    private static string FormatValidationHeading(string context, RelayValidationResult validation) =>
        $"{context}: {validation.State}";

    private static string DescribeEngineeringHandoverEligibility(PaHandoverImportResult import)
    {
        if (import.Validation.State == RelayValidationState.Incomplete)
        {
            return "PA handover is Incomplete — Engineering Agent handover cannot be marked ready.";
        }

        if (import.Validation.State == RelayValidationState.RejectedMalformed)
        {
            return "PA handover is RejectedMalformed — Engineering Agent handover cannot be marked ready.";
        }

        if (import.Package?.GovernanceCritical.Stop.State == RelayStopState.Active)
        {
            return "PA handover remains Valid, but active STOP blocks Engineering Agent handover readiness.";
        }

        if (import.Validation.IsEligibleForValidatedEngineeringAgentHandover)
        {
            return "PA handover is Valid and eligible for Engineering Agent handover preparation.";
        }

        return "PA handover validation did not reach Valid eligibility.";
    }

    private static string FormatProvenanceSummary(RelayProvenanceEvent item)
    {
        var correlation = item.CorrelationId?.Value.ToString("N")[..8] ?? "—";
        var package = item.PackageId?.Value.ToString("N")[..8] ?? "—";
        return $"{item.EventType} · package {package} · correlation {correlation}";
    }

    internal static bool RenderedHandoverUsesProjectConcordRelayV1(string rendered) =>
        rendered.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, StringComparison.Ordinal);

    private static void AssertRenderedUsesProjectConcordRelayV1(string rendered)
    {
        if (!RenderedHandoverUsesProjectConcordRelayV1(rendered))
        {
            throw new InvalidOperationException("Rendered handover must use projectconcord-relay-v1.");
        }
    }

    private void RaiseRelayCommandCanExecuteChanged()
    {
        (GeneratePaReviewCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ImportPaHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (PrepareEngineeringHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ImportEngineeringResultCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
    }
}
