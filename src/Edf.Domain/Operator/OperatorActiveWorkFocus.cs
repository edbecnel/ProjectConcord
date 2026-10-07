namespace Edf.Domain.Operator;

using Edf.Domain.Projects;
using Edf.Domain.Workflow;

public sealed record OperatorActiveWorkFocus(
    Guid FocusId,
    ProjectConcordProjectId ProjectId,
    WorkflowInstanceId WorkflowInstanceId,
    string SubjectLabel,
    OperatorWorkFocusSubjectProvenance SubjectProvenance,
    string? GovernedReferenceKey,
    OperatorWorkFocusResumptionTarget ResumptionTarget,
    string? LastContinuitySummary,
    Guid? LastContinuityPackageId,
    long ResourceVersion,
    DateTimeOffset UpdatedUtc);
