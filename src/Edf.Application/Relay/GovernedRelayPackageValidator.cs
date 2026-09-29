namespace Edf.Application.Relay;

using Edf.Domain.Relay;

/// <summary>
/// Core relay envelope validation (PC-PAR-013, PC-PAR-015, PC-PAR-020). No prose inference.
/// </summary>
public sealed class GovernedRelayPackageValidator
{
    private readonly ISoftwareDevelopmentRelayProfileValidator _profileValidator;

    public GovernedRelayPackageValidator()
        : this(NullSoftwareDevelopmentRelayProfileValidator.Instance)
    {
    }

    public GovernedRelayPackageValidator(ISoftwareDevelopmentRelayProfileValidator profileValidator)
    {
        _profileValidator = profileValidator;
    }

    public RelayValidationResult Validate(GovernedRelayPackage package) =>
        Validate(package, _profileValidator);

    public RelayValidationResult Validate(
        GovernedRelayPackage package,
        ISoftwareDevelopmentRelayProfileValidator profileValidator)
    {
        var accumulator = new RelayValidationAccumulator();

        ValidatePackageIdentity(package, accumulator);
        ValidateStructuralAgreement(package.StructuralAgreement, accumulator);

        if (!accumulator.Diagnostics.Any(d => d.Severity == RelayValidationDiagnosticSeverity.Malformed))
        {
            ValidateGovernanceCritical(package, accumulator);
            profileValidator.Validate(package, accumulator);
        }

        return accumulator.ToResult();
    }

    private static void ValidatePackageIdentity(GovernedRelayPackage package, RelayValidationAccumulator accumulator)
    {
        if (package.PackageId.IsEmpty || package.CorrelationId.IsEmpty || package.ProjectId.Value == Guid.Empty)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.PackageIdentityInvalid,
                "Package id, correlation id, and project id must be non-empty.");
        }
    }

    private static void ValidateStructuralAgreement(
        GovernedRelayStructuralAgreement agreement,
        RelayValidationAccumulator accumulator)
    {
        if (agreement.RequiresMachineBlock && !agreement.MachineBlockPresent)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.MachineBlockMissing,
                "Machine relay block is required but absent.");
        }

        if (agreement.RequiresGovernanceProjectionAgreement && !agreement.GovernanceProjectionsAgree)
        {
            accumulator.AddMalformed(
                RelayValidationCodes.GovernanceProjectionMismatch,
                "Machine block and projected governance-critical fields disagree.");
        }
    }

    private static void ValidateGovernanceCritical(GovernedRelayPackage package, RelayValidationAccumulator accumulator)
    {
        if (!RequiresGovernanceCriticalFields(package.Kind))
        {
            return;
        }

        var governance = package.GovernanceCritical;
        var session = governance.SessionContinuity;

        if (governance.EngineeringAgentMode is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.EngineeringAgentModeMissing,
                "Engineering agent mode is required and must not be inferred.");
        }

        if (session.EngineeringAgentSessionIntent is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.EngineeringAgentSessionIntentMissing,
                "Engineering agent session intent is required and must not be inferred from advisories or prose.");
        }

        if (session.ProjectArchitectSessionIntent is null)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ProjectArchitectSessionIntentMissing,
                "Project Architect session intent is required and must not be inferred from advisories or prose.");
        }

        EvaluateModeTransition(governance, accumulator);

        if (governance.DirectiveFlags.DirectsImplementationWork && !governance.AuthorizationDispositionPresent)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.AuthorizationDispositionMissing,
                "Authorization disposition is required when directing implementation work; STOP metadata does not substitute.");
        }

        if (governance.DirectiveFlags.DirectsTrancheWork && !governance.WorkContextPresent)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.WorkContextMissing,
                "Work/tranche context is required when the package directs tranche work.");
        }
    }

    internal static void EvaluateModeTransition(
        RelayGovernanceCriticalState governance,
        RelayValidationAccumulator accumulator)
    {
        var current = governance.EngineeringAgentMode;
        var prior = governance.PriorEngineeringAgentMode;
        var transition = governance.ModeTransition;

        if (transition is not null)
        {
            if (current is null)
            {
                accumulator.AddMalformed(
                    RelayValidationCodes.ModeTransitionContradictory,
                    "Mode transition is present but engineering agent mode is missing.");
                return;
            }

            if (transition.To != current)
            {
                accumulator.AddMalformed(
                    RelayValidationCodes.ModeTransitionContradictory,
                    "Mode transition target does not match the declared engineering agent mode.");
            }

            if (prior is not null && transition.From != prior)
            {
                accumulator.AddMalformed(
                    RelayValidationCodes.ModeTransitionContradictory,
                    "Mode transition source does not match the prior engineering agent mode.");
            }

            if (prior is null && transition.From != transition.To)
            {
                accumulator.AddIncomplete(
                    RelayValidationCodes.ModeTransitionMissing,
                    "Prior engineering agent mode is required when a mode transition is declared.");
            }

            if (prior is not null && prior == current)
            {
                accumulator.AddMalformed(
                    RelayValidationCodes.ModeTransitionContradictory,
                    "Mode transition is present but engineering agent mode did not change.");
            }

            return;
        }

        if (prior is not null && current is not null && prior != current)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ModeTransitionMissing,
                "Mode transition metadata is required when engineering agent mode changes.");
        }
    }

    /// <summary>
    /// PC-PAR-020 scope for T1: handover-oriented kinds only. New <see cref="GovernedPackageKind"/>
    /// values must be deliberately classified here — they must not silently bypass governance validation.
    /// </summary>
    private static bool RequiresGovernanceCriticalFields(GovernedPackageKind kind) =>
        kind is GovernedPackageKind.PaHandoverImport or GovernedPackageKind.CursorHandoverExport;
}
