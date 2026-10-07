namespace Edf.Application.Operator.WorkContinuity;

using Edf.Domain.Projects;

/// <summary>
/// Proposes work subject identity from explicit governed verification anchors only.
/// Display names and other incidental text are not governed references.
/// </summary>
public static class OperatorWorkFocusSubjectProposal
{
    public static bool TryFromGovernedVerificationContext(
        ProjectConcordProjectId projectId,
        string? projectDisplayName,
        out string governedReferenceKey,
        out string subjectLabel)
    {
        _ = projectDisplayName;

        if (projectId.Value == OperatorGovernedVerificationReferences.Mvr0005RetainedProjectId)
        {
            governedReferenceKey = OperatorGovernedVerificationReferences.Mvr0005Key;
            subjectLabel = OperatorGovernedVerificationReferences.Mvr0005DefaultSubjectLabel;
            return true;
        }

        governedReferenceKey = string.Empty;
        subjectLabel = string.Empty;
        return false;
    }
}
