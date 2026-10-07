namespace Edf.Desktop.ViewModels;

using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Edf.Application.Composition;
using Edf.Application.Operator;
using Edf.Application.Operator.PlanningRegion;
using Edf.Application.Operator.WorkContinuity;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

public sealed class PlanningRegionGuidedExchangeViewModel : ViewModelBase
{
    private readonly IGovernedRelayP0WorkflowService _relayWorkflow;
    private readonly IProjectWorkspaceService _workspace;
    private readonly WorkflowApplicationServices _workflowServices;
    private readonly Func<string, Task> _copyTextAsync;
    private readonly Action _returnToCurrentWork;
    private readonly Action? _onWorkStateMayHaveChanged;
    private readonly Func<string?> _projectDisplayNameProvider;
    private readonly PlanningRegionGuidedTransientState _transient = new();
    private PlanningRegionGuidedStep _currentStep = PlanningRegionGuidedStep.Inactive;
    private bool _isActive;
    private bool _requiresSubjectConfirmation;
    private string _pendingSubjectConfirmation = string.Empty;
    private string? _workSubject;
    private string? _whatJustHappened;
    private string? _authorizationSummary;
    private string? _stopSummary;
    private string? _nextStepSummary;
    private string? _whySummary;
    private string? _primaryActionLabel;
    private string? _stepTitle;
    private string? _stepBody;
    private string? _statusMessage;
    private string? _validationOperatorMessage;
    private string? _technicalValidationDetail;
    private bool _showCopyCorrectionRequest;
    private string? _correctionNextStepHint;
    private string? _humanReadableOutboundPackageView;
    private bool _importAttestationConfirmed;
    private AgentSessionIntent? _selectedPaSessionIntent;
    private AgentSessionIntent? _selectedEaSessionIntent;
    private string _paResponseDraft = string.Empty;
    private OperatorActiveWorkFocus? _activeFocus;

    public PlanningRegionGuidedExchangeViewModel(
        IGovernedRelayP0WorkflowService relayWorkflow,
        IProjectWorkspaceService workspace,
        WorkflowApplicationServices workflowServices,
        Func<string, Task> copyTextAsync,
        Action returnToCurrentWork,
        Func<string?> projectDisplayNameProvider,
        Action? onWorkStateMayHaveChanged = null)
    {
        _relayWorkflow = relayWorkflow ?? throw new ArgumentNullException(nameof(relayWorkflow));
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        _workflowServices = workflowServices ?? throw new ArgumentNullException(nameof(workflowServices));
        _copyTextAsync = copyTextAsync ?? throw new ArgumentNullException(nameof(copyTextAsync));
        _returnToCurrentWork = returnToCurrentWork ?? throw new ArgumentNullException(nameof(returnToCurrentWork));
        _projectDisplayNameProvider = projectDisplayNameProvider ?? throw new ArgumentNullException(nameof(projectDisplayNameProvider));
        _onWorkStateMayHaveChanged = onWorkStateMayHaveChanged;

        TechnicalDiagnostics = new ObservableCollection<string>();

        ConfirmSubjectCommand = new RelayCommand(ConfirmSubject, () => IsActive && _requiresSubjectConfirmation);
        ConfirmSessionContinuityCommand = new RelayCommand(
            ConfirmSessionContinuity,
            () => IsActive && _currentStep == PlanningRegionGuidedStep.ConfirmSessionContinuity);
        PrimaryActionCommand = new AsyncRelayCommand(ExecutePrimaryActionAsync, () => IsActive && HasPrimaryAction);
        ValidatePaResponseCommand = new AsyncRelayCommand(
            ValidatePaResponseAsync,
            () => IsActive && _currentStep == PlanningRegionGuidedStep.BringBackPaResponse);
        CopyCorrectionRequestCommand = new AsyncRelayCommand(
            CopyCorrectionRequestAsync,
            () => IsActive && _showCopyCorrectionRequest);
        CopyEngineeringAgentHandoverCommand = new AsyncRelayCommand(
            CopyEngineeringAgentHandoverAsync,
            () => IsActive && ShowCopyEngineeringAgentHandover);
        ReturnToCurrentWorkCommand = new RelayCommand(() => _returnToCurrentWork(), () => IsActive);
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

    public string? WorkSubject
    {
        get => _workSubject;
        private set => SetProperty(ref _workSubject, value);
    }

    public string? WhatJustHappened
    {
        get => _whatJustHappened;
        private set => SetProperty(ref _whatJustHappened, value);
    }

    public string WhereWeAre => PlanningRegionGuidedPresentation.ComposeWhereWeAre();

    public string? AuthorizationSummary
    {
        get => _authorizationSummary;
        private set => SetProperty(ref _authorizationSummary, value);
    }

    public string? StopSummary
    {
        get => _stopSummary;
        private set => SetProperty(ref _stopSummary, value);
    }

    public string? NextStepSummary
    {
        get => _nextStepSummary;
        private set => SetProperty(ref _nextStepSummary, value);
    }

    public string? WhySummary
    {
        get => _whySummary;
        private set => SetProperty(ref _whySummary, value);
    }

    public string? PrimaryActionLabel
    {
        get => _primaryActionLabel;
        private set => SetProperty(ref _primaryActionLabel, value);
    }

    public bool HasPrimaryAction =>
        _currentStep == PlanningRegionGuidedStep.SendToProjectArchitect
        || (_currentStep == PlanningRegionGuidedStep.SendToEngineeringAgent && !ShowCopyEngineeringAgentHandover);

    public bool ShowCopyEngineeringAgentHandover =>
        _currentStep == PlanningRegionGuidedStep.SendToEngineeringAgent
        && !string.IsNullOrWhiteSpace(_transient.PreparedEaRenderedHandover);

    public bool ShowHumanReadableOutboundPackage => ShowCopyEngineeringAgentHandover;

    public string HumanReadableOutboundHeading =>
        PlanningRegionGuidedPresentation.ComposeHumanReadableOutboundHeading();

    public string CopyEngineeringAgentHandoverActionLabel =>
        PlanningRegionGuidedPresentation.ComposeCopyEngineeringAgentHandoverActionLabel();

    public string? HumanReadableOutboundPackageView
    {
        get => _humanReadableOutboundPackageView;
        private set => SetProperty(ref _humanReadableOutboundPackageView, value);
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

    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public bool ShowConfirmSubject => _requiresSubjectConfirmation;

    public bool ShowConfirmSessionContinuity => _currentStep == PlanningRegionGuidedStep.ConfirmSessionContinuity;

    public bool ShowBringBackPaResponse => _currentStep == PlanningRegionGuidedStep.BringBackPaResponse;

    public bool ShowBlockedByStop => _currentStep == PlanningRegionGuidedStep.BlockedByStop;

    public string PendingSubjectConfirmation
    {
        get => _pendingSubjectConfirmation;
        set => SetProperty(ref _pendingSubjectConfirmation, value);
    }

    public bool ImportAttestationConfirmed
    {
        get => _importAttestationConfirmed;
        set
        {
            if (SetProperty(ref _importAttestationConfirmed, value))
            {
                _transient.ImportAttestationConfirmed = value;
            }
        }
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

    public bool ShowCopyCorrectionRequest
    {
        get => _showCopyCorrectionRequest;
        private set => SetProperty(ref _showCopyCorrectionRequest, value);
    }

    public string? CorrectionNextStepHint
    {
        get => _correctionNextStepHint;
        private set => SetProperty(ref _correctionNextStepHint, value);
    }

    public string? TechnicalPackageDetail
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_transient.PreparedEaRenderedHandover))
            {
                return _transient.PreparedEaRenderedHandover;
            }

            return string.IsNullOrWhiteSpace(_transient.CachedRenderedReview)
                ? null
                : _transient.CachedRenderedReview;
        }
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

    public ICommand ConfirmSubjectCommand { get; }

    public ICommand ConfirmSessionContinuityCommand { get; }

    public ICommand PrimaryActionCommand { get; }

    public ICommand ValidatePaResponseCommand { get; }

    public ICommand CopyCorrectionRequestCommand { get; }

    public ICommand CopyEngineeringAgentHandoverCommand { get; }

    public ICommand ReturnToCurrentWorkCommand { get; }

    public void ActivatePlanningRegionGuided()
    {
        IsActive = true;
        EnsureWorkFocus();
        Refresh();
    }

    public void Deactivate()
    {
        IsActive = false;
        CurrentStep = PlanningRegionGuidedStep.Inactive;
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
            EnsureWorkFocus();
            Refresh();
        }
    }

    public PlanningRegionGuidedStep CurrentStep
    {
        get => _currentStep;
        private set => SetProperty(ref _currentStep, value);
    }

    internal void Refresh()
    {
        if (!IsActive || _workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        var projection = ResolveProjection(projectId);
        if (projection is null)
        {
            return;
        }

        var relay = _workflowServices.PlanningRegionRelayReadModel.Resolve(projectId);
        var session = _relayWorkflow.GetSessionState(projectId);
        var sessionReady = session.ProjectArchitectSessionIntent is not null
                           && session.EngineeringAgentSessionIntent is not null;

        if (relay.LatestPaReviewExport is not null
            && string.IsNullOrWhiteSpace(_transient.CachedRenderedReview))
        {
            _transient.CachedRenderedReview = PlanningRegionReviewExportRenderer.RenderCompleteClipboardPayload(
                relay.LatestPaReviewExport.Package);
        }

        if (relay.LatestConsumedPaHandover is not null
            && _transient.LastCommittedHandoverPackage is null)
        {
            _transient.LastCommittedHandoverPackage = relay.LatestConsumedPaHandover.Package;
            _transient.LastCommittedHandoverValidation = relay.LatestConsumedPaHandover.Validation;
        }

        var step = PlanningRegionGuidedStepResolver.Resolve(
            projection,
            relay,
            _transient,
            sessionReady,
            _requiresSubjectConfirmation);

        CurrentStep = step;
        WorkSubject = _activeFocus?.SubjectLabel;
        WhatJustHappened = _activeFocus?.LastContinuitySummary
                           ?? OperatorWorkContinuityPresenter.ComposeWhatJustHappened(projection, relay);
        AuthorizationSummary = PlanningRegionGuidedPresentation.ComposeAuthorizationSummary(projection);
        StopSummary = projection.CurrentWork.Count == 1 && projection.CurrentWork[0].StopActive
            ? "STOP is active. Guided progression is blocked until STOP is cleared."
            : "STOP is not blocking ordinary guided planning progression.";
        NextStepSummary = PlanningRegionGuidedPresentation.ComposePrimaryActionLabel(step);
        WhySummary = PlanningRegionGuidedPresentation.ComposeWhyNextStep(step);
        PrimaryActionLabel = PlanningRegionGuidedPresentation.ComposePrimaryActionLabel(step);

        ShowCopyCorrectionRequest = _transient.LastValidationAttemptFailed
                                    && PaHandoverExchangeCorrectionSupport.ShouldOfferCorrectionRequest(
                                        _transient.LastCorrectionFailureClass);
        CorrectionNextStepHint = ShowCopyCorrectionRequest
            ? GovernedRelayManualPasteOperatorMessages.ComposeCorrectionNextStepHint()
            : null;
        ValidationOperatorMessage = _transient.LastOperatorValidationMessage;

        InvalidatePreparedEaHandoverIfNeeded(step, projection, relay);
        SyncHumanReadableOutboundView();
        ApplyStepPresentation(step);
        RaisePropertyChanged(nameof(ShowConfirmSubject));
        RaisePropertyChanged(nameof(ShowConfirmSessionContinuity));
        RaisePropertyChanged(nameof(ShowBringBackPaResponse));
        RaisePropertyChanged(nameof(ShowBlockedByStop));
        RaisePropertyChanged(nameof(HasPrimaryAction));
        RaisePropertyChanged(nameof(TechnicalPackageDetail));
        RaisePropertyChanged(nameof(ShowCopyEngineeringAgentHandover));
        RaisePropertyChanged(nameof(ShowHumanReadableOutboundPackage));
        RaisePropertyChanged(nameof(HumanReadableOutboundHeading));
        RaiseAllCommandCanExecuteChanged();
    }

    private void ApplyStepPresentation(PlanningRegionGuidedStep step)
    {
        switch (step)
        {
            case PlanningRegionGuidedStep.ConfirmSubject:
                StepTitle = "Confirm what you are working on";
                StepBody =
                    "ProjectConcord needs a human-readable work subject before continuing. "
                    + "Enter a short label and confirm — this does not grant authorization.";
                break;
            case PlanningRegionGuidedStep.BlockedByStop:
                StepTitle = "STOP is active";
                StepBody = StopSummary;
                break;
            case PlanningRegionGuidedStep.ConfirmSessionContinuity:
                StepTitle = "Conversation continuity";
                StepBody =
                    "Select how your Project Architect and Engineering Agent conversations continue. "
                    + "Engineering Agent mode remains Plan for guided planning work.";
                HydrateSessionIntentSelections(_workspace.CurrentProjectId!.Value);
                break;
            case PlanningRegionGuidedStep.SendToProjectArchitect:
                StepTitle = "Send work to the Project Architect";
                StepBody =
                    "Use the primary action below to prepare and copy the governed review request. "
                    + "Paste it into your Project Architect conversation. Technical package detail is available under Advanced disclosure.";
                break;
            case PlanningRegionGuidedStep.BringBackPaResponse:
                StepTitle = "Bring back the Project Architect response";
                StepBody =
                    "Paste the complete response from your Project Architect. "
                    + "Confirm the attestation, then validate. ProjectConcord will not treat the paste as complete without your explicit attestation.";
                break;
            case PlanningRegionGuidedStep.SendToEngineeringAgent:
                StepTitle = "Send work to the Engineering Agent";
                StepBody =
                    "A qualifying Project Architect response is on record. "
                    + "Prepare the handover, review what will be sent in human-readable form, then copy the canonical governed package for manual transfer in Plan mode.";
                break;
            default:
                StepTitle = "Guided planning work";
                StepBody = "This guided surface is not active for the current workflow state.";
                break;
        }
    }

    private async Task ExecutePrimaryActionAsync()
    {
        if (_currentStep == PlanningRegionGuidedStep.SendToProjectArchitect)
        {
            await SendToProjectArchitectAsync().ConfigureAwait(true);
            return;
        }

        if (_currentStep == PlanningRegionGuidedStep.SendToEngineeringAgent)
        {
            await SendToEngineeringAgentAsync().ConfigureAwait(true);
        }
    }

    private async Task SendToProjectArchitectAsync()
    {
        StatusMessage = null;
        if (_workspace.CurrentProjectId is not { } projectId || _workspace.CurrentRoot is not { } root)
        {
            StatusMessage = "Open a project before sending work to the Project Architect.";
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
            new RelayPaReviewExportOptions(EngineeringAgentMode.Plan, null));

        if (result.RenderedPackage is null)
        {
            StatusMessage = "ProjectConcord could not prepare the review package. Confirm conversation continuity and try again.";
            AppendTechnicalDiagnostics(result.Validation.Diagnostics);
            Refresh();
            return;
        }

        _transient.CachedRenderedReview = result.RenderedPackage;
        await _copyTextAsync(result.RenderedPackage).ConfigureAwait(true);
        _transient.ReviewTransferred = true;
        StatusMessage = "Review copied. Paste it into your Project Architect conversation, then return here to bring back the response.";
        RecordContinuity(projectId, result.PackageId);
        RaisePropertyChanged(nameof(TechnicalPackageDetail));
        RaisePropertyChanged(nameof(ShowCopyEngineeringAgentHandover));
        RaisePropertyChanged(nameof(ShowHumanReadableOutboundPackage));
        RaisePropertyChanged(nameof(HumanReadableOutboundHeading));
        Refresh();
    }

    private async Task SendToEngineeringAgentAsync()
    {
        await PrepareEngineeringAgentHandoverAsync().ConfigureAwait(true);
    }

    private async Task PrepareEngineeringAgentHandoverAsync()
    {
        StatusMessage = null;
        var package = _transient.LastCommittedHandoverPackage
                        ?? _workflowServices.PlanningRegionRelayReadModel
                            .Resolve(_workspace.CurrentProjectId!.Value).LatestConsumedPaHandover?.Package;
        var validation = _transient.LastCommittedHandoverValidation
                         ?? _workflowServices.PlanningRegionRelayReadModel
                             .Resolve(_workspace.CurrentProjectId!.Value).LatestConsumedPaHandover?.Validation;

        if (package is null || validation is null)
        {
            StatusMessage = "A validated Project Architect response is required before sending work to the Engineering Agent.";
            Refresh();
            return;
        }

        var preparation = _relayWorkflow.PrepareEngineeringAgentHandover(package, validation);
        if (!preparation.IsReadyForManualTransfer
            || preparation.RenderedHandover is null
            || preparation.ExportPackage is null)
        {
            StatusMessage = "Engineering Agent handover is not ready for manual transfer.";
            AppendTechnicalDiagnostics(preparation.Validation.Diagnostics);
            _transient.ClearPreparedEaHandover();
            SyncHumanReadableOutboundView();
            Refresh();
            return;
        }

        _transient.ClearPreparedEaHandover();
        _transient.PreparedEaExportPackage = preparation.ExportPackage;
        _transient.PreparedEaRenderedHandover = preparation.RenderedHandover;
        _transient.CachedEngineeringHandover = preparation.RenderedHandover;
        _transient.PreparedEaSourceHandoverPackageId = package.PackageId.Value;
        _transient.PreparedEaHumanReadableView = GovernedRelayHumanReadablePackageProjector.Project(
            preparation.ExportPackage,
            new GovernedRelayHumanReadablePackageOptions(
                GovernedRelayHumanReadableCounterparty.EngineeringAgent,
                WorkSubject));

        StatusMessage =
            "Engineering Agent handover prepared. Review what will be sent, then copy the canonical governed package for manual transfer.";
        _onWorkStateMayHaveChanged?.Invoke();
        SyncHumanReadableOutboundView();
        RaisePropertyChanged(nameof(TechnicalPackageDetail));
        Refresh();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private async Task CopyEngineeringAgentHandoverAsync()
    {
        if (string.IsNullOrWhiteSpace(_transient.PreparedEaRenderedHandover))
        {
            return;
        }

        await _copyTextAsync(_transient.PreparedEaRenderedHandover).ConfigureAwait(true);
        StatusMessage =
            "Canonical governed package copied for manual transfer to the Engineering Agent (Plan mode).";
        Refresh();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private void SyncHumanReadableOutboundView()
    {
        HumanReadableOutboundPackageView = _transient.PreparedEaHumanReadableView;
    }

    private void InvalidatePreparedEaHandoverIfNeeded(
        PlanningRegionGuidedStep step,
        GovernedWorkStateOperatorProjection projection,
        PlanningRegionRelayReadModelSnapshot relay)
    {
        if (_transient.PreparedEaExportPackage is null)
        {
            return;
        }

        if (step != PlanningRegionGuidedStep.SendToEngineeringAgent)
        {
            _transient.ClearPreparedEaHandover();
            return;
        }

        if (projection.CurrentWork.Count == 1 && projection.CurrentWork[0].StopActive)
        {
            _transient.ClearPreparedEaHandover();
            return;
        }

        var sourcePackage = _transient.LastCommittedHandoverPackage ?? relay.LatestConsumedPaHandover?.Package;
        if (sourcePackage is null
            || _transient.PreparedEaSourceHandoverPackageId != sourcePackage.PackageId.Value)
        {
            _transient.ClearPreparedEaHandover();
        }
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

        if (!ImportAttestationConfirmed)
        {
            ValidationOperatorMessage =
                "Confirm that you pasted the complete Project Architect response before validating.";
            return;
        }

        if (string.IsNullOrWhiteSpace(PaResponseDraft))
        {
            ValidationOperatorMessage = "Paste the complete Project Architect response first.";
            return;
        }

        var relay = _workflowServices.PlanningRegionRelayReadModel.Resolve(projectId);
        var requiredCorrelation = relay.LatestPaReviewExport?.Package.CorrelationId;

        var result = _relayWorkflow.TryValidatePaHandoverImport(
            projectId,
            PaResponseDraft,
            requiredCorrelation);

        if (!result.ProjectIdMatched)
        {
            RecordValidationFailure(
                PaHandoverCorrectionFailureClass.ProjectIdentityMismatch,
                GovernedRelayManualPasteOperatorMessages.ComposeImportFailureOperatorMessage(
                    result.Import.Validation,
                    offersCorrectionRequest: true),
                result.ProjectIdMismatchMessage);
            Refresh();
            return;
        }

        if (!PaHandoverExchangeCorrectionSupport.IsAuthorizedForGuidedDurableConsumption(result))
        {
            var failureClass = PaHandoverExchangeCorrectionSupport.ClassifyValidationFailure(result);
            var offersCorrection = PaHandoverExchangeCorrectionSupport.ShouldOfferCorrectionRequest(failureClass);
            RecordValidationFailure(
                failureClass,
                GovernedRelayManualPasteOperatorMessages.ComposeImportFailureOperatorMessage(
                    result.Import.Validation,
                    offersCorrection),
                FormatValidationDetail(result.Import.Validation));
            AppendTechnicalDiagnostics(result.Import.Validation.Diagnostics);
            Refresh();
            return;
        }

        _relayWorkflow.CommitConsumedPaHandoverImport(
            projectId,
            result.Import.Package,
            result.Import.Validation);

        _transient.ClearPreparedEaHandover();
        _transient.LastCommittedHandoverPackage = result.Import.Package;
        _transient.LastCommittedHandoverValidation = result.Import.Validation;
        _transient.LastValidationAttemptFailed = false;
        _transient.LastCorrectionFailureClass = PaHandoverCorrectionFailureClass.None;
        _transient.LastOperatorValidationMessage = null;
        ValidationOperatorMessage = null;
        StatusMessage = "Project Architect response validated and recorded.";
        RecordContinuity(projectId, result.Import.Package!.PackageId);
        _onWorkStateMayHaveChanged?.Invoke();
        Refresh();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private void RecordValidationFailure(
        PaHandoverCorrectionFailureClass failureClass,
        string operatorMessage,
        string? technicalDetail)
    {
        _transient.LastValidationAttemptFailed = true;
        _transient.LastCorrectionFailureClass = failureClass;
        _transient.LastOperatorValidationMessage = operatorMessage;
        ValidationOperatorMessage = operatorMessage;
        TechnicalValidationDetail = technicalDetail;
    }

    private async Task CopyCorrectionRequestAsync()
    {
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        var relay = _workflowServices.PlanningRegionRelayReadModel.Resolve(projectId);
        var review = relay.LatestPaReviewExport?.Package;
        if (review is null)
        {
            StatusMessage = "A prepared review is required before copying a correction request.";
            Refresh();
            return;
        }

        var request = GovernedRelayPaHandoverCorrectionRequest.RenderCompleteCorrectionRequest(
            review,
            PaHandoverResponseProfile.PlanningEntry,
            _transient.LastCorrectionFailureClass);

        await _copyTextAsync(request).ConfigureAwait(true);
        StatusMessage = "Correction request copied.";
        Refresh();
    }

    private void ConfirmSubject()
    {
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        var projection = ResolveProjection(projectId);
        if (projection is null)
        {
            return;
        }

        var ensure = _workflowServices.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(
            projectId,
            projection,
            _projectDisplayNameProvider(),
            PendingSubjectConfirmation);

        if (!ensure.IsSuccess || ensure.RequiredOperatorSubjectConfirmation)
        {
            StatusMessage = "Enter and confirm a work subject label to continue.";
            return;
        }

        _activeFocus = ensure.Focus;
        _requiresSubjectConfirmation = false;
        StatusMessage = null;
        Refresh();
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
        _transient.ClearPreparedEaHandover();
        _relayWorkflow.SetEngineeringAgentSessionIntent(projectId, eaIntent);
        StatusMessage = "Conversation continuity recorded.";
        Refresh();
    }

    private void EnsureWorkFocus()
    {
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            return;
        }

        var projection = ResolveProjection(projectId);
        if (projection is null)
        {
            return;
        }

        var ensure = _workflowServices.OperatorWorkFocus.EnsurePlanningRegionWorkFocus(
            projectId,
            projection,
            _projectDisplayNameProvider());

        if (!ensure.IsSuccess)
        {
            StatusMessage = ensure.FailureMessage;
            return;
        }

        _requiresSubjectConfirmation = ensure.RequiredOperatorSubjectConfirmation;
        _activeFocus = ensure.Focus;
        if (_activeFocus is not null)
        {
            var relay = _workflowServices.PlanningRegionRelayReadModel.Resolve(projectId);
            var continuity = OperatorWorkContinuityPresenter.ComposeRecentContinuityForPersistence(projection, relay);
            _workflowServices.OperatorWorkFocus.RecordContinuitySnapshot(
                _activeFocus,
                continuity,
                relay.LatestPaReviewExport?.Package.PackageId.Value);
            _activeFocus = _workflowServices.OperatorWorkFocus.GetActive(projectId);
        }
    }

    private void RecordContinuity(ProjectConcordProjectId projectId, GovernedPackageId? packageId = null)
    {
        var focus = _workflowServices.OperatorWorkFocus.GetActive(projectId);
        if (focus is null)
        {
            return;
        }

        var projection = ResolveProjection(projectId);
        if (projection is null)
        {
            return;
        }

        var relay = _workflowServices.PlanningRegionRelayReadModel.Resolve(projectId);
        var continuity = OperatorWorkContinuityPresenter.ComposeRecentContinuityForPersistence(projection, relay);
        _workflowServices.OperatorWorkFocus.RecordContinuitySnapshot(focus, continuity, packageId?.Value);
        _activeFocus = _workflowServices.OperatorWorkFocus.GetActive(projectId);
        WhatJustHappened = continuity;
    }

    private GovernedWorkStateOperatorProjection? ResolveProjection(ProjectConcordProjectId projectId)
    {
        var rootPath = _workspace.CurrentRoot?.AbsolutePath;
        return _workflowServices.WorkStateOperatorProjection.ProjectForProject(projectId, rootPath);
    }

    private void HydrateSessionIntentSelections(ProjectConcordProjectId projectId)
    {
        var session = _relayWorkflow.GetSessionState(projectId);
        SelectedPaSessionIntent = session.ProjectArchitectSessionIntent ?? AgentSessionIntent.Continue;
        SelectedEaSessionIntent = session.EngineeringAgentSessionIntent ?? AgentSessionIntent.Continue;
    }

    private void AppendTechnicalDiagnostics(IReadOnlyList<RelayValidationDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            TechnicalDiagnostics.Add(diagnostic.Message);
        }
    }

    private static string FormatValidationDetail(RelayValidationResult validation)
    {
        if (validation.Diagnostics.Count == 0)
        {
            return validation.State.ToString();
        }

        var builder = new StringBuilder();
        foreach (var diagnostic in validation.Diagnostics)
        {
            builder.AppendLine(diagnostic.Message);
        }

        return builder.ToString().Trim();
    }

    private void RaiseAllCommandCanExecuteChanged()
    {
        (ConfirmSubjectCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (ConfirmSessionContinuityCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (PrimaryActionCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ValidatePaResponseCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (CopyCorrectionRequestCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (CopyEngineeringAgentHandoverCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ReturnToCurrentWorkCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
}
