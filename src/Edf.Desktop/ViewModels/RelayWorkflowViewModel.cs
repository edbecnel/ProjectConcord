namespace Edf.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Edf.Application.Operator;
using Edf.Application.Operator.Relay;
using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Transport;
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
    private readonly IEngineeringAgentAutomatedTransportService? _automatedTransport;
    private readonly RelayWorkflowOperatorProjectionService? _operatorProjections;
    private readonly Action? _onOperatorWorkStateMayHaveChanged;

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
    private EngineeringAgentAutomatedTransportResult? _lastAutomatedTransportResult;
    private string? _automatedTransportStatus;
    private string? _recommendedManualRelaySummary;
    private bool _hasAutomatedTransportIntegration;

    public RelayWorkflowViewModel(
        IGovernedRelayP0WorkflowService workflow,
        IProjectWorkspaceService workspace,
        Func<string, Task> copyTextAsync)
        : this(workflow, workspace, copyTextAsync, null, null, null)
    {
    }

    public RelayWorkflowViewModel(
        IGovernedRelayP0WorkflowService workflow,
        IProjectWorkspaceService workspace,
        Func<string, Task> copyTextAsync,
        IEngineeringAgentAutomatedTransportService? automatedTransport,
        RelayWorkflowOperatorProjectionService? operatorProjections,
        Action? onOperatorWorkStateMayHaveChanged = null)
    {
        _workflow = workflow ?? throw new ArgumentNullException(nameof(workflow));
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _copyTextAsync = copyTextAsync ?? throw new ArgumentNullException(nameof(copyTextAsync));
        _automatedTransport = automatedTransport;
        _operatorProjections = operatorProjections;
        _onOperatorWorkStateMayHaveChanged = onOperatorWorkStateMayHaveChanged;
        _hasAutomatedTransportIntegration = automatedTransport is not null && operatorProjections is not null;

        Diagnostics = new ObservableCollection<string>();
        ProvenanceEvents = new ObservableCollection<RelayProvenanceItemViewModel>();
        AttentionItems = new ObservableCollection<OperatorAttentionItemViewModel>();

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
        ForwardAutomatedHandoverCommand = new AsyncRelayCommand(
            ForwardAutomatedHandoverAsync,
            () => IsRelaySectionEnabled && CanForwardAutomatedHandover);
        CancelAutomatedTransportCommand = new AsyncRelayCommand(
            CancelAutomatedTransportAsync,
            () => IsRelaySectionEnabled && CanCancelAutomatedTransport);
    }

    public (GovernedRelayPackage? Package, RelayValidationResult? Validation) ConsumedPaHandover =>
        (_lastPaHandoverImport, _lastPaHandoverValidation);

    public ObservableCollection<string> Diagnostics { get; }

    public ObservableCollection<OperatorAttentionItemViewModel> AttentionItems { get; }

    public ObservableCollection<RelayProvenanceItemViewModel> ProvenanceEvents { get; }

    public ICommand GeneratePaReviewCommand { get; }

    public ICommand CopyPaReviewCommand { get; }

    public ICommand ImportPaHandoverCommand { get; }

    public ICommand PrepareEngineeringHandoverCommand { get; }

    public ICommand CopyEngineeringHandoverCommand { get; }

    public ICommand ImportEngineeringResultCommand { get; }

    public ICommand ForwardAutomatedHandoverCommand { get; }

    public ICommand CancelAutomatedTransportCommand { get; }

    public bool HasAutomatedTransportIntegration => _hasAutomatedTransportIntegration;

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
        set
        {
            if (SetProperty(ref _engineeringAgentMode, value))
            {
                RefreshOperatorProjections();
            }
        }
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

    public string? AutomatedTransportStatus
    {
        get => _automatedTransportStatus;
        private set => SetProperty(ref _automatedTransportStatus, value);
    }

    public string? RecommendedManualRelaySummary
    {
        get => _recommendedManualRelaySummary;
        private set
        {
            if (SetProperty(ref _recommendedManualRelaySummary, value))
            {
                RaisePropertyChanged(nameof(HasRecommendedManualRelay));
            }
        }
    }

    public bool HasRecommendedManualRelay => !string.IsNullOrWhiteSpace(RecommendedManualRelaySummary);

    public bool CanForwardAutomatedHandover { get; private set; }

    public bool CanCancelAutomatedTransport { get; private set; }

    public string SessionIntentHint =>
        "Session intents are explicit user choices (NEW/CONTINUE). They are not inferred from package prose.";

    public const string ExchangeSectionTitle = "Governed Exchange";

    public string AuthorizationBoundaryNotice =>
        "Package validation and handover acceptance do not grant durable development work authorization or implementation permission.";

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
        RefreshOperatorProjections();
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
        RefreshOperatorProjections();
        _onOperatorWorkStateMayHaveChanged?.Invoke();
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
        RefreshOperatorProjections();
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
        RefreshOperatorProjections();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private async Task ForwardAutomatedHandoverAsync()
    {
        if (_automatedTransport is null || _workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        if (_workspace.CurrentRoot is not { } projectRoot)
        {
            RelayStatusMessage = "Open a Project Root before automated forward.";
            return;
        }

        if (_lastPaHandoverImport is null)
        {
            RelayStatusMessage = "Import a validated PA handover before automated forward.";
            return;
        }

        RelayStatusMessage = null;
        var governedRoot = ProjectLocator.FromPath(projectRoot.AbsolutePath);
        var result = await _automatedTransport.ForwardGovernedHandoverAsync(
            new EngineeringAgentAutomatedForwardRequest(
                projectId,
                _lastPaHandoverImport.PackageId,
                EngineeringAgentMode,
                governedRoot)).ConfigureAwait(true);

        _lastAutomatedTransportResult = result;
        if (result.GovernanceValidation is not null)
        {
            ApplyValidationPresentation(result.GovernanceValidation);
        }

        RelayStatusMessage = $"Automated transport completed with outcome {result.Outcome}.";
        RefreshProvenance(projectId);
        RefreshOperatorProjections();
    }

    private async Task CancelAutomatedTransportAsync()
    {
        if (_automatedTransport is null
            || _workspace.CurrentProjectId is not { } projectId
            || _lastAutomatedTransportResult?.Operation is not { } operation)
        {
            return;
        }

        var result = await _automatedTransport.CancelTransportOperationAsync(
            new EngineeringAgentAutomatedCancelRequest(projectId, operation.OperationId)).ConfigureAwait(true);
        _lastAutomatedTransportResult = result;
        RelayStatusMessage = $"Automated transport cancel completed with outcome {result.Outcome}.";
        RefreshOperatorProjections();
    }

    private void RefreshOperatorProjections()
    {
        if (_operatorProjections is null)
        {
            AttentionItems.Clear();
            AutomatedTransportStatus = null;
            RecommendedManualRelaySummary = null;
            CanForwardAutomatedHandover = false;
            CanCancelAutomatedTransport = false;
            RaiseAutomatedTransportCommandCanExecuteChanged();
            _onOperatorWorkStateMayHaveChanged?.Invoke();
            return;
        }

        var input = new RelayWorkflowOperatorProjectionInput(
            _workspace.CurrentProjectId,
            EngineeringAgentMode,
            _lastPaHandoverImport,
            _lastPaHandoverValidation,
            _lastAutomatedTransportResult);

        var projection = _operatorProjections.Project(input);
        AttentionItems.Clear();
        foreach (var item in projection.AttentionItems)
        {
            AttentionItems.Add(new OperatorAttentionItemViewModel(item));
        }

        AutomatedTransportStatus = projection.AutomatedTransportStatusSummary;
        RecommendedManualRelaySummary = projection.NextActions
            .FirstOrDefault(a => a.ActionClass == OperatorNextActionClass.Recommended)
            ?.Message;
        CanForwardAutomatedHandover = projection.CanAttemptAutomatedForward;
        CanCancelAutomatedTransport = _lastAutomatedTransportResult?.Operation?.LifecycleState
            is TransportOperationLifecycleState.ForwardAcknowledged
            or TransportOperationLifecycleState.AwaitingResult
            or TransportOperationLifecycleState.ForwardInProgress;
        RaiseAutomatedTransportCommandCanExecuteChanged();
        _onOperatorWorkStateMayHaveChanged?.Invoke();
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
        _lastAutomatedTransportResult = null;
        AttentionItems.Clear();
        AutomatedTransportStatus = null;
        RecommendedManualRelaySummary = null;
        RaisePropertyChanged(nameof(CanPrepareEngineeringHandover));
        RaiseRelayCommandCanExecuteChanged();
        RefreshOperatorProjections();
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

    private static string FormatValidationHeading(string context, RelayValidationResult validation)
    {
        if (validation.Diagnostics.Count == 0)
        {
            return $"{context}: {validation.State}";
        }

        var diagnosticLines = validation.Diagnostics
            .Select(d => $"  • {d.Code} — {d.Message}");
        return $"{context}: {validation.State}\n{string.Join('\n', diagnosticLines)}";
    }

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
        RaiseAutomatedTransportCommandCanExecuteChanged();
    }

    private void RaiseAutomatedTransportCommandCanExecuteChanged()
    {
        (ForwardAutomatedHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (CancelAutomatedTransportCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        RaisePropertyChanged(nameof(CanForwardAutomatedHandover));
        RaisePropertyChanged(nameof(CanCancelAutomatedTransport));
    }
}
