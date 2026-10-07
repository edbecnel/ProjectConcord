namespace Edf.Application.Operator.WorkContinuity;

/// <summary>
/// Minimum-tranche bootstrap anchors for governed verification subject identity.
/// Not a general production registry of project UUID → work subject mappings.
/// </summary>
public static class OperatorGovernedVerificationReferences
{
    public const string Mvr0005Key = "MVR-0005";

    public const string Mvr0005DefaultSubjectLabel =
        "ProjectConcord Operator MVP human verification (MVR-0005)";

    /// <summary>
    /// Retained MVR-0005 human-verification project. Establishes MVR-0005 subject only via
    /// <see cref="OperatorWorkFocusSubjectProvenance.GovernedVerificationReference"/> for this UUID.
    /// Do not mutate in automated tests; do not operate the real GEW during implementation tests.
    /// </summary>
    public static readonly Guid Mvr0005RetainedProjectId = Guid.Parse("aff1297f-f0fe-475c-b2b4-ddf27663caff");
}
