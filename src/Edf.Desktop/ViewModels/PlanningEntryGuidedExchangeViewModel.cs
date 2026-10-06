namespace Edf.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Edf.Application.Composition;
using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

public sealed class PlanningEntryGuidedExchangeViewModel : ViewModelBase
{
    private readonly IGovernedRelayP0WorkflowService _relayWorkflow;
    private readonly IProjectWorkspaceService _workspace;
    private readonly WorkflowApplicationServices _workflowServices;
    private readonly Func<string, Task> _copyTextAsync;
    private readonly Action _returnToCurrentWork;
    private readonly Action? _onWorkStateMayHaveChanged;
    private readonly PlanningEntryGuidedTransientState _transient = new();
    private PlanningEntryGuidedStep _currentStep = PlanningEntryGuidedStep.Inactive;
    private string? _stepTitle;
    private string? _stepBody;
    private string? _statusMessage;
    private string? _decisionSummary;
    private string? _validationOperatorMessage;
    private string? _technicalValidationDetail;
    private bool _reviewCopied;
    private bool _showAwaitPaResponseButton;
    private bool _isActive;
    private AgentSessionIntent? _selectedPaSessionIntent;
    private AgentSessionIntent? _selectedEaSessionIntent;
    private string _paResponseDraft = string.Empty;

    public PlanningEntryGuidedExchangeViewModel(
        IGovernedRelayP0WorkflowService relayWorkflow,
        IProjectWorkspaceService workspace,
        WorkflowApplicationServices workflowServices,
        Func<string, Task> copyTextAsync,
        Action returnToCurrentWork,
        Action? onWorkStateMayHaveChanged = null)
    {
        _relayWorkflow = relayWorkflow ?? throw new ArgumentNullException(nameof(relayWorkflow));
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _workflowServices = workflowServices ?? throw new ArgumentNullException(nameof(workflowServices));
        _copyTextAsync = copyTextAsync ?? throw new ArgumentNullException(nameof(copyTextAsync));
        _returnToCurrentWork = returnToCurrentWork ?? throw new ArgumentNullException(nameof(returnToCurrentWork));
        _onWorkStateMayHaveChanged = onWorkStateMayHaveChanged;

        TechnicalDiagnostics = new ObservableCollection<string>();

        PrepareReviewCommand = new AsyncRelayCommand(PrepareReviewAsync, () => IsActive && _currentStep == PlanningEntryGuidedStep.PrepareReview);
        ConfirmSessionContinuityCommand = new RelayCommand(ConfirmSessionContinuity, () => IsActive && _currentStep == PlanningEntryGuidedStep.ConfirmSessionContinuity);
        CopyReviewCommand = new AsyncRelayCommand(CopyReviewAsync, () => IsActive && _currentStep == PlanningEntryGuidedStep.SendReview && HasRenderableReview);
        AcknowledgeHavePaResponseCommand = new RelayCommand(AcknowledgeHavePaResponse, () => IsActive && _reviewCopied && _showAwaitPaResponseButton);
        ValidatePaResponseCommand = new AsyncRelayCommand(ValidatePaResponseAsync, () => IsActive && _currentStep == PlanningEntryGuidedStep.ValidateResponse);
        EnterGovernedPlanningCommand = new AsyncRelayCommand(EnterGovernedPlanningAsync, () => IsActive && _currentStep == PlanningEntryGuidedStep.ReviewDecision);
        ReturnToCurrentWorkCommand = new RelayCommand(() => _returnToCurrentWork(), () => IsActive);
        PrepareNewReviewCommand = new RelayCommand(StartNewReviewCycle, () => IsActive);
    }

    public ObservableCollection<string> TechnicalDiagnostics { get; }

    public bool IsActive
    {
        get => _isActive;
        private set
        {
            if (SetProperty(ref _isActive, value))
            {
                RaiseAllCommandCanExecuteChanged();
            }
        }
    }

    public PlanningEntryGuidedStep CurrentStep
    {
        get => _currentStep;
        private set => SetProperty(ref _currentStep, value);
    }

    public string? StepTitle
    {
        get => _stepTitle;
        private set => SetProperty(ref _stepTitle, value);
    }

    public string? StepBody
    {
        get => _stepBody;
        private set => SetProperty(ref _stepBody, value);
    }

    public string StepProgressLabel =>
        _currentStep is PlanningEntryGuidedStep.Inactive or PlanningEntryGuidedStep.Complete
            ? string.Empty
            : $"Step {PlanningEntryGuidedStepResolver.StepNumber(_currentStep)} of {PlanningEntryGuidedStepResolver.StepCount(_currentStep)}";

    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool ReviewCopied
    {
        get => _reviewCopied;
        private set => SetProperty(ref _reviewCopied, value);
    }

    public bool ShowAwaitPaResponseButton
    {
        get => _showAwaitPaResponseButton;
        private set => SetProperty(ref _showAwaitPaResponseButton, value);
    }

    public string PaResponseDraft
    {
        get => _paResponseDraft;
        set
        {
            if (SetProperty(ref _paResponseDraft, value))
            {
                _transient.PaResponseDraft = value;
            }
        }
    }

    public string? ValidationOperatorMessage
    {
        get => _validationOperatorMessage;
        private set => SetProperty(ref _validationOperatorMessage, value);
    }

    public string? TechnicalValidationDetail
    {
        get => _technicalValidationDetail;
        private set => SetProperty(ref _technicalValidationDetail, value);
    }

    public string? DecisionSummary
    {
        get => _decisionSummary;
        private set => SetProperty(ref _decisionSummary, value);
    }

    public AgentSessionIntent? SelectedPaSessionIntent
    {
        get => _selectedPaSessionIntent;
        set => SetProperty(ref _selectedPaSessionIntent, value);
    }

    public AgentSessionIntent? SelectedEaSessionIntent
    {
        get => _selectedEaSessionIntent;
        set => SetProperty(ref _selectedEaSessionIntent, value);
    }

    public IReadOnlyList<AgentSessionIntent> SessionIntentOptions { get; } =
        [AgentSessionIntent.New, AgentSessionIntent.Continue];

    public bool HasRenderableReview =>
        !string.IsNullOrWhiteSpace(_transient.CachedRenderedReview);

    public bool ShowConfirmSessionContinuity => _currentStep == PlanningEntryGuidedStep.ConfirmSessionContinuity;

    public bool ShowPrepareReview => _currentStep == PlanningEntryGuidedStep.PrepareReview;

    public bool ShowSendReview => _currentStep == PlanningEntryGuidedStep.SendReview;

    public bool ShowValidateResponse => _currentStep == PlanningEntryGuidedStep.ValidateResponse;

    public bool ShowReviewDecision => _currentStep == PlanningEntryGuidedStep.ReviewDecision;

    public bool ShowComplete => _currentStep == PlanningEntryGuidedStep.Complete;

    public ICommand PrepareReviewCommand { get; }

    public ICommand ConfirmSessionContinuityCommand { get; }

    public ICommand CopyReviewCommand { get; }

    public ICommand AcknowledgeHavePaResponseCommand { get; }

    public ICommand ValidatePaResponseCommand { get; }

    public ICommand EnterGovernedPlanningCommand { get; }

    public ICommand ReturnToCurrentWorkCommand { get; }

    public ICommand PrepareNewReviewCommand { get; }

    public void ActivatePlanningEntryGuided()
    {
        IsActive = true;
        Refresh();
    }

    public void Deactivate()
    {
        IsActive = false;
        CurrentStep = PlanningEntryGuidedStep.Inactive;
    }

    public void OnActiveProjectChanged(ProjectConcordProjectId? projectId, bool hasActiveProject)
    {
        if (!hasActiveProject || projectId is null)
        {
            Deactivate();
            return;
        }

        if (IsActive)
        {
            HydrateSessionIntentSelections(projectId.Value);
            Refresh();
        }
    }

    public (GovernedRelayPackage? Package, RelayValidationResult? Validation) ResolveAuthoritativeConsumedHandover(
        ProjectConcordProjectId projectId)
    {
        var relay = _workflowServices.PlanningEntryRelayReadModel.Resolve(projectId);
        var consumed = relay.LatestConsumedPaHandover;
        if (consumed is null)
        {
            return default;
        }

        return (consumed.Package, consumed.Validation);
    }

    internal void Refresh()
    {
        if (!IsActive || _workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        var place = ResolveIntakeTopologyPlace(projectId);
        var relay = _workflowServices.PlanningEntryRelayReadModel.Resolve(projectId);
        var session = _relayWorkflow.GetSessionState(projectId);
        var sessionReady = session.ProjectArchitectSessionIntent is not null
                           && session.EngineeringAgentSessionIntent is not null;

        ReviewCopied = _transient.ReviewCopied;
        ShowAwaitPaResponseButton = _transient.ReviewCopied && !_transient.AwaitingPaResponseAcknowledged;

        if (relay.LatestPaReviewExport is not null
            && string.IsNullOrWhiteSpace(_transient.CachedRenderedReview))
        {
            _transient.CachedRenderedReview = PlanningEntryReviewExportRenderer.RenderCompleteClipboardPayload(
                relay.LatestPaReviewExport.Package);
        }

        var step = PlanningEntryGuidedStepResolver.Resolve(
            place,
            relay,
            _transient,
            _workflowServices.IntakePlanningEntryTransitions,
            projectId,
            sessionReady);

        CurrentStep = step;
        ApplyStepPresentation(step, relay, projectId, sessionReady);
        RaisePropertyChanged(nameof(StepProgressLabel));
        RaisePropertyChanged(nameof(HasRenderableReview));
        RaiseStepVisibilityProperties();
        RaiseAllCommandCanExecuteChanged();
    }

    private void RaiseStepVisibilityProperties()
    {
        RaisePropertyChanged(nameof(ShowConfirmSessionContinuity));
        RaisePropertyChanged(nameof(ShowPrepareReview));
        RaisePropertyChanged(nameof(ShowSendReview));
        RaisePropertyChanged(nameof(ShowValidateResponse));
        RaisePropertyChanged(nameof(ShowReviewDecision));
        RaisePropertyChanged(nameof(ShowComplete));
    }

    private void ApplyStepPresentation(
        PlanningEntryGuidedStep step,
        PlanningEntryRelayReadModelSnapshot relay,
        ProjectConcordProjectId projectId,
        bool sessionReady)
    {
        TechnicalDiagnostics.Clear();
        ValidationOperatorMessage = _transient.LastOperatorValidationMessage;
        DecisionSummary = null;

        switch (step)
        {
            case PlanningEntryGuidedStep.Complete:
                StepTitle = "Entered Governed Planning";
                StepBody = "ProjectConcord is now in Governed Planning. Return to Current Work to see what is next.";
                break;
            case PlanningEntryGuidedStep.ConfirmSessionContinuity:
                StepTitle = "Conversation continuity";
                StepBody =
                    "Before preparing the review, ProjectConcord needs your choices for conversation continuity. "
                    + "These are not guesses — select what applies to your Project Architect and Engineering Agent conversations.";
                HydrateSessionIntentSelections(projectId);
                break;
            case PlanningEntryGuidedStep.PrepareReview:
                StepTitle = "Prepare review";
                StepBody =
                    "ProjectConcord needs your Project Architect to decide whether this project may enter Governed Planning. "
                    + "ProjectConcord will prepare the governed project information needed for that review.";
                break;
            case PlanningEntryGuidedStep.SendReview:
                StepTitle = "Send review to Project Architect";
                StepBody = ReviewCopied
                    ? "Review copied. Paste it into your Project Architect conversation. When the Project Architect returns a response, come back here."
                    : "Your Project Architect review is ready. Copy the review and paste it into your Project Architect conversation.";
                break;
            case PlanningEntryGuidedStep.ValidateResponse:
                StepTitle = "Bring back the Project Architect response";
                StepBody =
                    "Copy the complete response returned by your Project Architect (one outer copy surface) and paste it below. "
                    + "ProjectConcord will validate it before you can enter Governed Planning.";
                break;
            case PlanningEntryGuidedStep.ReviewDecision:
                StepTitle = "Review Project Architect decision";
                StepBody = "Project Architect decision received. Review what this permits before entering Governed Planning.";
                DecisionSummary = ComposeDecisionSummary(relay.LatestConsumedPaHandover);
                break;
            default:
                StepTitle = "Planning Entry";
                StepBody = "This guided exchange is not active for the current workflow state.";
                break;
        }
    }

    private async Task PrepareReviewAsync()
    {
        StatusMessage = null;
        if (_workspace.CurrentProjectId is not { } projectId || _workspace.CurrentRoot is not { } root)
        {
            StatusMessage = "Open a project before preparing a review.";
            return;
        }

        var session = _relayWorkflow.GetSessionState(projectId);
        if (session.ProjectArchitectSessionIntent is null || session.EngineeringAgentSessionIntent is null)
        {
            Refresh();
            return;
        }

        var result = _relayWorkflow.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(
                EngineeringAgentMode.Plan,
                null,
                PaHandoverResponseProfile.PlanningEntry));

        if (result.RenderedPackage is null)
        {
            StatusMessage = "The review could not be prepared.";
            AppendTechnicalDiagnostics(result.Validation.Diagnostics);
            Refresh();
            return;
        }

        _transient.ReviewCopied = false;
        _transient.AwaitingPaResponseAcknowledged = false;
        _transient.LastValidationAttemptFailed = false;
        _transient.LastOperatorValidationMessage = null;
        _transient.CachedRenderedReview = result.RenderedPackage;
        PaResponseDraft = string.Empty;
        StatusMessage = "Review prepared.";
        Refresh();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private void ConfirmSessionContinuity()
    {
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        if (SelectedPaSessionIntent is not { } paIntent || SelectedEaSessionIntent is not { } eaIntent)
        {
            StatusMessage = "Select continuity for both conversations before continuing.";
            return;
        }

        _relayWorkflow.SetProjectArchitectSessionIntent(projectId, paIntent);
        _relayWorkflow.SetEngineeringAgentSessionIntent(projectId, eaIntent);
        StatusMessage = "Conversation continuity recorded.";
        Refresh();
    }

    private async Task CopyReviewAsync()
    {
        if (string.IsNullOrWhiteSpace(_transient.CachedRenderedReview))
        {
            return;
        }

        await _copyTextAsync(_transient.CachedRenderedReview).ConfigureAwait(true);
        _transient.ReviewCopied = true;
        ReviewCopied = true;
        ShowAwaitPaResponseButton = true;
        StatusMessage = "Review copied.";
        Refresh();
    }

    private void AcknowledgeHavePaResponse()
    {
        _transient.AwaitingPaResponseAcknowledged = true;
        ShowAwaitPaResponseButton = false;
        Refresh();
    }

    private async Task ValidatePaResponseAsync()
    {
        ValidationOperatorMessage = null;
        TechnicalValidationDetail = null;
        TechnicalDiagnostics.Clear();
        _transient.LastValidationAttemptFailed = false;

        if (_workspace.CurrentProjectId is not { } projectId)
        {
            ValidationOperatorMessage = "Open a project before validating a response.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PaResponseDraft))
        {
            ValidationOperatorMessage = "Paste the complete Project Architect response first.";
            return;
        }

        var result = _relayWorkflow.ImportPaHandover(projectId, PaResponseDraft);
        if (result.Import.Package is null || result.Import.Validation.State == RelayValidationState.RejectedMalformed)
        {
            _transient.LastValidationAttemptFailed = true;
            ValidationOperatorMessage = ComposeImportFailureOperatorMessage(result.Import.Validation);
            TechnicalValidationDetail = FormatValidationDetail(result.Import.Validation);
            AppendTechnicalDiagnostics(result.Import.Validation.Diagnostics);
            _transient.LastOperatorValidationMessage = ValidationOperatorMessage;
            Refresh();
            return;
        }

        if (!result.ProjectIdMatched)
        {
            _transient.LastValidationAttemptFailed = true;
            ValidationOperatorMessage = result.ProjectIdMismatchMessage
                                        ?? "This response belongs to a different project.";
            _transient.LastOperatorValidationMessage = ValidationOperatorMessage;
            Refresh();
            return;
        }

        var eligibility = _workflowServices.IntakePlanningEntryTransitions.EvaluateEligibility(
            projectId,
            result.Import.Package,
            result.Import.Validation);

        if (!eligibility.IsEligible)
        {
            _transient.LastValidationAttemptFailed = true;
            ValidationOperatorMessage =
                "The response was read, but it does not authorize Governed Planning entry. "
                + "Ask your Project Architect for a planning-entry handover that authorizes planning without implementation.";
            TechnicalValidationDetail = eligibility.ReasonCode;
            _transient.LastOperatorValidationMessage = ValidationOperatorMessage;
            Refresh();
            return;
        }

        _transient.LastValidationAttemptFailed = false;
        _transient.LastOperatorValidationMessage = null;
        ValidationOperatorMessage = null;
        StatusMessage = "Project Architect response validated.";
        _onWorkStateMayHaveChanged?.Invoke();
        Refresh();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private async Task EnterGovernedPlanningAsync()
    {
        StatusMessage = null;
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        var (package, validation) = ResolveAuthoritativeConsumedHandover(projectId);
        if (package is null || validation is null)
        {
            StatusMessage = "A validated Project Architect response is required before entering Governed Planning.";
            Refresh();
            return;
        }

        var result = _workflowServices.IntakePlanningEntryTransitions.TryEnterGovernedPlanning(
            projectId,
            package,
            validation);
        StatusMessage = result.OperatorMessage;
        _onWorkStateMayHaveChanged?.Invoke();
        Refresh();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private void StartNewReviewCycle()
    {
        _transient.ResetForNewReviewCycle();
        PaResponseDraft = string.Empty;
        ReviewCopied = false;
        Refresh();
    }

    private void HydrateSessionIntentSelections(ProjectConcordProjectId projectId)
    {
        var session = _relayWorkflow.GetSessionState(projectId);
        if (session.ProjectArchitectSessionIntent is not null)
        {
            SelectedPaSessionIntent = session.ProjectArchitectSessionIntent;
        }

        if (session.EngineeringAgentSessionIntent is not null)
        {
            SelectedEaSessionIntent = session.EngineeringAgentSessionIntent;
        }
    }

    private string? ResolveIntakeTopologyPlace(ProjectConcordProjectId projectId)
    {
        var recovery = _workflowServices.WorkStateRecovery.RecoverForProject(projectId);
        var intake = recovery.ActiveInstances
            .FirstOrDefault(i => i.TopologyPlaceId.Value == GewV1TopologyPlaces.Intake);
        if (intake is not null)
        {
            return GewV1TopologyPlaces.Intake;
        }

        var planning = recovery.ActiveInstances
            .FirstOrDefault(i => i.TopologyPlaceId.Value == GewV1TopologyPlaces.PlanningGoverned);
        return planning?.TopologyPlaceId.Value;
    }

    private static string ComposeDecisionSummary(PlanningEntryRelayHandoverSnapshot? consumed)
    {
        if (consumed is null)
        {
            return string.Empty;
        }

        var package = consumed.Package;
        var gc = package.GovernanceCritical;
        var builder = new StringBuilder();
        builder.AppendLine("✓ Governed Planning authorized for entry (planning entry handover).");
        builder.AppendLine("Implementation is NOT authorized by this decision.");

        if (gc.Stop.State == RelayStopState.Active)
        {
            builder.AppendLine("STOP is active — additional governed constraints apply.");
        }
        else
        {
            builder.AppendLine("STOP: none.");
        }

        if (!SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(
                package.ProfilePayload,
                out var payload,
                out _)
            || payload is null)
        {
            return builder.ToString().TrimEnd();
        }

        if (payload.DevelopmentWorkAuthorization is not null)
        {
            builder.AppendLine("A development-work authorization record is present — review technical detail if unexpected.");
        }
        else
        {
            builder.AppendLine("No durable development-work authorization is granted by this step.");
        }

        if (payload.AuthorizationDisposition is { } disposition)
        {
            builder.AppendLine($"Disposition: {disposition.DispositionSummary}");
        }

        return builder.ToString().TrimEnd();
    }

    private static string ComposeImportFailureOperatorMessage(RelayValidationResult validation)
    {
        var codes = validation.Diagnostics.Select(d => d.Code).ToHashSet(StringComparer.Ordinal);
        if (codes.Contains(RelayValidationCodes.GovernanceProjectionMismatch))
        {
            return "ProjectConcord received the Project Architect response, but its governance information is inconsistent. "
                   + "The response could not be accepted.";
        }

        if (codes.Any(c => c.StartsWith("relay.manual_paste.", StringComparison.Ordinal))
            || codes.Contains(RelayValidationCodes.MachineBlockMissing)
            || codes.Contains(RelayValidationCodes.RenderVersionMissing)
            || codes.Contains(RelayValidationCodes.ManualPasteJsonOnlyRejected)
            || codes.Contains(RelayValidationCodes.ManualPasteMachineBlockOnlyRejected)
            || codes.Contains(RelayValidationCodes.ManualPasteRenderMarkerMissing))
        {
            return "ProjectConcord could not read a complete Project Architect response. "
                   + "Copy the complete outer plain-text artifact from your Project Architect (not JSON alone) and try again.";
        }

        return "ProjectConcord could not validate this Project Architect response. "
               + "Review the response with your Project Architect and try again.";
    }

    private static string FormatValidationDetail(RelayValidationResult validation)
    {
        if (validation.Diagnostics.Count == 0)
        {
            return validation.State.ToString();
        }

        return string.Join(
            "; ",
            validation.Diagnostics.Select(d => $"{d.Code}: {d.Message}"));
    }

    private void AppendTechnicalDiagnostics(IReadOnlyList<RelayValidationDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            TechnicalDiagnostics.Add($"{diagnostic.Code}: {diagnostic.Message}");
        }
    }

    private void RaiseAllCommandCanExecuteChanged()
    {
        (PrepareReviewCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ConfirmSessionContinuityCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (CopyReviewCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (AcknowledgeHavePaResponseCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ValidatePaResponseCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (EnterGovernedPlanningCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ReturnToCurrentWorkCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PrepareNewReviewCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
