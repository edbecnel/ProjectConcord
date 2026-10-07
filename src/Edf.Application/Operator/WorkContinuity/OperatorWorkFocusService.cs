namespace Edf.Application.Operator.WorkContinuity;

using Edf.Application.Operator.WorkState;
using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;

public sealed class OperatorWorkFocusService
{
    private readonly IOperatorWorkFocusStore _store;

    public OperatorWorkFocusService(IOperatorWorkFocusStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public OperatorActiveWorkFocus? GetActive(ProjectConcordProjectId projectId) =>
        _store.GetActiveForProject(projectId);

    public OperatorWorkFocusEnsureResult EnsurePlanningRegionWorkFocus(
        ProjectConcordProjectId projectId,
        GovernedWorkStateOperatorProjection projection,
        string? projectDisplayName,
        string? operatorConfirmedSubjectLabel = null)
    {
        // Follow-up (non-blocking): reconciling WorkflowInstanceId when projection instance changes is not required for this tranche.
        var existing = _store.GetActiveForProject(projectId);
        if (existing is not null
            && existing.ResumptionTarget == OperatorWorkFocusResumptionTarget.PlanningRegionWork)
        {
            return OperatorWorkFocusEnsureResult.Succeeded(existing, requiredOperatorSubjectConfirmation: false);
        }

        if (projection.CurrentWork.Count != 1)
        {
            return OperatorWorkFocusEnsureResult.Failed("Active work focus requires exactly one current workflow instance.");
        }

        var work = projection.CurrentWork[0];
        if (OperatorWorkFocusSubjectProposal.TryFromGovernedVerificationContext(
                projectId,
                projectDisplayName,
                out var governedKey,
                out var subjectLabel))
        {
            var record = CreateRecord(
                projectId,
                work.InstanceId,
                subjectLabel,
                OperatorWorkFocusSubjectProvenance.GovernedVerificationReference,
                governedKey);
            _store.Upsert(record);
            return OperatorWorkFocusEnsureResult.Succeeded(record, requiredOperatorSubjectConfirmation: false);
        }

        if (!string.IsNullOrWhiteSpace(operatorConfirmedSubjectLabel))
        {
            var record = CreateRecord(
                projectId,
                work.InstanceId,
                operatorConfirmedSubjectLabel.Trim(),
                OperatorWorkFocusSubjectProvenance.OperatorConfirmed,
                null);
            _store.Upsert(record);
            return OperatorWorkFocusEnsureResult.Succeeded(record, requiredOperatorSubjectConfirmation: false);
        }

        return OperatorWorkFocusEnsureResult.Succeeded(null, requiredOperatorSubjectConfirmation: true);
    }

    public void RecordContinuitySnapshot(
        OperatorActiveWorkFocus focus,
        string continuitySummary,
        Guid? relatedPackageId)
    {
        ArgumentNullException.ThrowIfNull(focus);
        var updated = focus with
        {
            LastContinuitySummary = continuitySummary,
            LastContinuityPackageId = relatedPackageId,
            ResourceVersion = focus.ResourceVersion + 1,
            UpdatedUtc = DateTimeOffset.UtcNow,
        };

        if (!_store.TryUpdateWithExpectedVersion(updated, focus.ResourceVersion))
        {
            _store.Upsert(updated);
        }
    }

    private static OperatorActiveWorkFocus CreateRecord(
        ProjectConcordProjectId projectId,
        WorkflowInstanceId workflowInstanceId,
        string subjectLabel,
        OperatorWorkFocusSubjectProvenance provenance,
        string? governedReferenceKey) =>
        new(
            Guid.NewGuid(),
            projectId,
            workflowInstanceId,
            subjectLabel,
            provenance,
            governedReferenceKey,
            OperatorWorkFocusResumptionTarget.PlanningRegionWork,
            null,
            null,
            1,
            DateTimeOffset.UtcNow);
}

public sealed record OperatorWorkFocusEnsureResult(
    OperatorActiveWorkFocus? Focus,
    bool RequiredOperatorSubjectConfirmation,
    string? FailureMessage)
{
    public bool IsSuccess => FailureMessage is null;

    public static OperatorWorkFocusEnsureResult Succeeded(
        OperatorActiveWorkFocus? focus,
        bool requiredOperatorSubjectConfirmation) =>
        new(focus, requiredOperatorSubjectConfirmation, null);

    public static OperatorWorkFocusEnsureResult Failed(string message) =>
        new(null, false, message);
}
