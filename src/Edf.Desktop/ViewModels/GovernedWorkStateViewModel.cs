using System.Collections.ObjectModel;
using System.Windows.Input;
using Edf.Application.Composition;
using Edf.Application.Operator.WorkState;
using Edf.Application.Projects;
using Edf.Application.Relay;
using Edf.Application.Workflow;
using Edf.Application.Workflow.Eligibility;
using Edf.Application.Operator.PlanningAuthorization;
using Edf.Application.Operator.PlanningEntry;
using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Desktop.ViewModels;

public sealed class GovernedWorkStateViewModel : ViewModelBase
{
    private readonly IProjectWorkspaceService _workspace;
    private readonly GovernedWorkStateOperatorProjectionService _projection;
    private readonly IWorkflowInstanceService _workflowInstances;
    private readonly IGewV1IntakePlanningEntryTransitionService _planningEntryTransitions;
    private readonly IPlanningEntryRelayReadModel _planningEntryRelayReadModel;
    private readonly Func<(GovernedRelayPackage? Package, RelayValidationResult? Validation)>? _consumedPaHandoverProvider;
    private readonly Action? _launchPlanningEntryGuidedExchange;
    private readonly Action? _launchPlanningAuthorizationGuidedExchange;
    private bool _isSectionEnabled;
    private string? _statusMessage;
    private string? _fullyGovernedFrontierLine;
    private string? _candidateFrontierLine;
    private string? _waitingOnAvailabilityLine;
    private string? _workflowNextActionAvailabilityLine;
    private string? _operatorSituationHeadline;
    private string? _operatorSituationSummary;
    private string? _operatorNextStepSummary;
    private bool _suppressGenericNextStepSummary;
    private bool _canOpenExchange;
    private bool _canEnterGovernedPlanning;
    private bool _canObtainPlanningAuthorization;
    private string? _planningEntryActionExplanation;
    private string? _planningAuthorizationActionExplanation;

    public GovernedWorkStateViewModel(
        IProjectWorkspaceService workspace,
        WorkflowApplicationServices workflowServices)
        : this(workspace, workflowServices, null, null)
    {
    }

    public GovernedWorkStateViewModel(
        IProjectWorkspaceService workspace,
        WorkflowApplicationServices workflowServices,
        Func<(GovernedRelayPackage? Package, RelayValidationResult? Validation)>? consumedPaHandoverProvider,
        Action? openExchangeTab,
        Action? openPlanningAuthorizationExchange = null)
    {
        _workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        ArgumentNullException.ThrowIfNull(workflowServices);
        _projection = workflowServices.WorkStateOperatorProjection;
        _workflowInstances = workflowServices.WorkflowInstances;
        _planningEntryTransitions = workflowServices.IntakePlanningEntryTransitions;
        _planningEntryRelayReadModel = workflowServices.PlanningEntryRelayReadModel;
        _consumedPaHandoverProvider = consumedPaHandoverProvider;
        _launchPlanningEntryGuidedExchange = openExchangeTab;
        _launchPlanningAuthorizationGuidedExchange = openPlanningAuthorizationExchange;

        CurrentWorkItems = new ObservableCollection<GovernedWorkStateCurrentWorkItemViewModel>();
        WaitingOnItems = new ObservableCollection<GovernedWorkStateWaitingOnItemViewModel>();
        WorkflowNextActions = new ObservableCollection<GovernedWorkStateNextActionItemViewModel>();

        StartGewBootstrapCommand = new AsyncRelayCommand(StartGewBootstrapAsync, () => CanStartGewBootstrap);
        OpenExchangeCommand = new RelayCommand(OpenExchange, () => CanOpenExchange);
        EnterGovernedPlanningCommand = new AsyncRelayCommand(EnterGovernedPlanningAsync, () => CanEnterGovernedPlanning);
        ObtainPlanningAuthorizationCommand = new RelayCommand(
            OpenPlanningAuthorizationExchange,
            () => CanObtainPlanningAuthorization);
    }

    public ObservableCollection<GovernedWorkStateCurrentWorkItemViewModel> CurrentWorkItems { get; }

    public ObservableCollection<GovernedWorkStateWaitingOnItemViewModel> WaitingOnItems { get; }

    public ObservableCollection<GovernedWorkStateNextActionItemViewModel> WorkflowNextActions { get; }

    public ICommand StartGewBootstrapCommand { get; }

    public ICommand OpenExchangeCommand { get; }

    public ICommand EnterGovernedPlanningCommand { get; }

    public ICommand ObtainPlanningAuthorizationCommand { get; }

    public bool SuppressGenericNextStepSummary
    {
        get => _suppressGenericNextStepSummary;
        private set => SetProperty(ref _suppressGenericNextStepSummary, value);
    }

    public bool CanOpenExchange
    {
        get => _canOpenExchange;
        private set
        {
            if (SetProperty(ref _canOpenExchange, value))
            {
                (OpenExchangeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool CanEnterGovernedPlanning
    {
        get => _canEnterGovernedPlanning;
        private set
        {
            if (SetProperty(ref _canEnterGovernedPlanning, value))
            {
                (EnterGovernedPlanningCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string? PlanningEntryActionExplanation
    {
        get => _planningEntryActionExplanation;
        private set => SetProperty(ref _planningEntryActionExplanation, value);
    }

    public bool CanObtainPlanningAuthorization
    {
        get => _canObtainPlanningAuthorization;
        private set
        {
            if (SetProperty(ref _canObtainPlanningAuthorization, value))
            {
                (ObtainPlanningAuthorizationCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string? PlanningAuthorizationActionExplanation
    {
        get => _planningAuthorizationActionExplanation;
        private set => SetProperty(ref _planningAuthorizationActionExplanation, value);
    }

    public string BoundaryNotice =>
        "Workflow projections show durable governed work state. Relay package validation is separate and does not grant DevelopmentWorkAuthorization.";

    public bool IsSectionEnabled
    {
        get => _isSectionEnabled;
        private set
        {
            if (SetProperty(ref _isSectionEnabled, value))
            {
                RaisePropertyChanged(nameof(CanStartGewBootstrap));
                (StartGewBootstrapCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasCurrentWorkItems => CurrentWorkItems.Count > 0;

    public bool CanStartGewBootstrap => IsSectionEnabled && !HasCurrentWorkItems;

    public string EmptyStateMessage =>
        IsSectionEnabled
            ? "No active workflow instances. Use Start GEW — Standard profile to begin operator-initiated bootstrap (not authorization)."
            : "Open a project to view Current Work.";

    public string? StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public string? FullyGovernedFrontierLine
    {
        get => _fullyGovernedFrontierLine;
        private set => SetProperty(ref _fullyGovernedFrontierLine, value);
    }

    public string? CandidateFrontierLine
    {
        get => _candidateFrontierLine;
        private set => SetProperty(ref _candidateFrontierLine, value);
    }

    public string? WaitingOnAvailabilityLine
    {
        get => _waitingOnAvailabilityLine;
        private set => SetProperty(ref _waitingOnAvailabilityLine, value);
    }

    public string? WorkflowNextActionAvailabilityLine
    {
        get => _workflowNextActionAvailabilityLine;
        private set => SetProperty(ref _workflowNextActionAvailabilityLine, value);
    }

    public string? OperatorSituationHeadline
    {
        get => _operatorSituationHeadline;
        private set => SetProperty(ref _operatorSituationHeadline, value);
    }

    public string? OperatorSituationSummary
    {
        get => _operatorSituationSummary;
        private set => SetProperty(ref _operatorSituationSummary, value);
    }

    public string? OperatorNextStepSummary
    {
        get => _operatorNextStepSummary;
        private set => SetProperty(ref _operatorNextStepSummary, value);
    }

    public bool HasOperatorNextStepSummary =>
        !string.IsNullOrWhiteSpace(OperatorNextStepSummary);

    public bool HasWaitingOnItems => WaitingOnItems.Count > 0;

    internal void OnActiveProjectChanged(ProjectConcordProjectId? projectId, bool hasActiveProject)
    {
        IsSectionEnabled = hasActiveProject && projectId is not null;
        StatusMessage = null;
        RefreshFromProjection();
    }

    internal void RefreshFromProjection()
    {
        CurrentWorkItems.Clear();
        WaitingOnItems.Clear();
        WorkflowNextActions.Clear();
        FullyGovernedFrontierLine = null;
        CandidateFrontierLine = null;
        WaitingOnAvailabilityLine = null;
        WorkflowNextActionAvailabilityLine = null;
        OperatorSituationHeadline = null;
        OperatorSituationSummary = null;
        OperatorNextStepSummary = null;
        SuppressGenericNextStepSummary = false;
        CanOpenExchange = false;
        CanEnterGovernedPlanning = false;
        CanObtainPlanningAuthorization = false;
        PlanningEntryActionExplanation = null;
        PlanningAuthorizationActionExplanation = null;

        if (!IsSectionEnabled || _workspace.CurrentProjectId is not { } projectId)
        {
            RaisePropertyChanged(nameof(HasCurrentWorkItems));
            RaisePropertyChanged(nameof(HasWaitingOnItems));
            RaisePropertyChanged(nameof(HasOperatorNextStepSummary));
            RaisePropertyChanged(nameof(CanStartGewBootstrap));
            RaisePropertyChanged(nameof(EmptyStateMessage));
            (StartGewBootstrapCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            return;
        }

        var rootPath = _workspace.CurrentRoot?.AbsolutePath;
        var projection = _projection.ProjectForProject(projectId, rootPath);

        foreach (var item in projection.CurrentWork)
        {
            CurrentWorkItems.Add(GovernedWorkStateCurrentWorkItemViewModel.FromProjection(item));
        }

        foreach (var waiting in projection.WaitingOnItems)
        {
            WaitingOnItems.Add(GovernedWorkStateWaitingOnItemViewModel.FromProjection(waiting));
        }

        WorkflowOperatorNextActionItem? primaryNextAction = null;
        foreach (var action in projection.WorkflowNextActions)
        {
            primaryNextAction ??= action;
            WorkflowNextActions.Add(GovernedWorkStateNextActionItemViewModel.FromProjection(action));
        }

        FullyGovernedFrontierLine = FormatFullyGovernedFrontier(projection);
        CandidateFrontierLine = FormatCandidateFrontier(projection);
        WaitingOnAvailabilityLine = FormatAvailability(
            "Waiting On",
            projection.WaitingOnAvailability,
            projection.WaitingOnUnavailableReason);
        WorkflowNextActionAvailabilityLine = FormatAvailability(
            "Workflow suggested next action",
            projection.NextActionAvailability,
            projection.NextActionUnavailableReason);

        if (CurrentWorkItems.Count > 0)
        {
            var primary = CurrentWorkItems[0];
            OperatorSituationHeadline = CurrentWorkItems.Count == 1
                ? primary.OperatorHeadline
                : $"{CurrentWorkItems.Count} active workflow instances — {primary.OperatorHeadline}";
            OperatorSituationSummary = CurrentWorkItems.Count == 1
                ? primary.OperatorSituationSummary
                : primary.OperatorSituationSummary + " See each instance below for full detail.";
        }

        if (primaryNextAction is not null && !SuppressGenericNextStepSummary)
        {
            OperatorNextStepSummary = GovernedWorkStatePresentation.ComposeNextStepOperatorSummary(primaryNextAction);
        }

        ApplyPlanningEntryOperatorPresentation(projectId, projection, primaryNextAction);

        RaisePropertyChanged(nameof(HasCurrentWorkItems));
        RaisePropertyChanged(nameof(HasWaitingOnItems));
        RaisePropertyChanged(nameof(HasOperatorNextStepSummary));
        RaisePropertyChanged(nameof(CanStartGewBootstrap));
        RaisePropertyChanged(nameof(EmptyStateMessage));
        (StartGewBootstrapCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (OpenExchangeCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (EnterGovernedPlanningCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        (ObtainPlanningAuthorizationCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void ApplyPlanningEntryOperatorPresentation(
        ProjectConcordProjectId projectId,
        GovernedWorkStateOperatorProjection projection,
        WorkflowOperatorNextActionItem? primaryNextAction)
    {
        if (CurrentWorkItems.Count != 1)
        {
            return;
        }

        var primaryItem = projection.CurrentWork[0];
        var place = primaryItem.TopologyPlaceId.Value;

        if (place == GewV1TopologyPlaces.PlanningGoverned)
        {
            OperatorSituationHeadline = "Where you are: Governed Planning";
            OperatorSituationSummary = GovernedWorkStatePresentation.ComposeGovernedPlanningSituationSummary(primaryItem);
            var needsPlanningDwa = PlanningAuthorizationWorkStateFacts.InstanceNeedsPlanningDevelopmentWorkAuthorization(
                projection);
            if (needsPlanningDwa)
            {
                SuppressGenericNextStepSummary = true;
                OperatorNextStepSummary = GovernedWorkStatePresentation.ComposeObtainPlanningAuthorizationNextStepSummary();
                CanObtainPlanningAuthorization = true;
                PlanningAuthorizationActionExplanation =
                    "Start here to complete the governed authorization steps in ProjectConcord.";
            }
            else
            {
                SuppressGenericNextStepSummary = false;
                if (primaryNextAction is not null)
                {
                    OperatorNextStepSummary = GovernedWorkStatePresentation.ComposeNextStepOperatorSummary(primaryNextAction);
                }
            }

            return;
        }

        if (place != GewV1TopologyPlaces.Intake)
        {
            return;
        }

        var (package, validation) = ResolveConsumedPaHandover(projectId);
        var eligibility = _planningEntryTransitions.EvaluateEligibility(projectId, package, validation);

        OperatorSituationHeadline = "Where you are: Intake";
        SuppressGenericNextStepSummary = true;
        OperatorNextStepSummary = null;

        if (eligibility.IsEligible)
        {
            OperatorSituationSummary = GovernedWorkStatePresentation.ComposeIntakeWithQualifyingHandoverSummary();
            CanEnterGovernedPlanning = true;
            PlanningEntryActionExplanation =
                "Enter Governed Planning records the topology transition using relay provenance from the imported PA handover.";
            return;
        }

        OperatorSituationSummary = GovernedWorkStatePresentation.ComposeIntakeAwaitingPaApprovalSummary();
        CanOpenExchange = true;
        PlanningEntryActionExplanation =
            "Get Project Architect approval using the guided exchange. ProjectConcord will walk you through each step.";
    }

    private void OpenExchange()
    {
        _launchPlanningEntryGuidedExchange?.Invoke();
    }

    private void OpenPlanningAuthorizationExchange()
    {
        _launchPlanningAuthorizationGuidedExchange?.Invoke();
    }

    private async Task EnterGovernedPlanningAsync()
    {
        StatusMessage = null;
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            StatusMessage = "Open a project before entering governed planning.";
            return;
        }

        var (package, validation) = ResolveConsumedPaHandover(projectId);
        if (package is null || validation is null)
        {
            StatusMessage = "Validate a qualifying Project Architect response in the guided exchange first.";
            RefreshFromProjection();
            return;
        }

        var result = _planningEntryTransitions.TryEnterGovernedPlanning(projectId, package, validation);
        StatusMessage = result.OperatorMessage;
        if (result.Outcome == IntakePlanningEntryTransitionOutcome.Succeeded
            || result.Outcome == IntakePlanningEntryTransitionOutcome.AlreadyApplied)
        {
            RefreshFromProjection();
        }
        else
        {
            CanEnterGovernedPlanning = false;
            (EnterGovernedPlanningCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
        }

        await Task.CompletedTask.ConfigureAwait(true);
    }

    private (GovernedRelayPackage? Package, RelayValidationResult? Validation) ResolveConsumedPaHandover(
        ProjectConcordProjectId projectId)
    {
        var durable = _planningEntryRelayReadModel.Resolve(projectId).LatestConsumedPaHandover;
        if (durable is not null)
        {
            return (durable.Package, durable.Validation);
        }

        return _consumedPaHandoverProvider?.Invoke() ?? default;
    }

    private async Task StartGewBootstrapAsync()
    {
        StatusMessage = null;
        if (_workspace.CurrentProjectId is not { } projectId)
        {
            StatusMessage = "Open a project before starting a governed workflow instance.";
            return;
        }

        if (HasCurrentWorkItems)
        {
            StatusMessage = "Active workflow instances already exist.";
            RefreshFromProjection();
            return;
        }

        var rootPath = _workspace.CurrentRoot?.AbsolutePath;
        _workflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            rootPath,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));

        StatusMessage = "GEW instance created (Standard profile). This bootstrap does not grant implementation authorization.";
        RefreshFromProjection();
        await Task.CompletedTask.ConfigureAwait(true);
    }

    private static string FormatFullyGovernedFrontier(GovernedWorkStateOperatorProjection projection)
    {
        if (projection.FullyGovernedFrontierAvailability == ProjectionAvailability.Available)
        {
            return "Fully governed permission: additional evaluation reported available (not asserted in current scope).";
        }

        return "Fully governed permission is not determined from the current evaluation scope. "
               + "This is not full governed authorization.";
    }

    private static string FormatCandidateFrontier(GovernedWorkStateOperatorProjection projection)
    {
        if (projection.CandidateFrontierInstanceIds.Count == 0)
        {
            return "Candidate frontier: none under evaluated constraints (eligibility only — not authorization).";
        }

        var labels = string.Join(
            ", ",
            projection.CandidateFrontierInstanceIds.Select(i => GovernedWorkStatePresentation.ShortInstanceLabel(i.Value)));
        return $"Candidate frontier instances: {labels} (evaluated eligibility only — not authorization).";
    }

    private static string FormatAvailability(string label, ProjectionAvailability availability, string? reason)
    {
        if (availability == ProjectionAvailability.Available)
        {
            return $"{label}: information available.";
        }

        var context = label.ToLowerInvariant();
        return GovernedWorkStatePresentation.DescribeAvailabilityUnavailable(reason, context);
    }
}

public sealed class GovernedWorkStateCurrentWorkItemViewModel
{
    private GovernedWorkStateCurrentWorkItemViewModel(
        string instanceLabel,
        string operatorHeadline,
        string operatorSituationSummary,
        string technicalDetail,
        string workflowSummary,
        string placeSummary,
        string baselineLine,
        string stopLine,
        string dependencyLine,
        string effectiveConfigLine,
        string applicableDwaLine,
        string actionabilityLine,
        string eligibilitySummary)
    {
        InstanceLabel = instanceLabel;
        OperatorHeadline = operatorHeadline;
        OperatorSituationSummary = operatorSituationSummary;
        TechnicalDetail = technicalDetail;
        WorkflowSummary = workflowSummary;
        PlaceSummary = placeSummary;
        BaselineLine = baselineLine;
        StopLine = stopLine;
        DependencyLine = dependencyLine;
        EffectiveConfigLine = effectiveConfigLine;
        ApplicableDwaLine = applicableDwaLine;
        ActionabilityLine = actionabilityLine;
        EligibilitySummary = eligibilitySummary;
    }

    public string InstanceLabel { get; }

    public string OperatorHeadline { get; }

    public string OperatorSituationSummary { get; }

    public string TechnicalDetail { get; }

    public string WorkflowSummary { get; }

    public string PlaceSummary { get; }

    public string BaselineLine { get; }

    public string StopLine { get; }

    public string DependencyLine { get; }

    public string EffectiveConfigLine { get; }

    public string ApplicableDwaLine { get; }

    public string ActionabilityLine { get; }

    public string EligibilitySummary { get; }

    internal static GovernedWorkStateCurrentWorkItemViewModel FromProjection(GovernedWorkStateCurrentWorkItem item)
    {
        var profileValue = item.ProfileId is { } p && !p.IsEmpty ? p.Value : null;
        var profileLabel = profileValue switch
        {
            GewV1ProfileIds.Standard => "Standard profile",
            GewV1ProfileIds.Accelerated => "Accelerated profile",
            GewV1ProfileIds.HighAssurance => "High assurance profile",
            null => "Profile not set",
            _ => "Workflow profile",
        };

        var baseline = item.StoredGovernedBaseline.Kind == GovernedBaselineKind.GitCommit
            ? $"Governed baseline: git commit {item.StoredGovernedBaseline.Value}"
            : $"Governed baseline: {item.StoredGovernedBaseline.Kind} {item.StoredGovernedBaseline.Value}";

        var drift = item.HeadDriftsFromStoredBaseline switch
        {
            true => "Repository HEAD differs from stored baseline.",
            false => "Repository HEAD matches stored baseline.",
            null => "Baseline drift: not evaluated for this project root.",
        };

        var dwaKinds = item.ApplicableActiveAuthorizationKinds.Count == 0
            ? "None recorded in durable workflow authorization store"
            : string.Join(", ", item.ApplicableActiveAuthorizationKinds);

        var actionability = item.GovernedEligibility.FullyGovernedActionability;
        var actionabilityLine = actionability == GovernedActionabilityCompleteness.Indeterminate
            ? "Fully governed actionability: indeterminate (additional dimensions unevaluated)."
            : $"Fully governed actionability: {actionability}.";

        var violations = item.GovernedEligibility.EvaluatedConstraintViolationCodes.Count == 0
            ? "No evaluated blockers on this instance."
            : "Evaluated blockers: "
              + string.Join(
                  "; ",
                  item.GovernedEligibility.EvaluatedConstraintViolationCodes.Select(
                      GovernedWorkStatePresentation.DescribeBlockerCode));

        var dependencyLine = item.DependencyBlocked
            ? "Blocked by workflow dependency on other instance(s)."
            : "Not blocked by workflow dependencies.";

        var effectiveConfigLine = item.EffectiveConfigurationAvailability == ProjectionAvailability.Available
            ? "Effective configuration: resolved."
            : GovernedWorkStatePresentation.DescribeEffectiveConfigUnavailable(
                item.EffectiveConfigurationUnavailableReason);

        var technicalDetail =
            $"Instance ID: {item.InstanceId.Value:D}\r\n"
            + $"Profile ID: {profileValue ?? "(none)"}\r\n"
            + $"Place ID: {item.TopologyPlaceId.Value}\r\n"
            + $"Actionability reason: {item.GovernedEligibility.FullyGovernedActionabilityReasonCode}\r\n"
            + (item.DependencyBlocked
                ? "Required instances: "
                  + string.Join(", ", item.UnresolvedRequiredWorkflowInstanceIds.Select(i => i.Value))
                  + "\r\n"
                : string.Empty)
            + (item.GovernedEligibility.EvaluatedConstraintViolationCodes.Count > 0
                ? "Blocker codes: " + string.Join(", ", item.GovernedEligibility.EvaluatedConstraintViolationCodes)
                : string.Empty);

        var operatorHeadline = GovernedWorkStatePresentation.ComposeInstanceHeadline(
            profileValue,
            item.TopologyPlaceId.Value);
        var operatorSituation = GovernedWorkStatePresentation.ComposeInstanceSituationSummary(item);
        var placeSummary = $"Stage: {GovernedWorkStatePresentation.DescribeTopologyPlace(item.TopologyPlaceId.Value)}";

        return new GovernedWorkStateCurrentWorkItemViewModel(
            GovernedWorkStatePresentation.ShortInstanceLabel(item.InstanceId.Value),
            operatorHeadline,
            operatorSituation,
            technicalDetail.Trim(),
            $"Governed Engineering Workflow — {profileLabel}",
            placeSummary,
            $"{baseline} {drift}",
            item.StopActive ? "STOP is active on this instance." : "STOP is not active on this instance.",
            dependencyLine,
            effectiveConfigLine,
            $"Durable development work authorization kinds: {dwaKinds}",
            actionabilityLine,
            violations);
    }
}

public sealed class GovernedWorkStateWaitingOnItemViewModel
{
    private GovernedWorkStateWaitingOnItemViewModel(string displayLine, string technicalDetail)
    {
        DisplayLine = displayLine;
        TechnicalDetail = technicalDetail;
    }

    public string DisplayLine { get; }

    public string TechnicalDetail { get; }

    internal static GovernedWorkStateWaitingOnItemViewModel FromProjection(GovernedWorkStateWaitingOnItem item)
    {
        var summary = GovernedWorkStatePresentation.DescribeWaitingOnReason(item.ReasonCode);
        var instanceLabel = GovernedWorkStatePresentation.ShortInstanceLabel(item.WorkflowInstanceId.Value);
        var technical =
            $"Instance ID: {item.WorkflowInstanceId.Value:D}\r\n"
            + $"Reason code: {item.ReasonCode}\r\n"
            + (item.RelatedWorkflowInstanceIds.Count > 0
                ? "Related instances: "
                  + string.Join(", ", item.RelatedWorkflowInstanceIds.Select(i => i.Value))
                : string.Empty);
        return new GovernedWorkStateWaitingOnItemViewModel(
            $"Instance {instanceLabel}: {summary}",
            technical.Trim());
    }
}

public sealed class GovernedWorkStateNextActionItemViewModel
{
    private GovernedWorkStateNextActionItemViewModel(
        string displayLine,
        string permissionLine,
        string operatorPlainSummary,
        string technicalDetail)
    {
        DisplayLine = displayLine;
        PermissionLine = permissionLine;
        OperatorPlainSummary = operatorPlainSummary;
        TechnicalDetail = technicalDetail;
    }

    public string DisplayLine { get; }

    public string PermissionLine { get; }

    public string OperatorPlainSummary { get; }

    public string TechnicalDetail { get; }

    internal static GovernedWorkStateNextActionItemViewModel FromProjection(WorkflowOperatorNextActionItem item)
    {
        var permissionLine =
            $"Permission to execute: {item.PermissionToExecute} (suggested step — not authorization)";
        var step = GovernedWorkStatePresentation.DescribeNextStepCode(item.SuggestedStepCode);
        var instanceLabel = GovernedWorkStatePresentation.ShortInstanceLabel(item.WorkflowInstanceId.Value);
        var line = $"Instance {instanceLabel}: {step}. {item.Message}";
        var technical =
            $"Instance ID: {item.WorkflowInstanceId.Value:D}\r\n"
            + $"Step code: {item.SuggestedStepCode}\r\n"
            + $"Action class: {item.ActionClass}";
        var operatorPlain = GovernedWorkStatePresentation.ComposeNextStepOperatorSummary(item);
        return new GovernedWorkStateNextActionItemViewModel(line, permissionLine, operatorPlain, technical);
    }
}
