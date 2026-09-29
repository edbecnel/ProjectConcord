using Edf.Application.Relay;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayPackageValidatorTests
{
    private readonly GovernedRelayPackageValidator _validator = new();

    [Fact]
    public void CompleteGovernanceCriticalState_ValidatesAsValid()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with
            {
                EngineeringAgentMode = EngineeringAgentMode.Agent,
                PriorEngineeringAgentMode = EngineeringAgentMode.Plan,
                ModeTransition = new EngineeringAgentModeTransition(EngineeringAgentMode.Plan, EngineeringAgentMode.Agent),
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
                DirectiveFlags = new RelayGovernanceDirectiveFlags(
                    DirectsImplementationWork: true,
                    DirectsTrancheWork: false),
                AuthorizationDispositionPresent = true,
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Valid, result.State);
        Assert.True(result.IsEligibleForValidatedCursorHandover);
    }

    [Fact]
    public void MissingEngineeringAgentMode_IsIncomplete_NotMalformed()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with { EngineeringAgentMode = null });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.NotEqual(RelayValidationState.RejectedMalformed, result.State);
        Assert.False(result.IsEligibleForValidatedCursorHandover);
    }

    [Fact]
    public void MissingSessionIntents_AreIncomplete_EvenWhenAdvisoriesRecommendContinue()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with
            {
                EngineeringAgentMode = EngineeringAgentMode.Agent,
                SessionContinuity = new RelaySessionContinuity(
                    ProjectArchitectSessionIntent: null,
                    ProjectArchitectSessionAdvisory: new AgentSessionAdvisory(AgentSessionAdvisoryKind.RecommendContinue),
                    EngineeringAgentSessionIntent: null,
                    EngineeringAgentSessionAdvisory: new AgentSessionAdvisory(AgentSessionAdvisoryKind.RecommendContinue)),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(
            result.Diagnostics,
            d => d.Code == RelayValidationCodes.EngineeringAgentSessionIntentMissing);
        Assert.Contains(
            result.Diagnostics,
            d => d.Code == RelayValidationCodes.ProjectArchitectSessionIntentMissing);
    }

    [Fact]
    public void MissingMachineBlock_IsRejectedMalformed()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(
            structuralAgreement: new GovernedRelayStructuralAgreement(
                RequiresMachineBlock: true,
                MachineBlockPresent: false,
                RequiresGovernanceProjectionAgreement: false,
                GovernanceProjectionsAgree: true));

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.State);
    }

    [Fact]
    public void GovernanceProjectionMismatch_IsRejectedMalformed()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(
            governance => governance with { EngineeringAgentMode = EngineeringAgentMode.Agent },
            structuralAgreement: new GovernedRelayStructuralAgreement(
                RequiresMachineBlock: true,
                MachineBlockPresent: true,
                RequiresGovernanceProjectionAgreement: true,
                GovernanceProjectionsAgree: false));

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.State);
        Assert.NotEqual(RelayValidationState.Incomplete, result.State);
    }

    [Fact]
    public void ActiveStop_DoesNotSubstituteForMissingAuthorizationDisposition()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with
            {
                EngineeringAgentMode = EngineeringAgentMode.Agent,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
                Stop = new RelayStopMetadata(RelayStopState.Active, "STOP"),
                DirectiveFlags = new RelayGovernanceDirectiveFlags(
                    DirectsImplementationWork: true,
                    DirectsTrancheWork: false),
                AuthorizationDispositionPresent = false,
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(
            result.Diagnostics,
            d => d.Code == RelayValidationCodes.AuthorizationDispositionMissing);
    }

    [Fact]
    public void ModeTransitionContradiction_IsRejectedMalformed()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with
            {
                EngineeringAgentMode = EngineeringAgentMode.Agent,
                PriorEngineeringAgentMode = EngineeringAgentMode.Plan,
                ModeTransition = new EngineeringAgentModeTransition(EngineeringAgentMode.Agent, EngineeringAgentMode.Agent),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.State);
    }

    [Fact]
    public void ModeChangeWithoutTransition_IsIncomplete()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with
            {
                EngineeringAgentMode = EngineeringAgentMode.Agent,
                PriorEngineeringAgentMode = EngineeringAgentMode.Plan,
                ModeTransition = null,
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ModeTransitionMissing);
    }

    [Fact]
    public void ProfileValidator_CanAddDiagnosticsWithoutCoreEmbeddingBSemantics()
    {
        var package = RelayTestFixtures.CreatePaHandoverImport(governance =>
            governance with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
            });

        var validator = new GovernedRelayPackageValidator(new HandoverDwaConflationTestValidator());
        var result = validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ProfileBoundaryViolation);
    }

    [Fact]
    public void OnlyValidState_IsEligibleForValidatedCursorHandover()
    {
        Assert.False(RelayValidatedHandoverEligibility.IsEligibleForValidatedCursorHandover(RelayValidationState.Incomplete));
        Assert.False(RelayValidatedHandoverEligibility.IsEligibleForValidatedCursorHandover(RelayValidationState.RejectedMalformed));
        Assert.True(RelayValidatedHandoverEligibility.IsEligibleForValidatedCursorHandover(RelayValidationState.Valid));
    }

    [Fact]
    public void GovernedRelayPackage_LivesInDomainWithoutApplicationDependencies()
    {
        var assembly = typeof(GovernedRelayPackage).Assembly;
        var references = assembly.GetReferencedAssemblies().Select(r => r.Name).ToList();

        Assert.DoesNotContain("Edf.Application", references);
        Assert.DoesNotContain("Edf.ProjectServices", references);
        Assert.DoesNotContain("Edf.Desktop", references);
    }

    private sealed class HandoverDwaConflationTestValidator : ISoftwareDevelopmentRelayProfileValidator
    {
        public void Validate(GovernedRelayPackage package, RelayValidationAccumulator accumulator)
        {
            accumulator.AddIncomplete(
                RelayValidationCodes.ProfileBoundaryViolation,
                "Handover content must not be conflated with DevelopmentWorkAuthorization semantics.");
        }
    }
}

internal static class RelayTestFixtures
{
    public static RelaySessionContinuity SessionWithBothIntents() =>
        new(
            ProjectArchitectSessionIntent: AgentSessionIntent.Continue,
            ProjectArchitectSessionAdvisory: AgentSessionAdvisory.None,
            EngineeringAgentSessionIntent: AgentSessionIntent.New,
            EngineeringAgentSessionAdvisory: AgentSessionAdvisory.None);

    public static GovernedRelayPackage CreatePaHandoverImport(
        Func<RelayGovernanceCriticalState, RelayGovernanceCriticalState>? configure = null,
        GovernedRelayStructuralAgreement? structuralAgreement = null)
    {
        var governance = DefaultGovernanceCritical();
        if (configure is not null)
        {
            governance = configure(governance);
        }

        return new GovernedRelayPackage(
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            GovernedPackageKind.PaHandoverImport,
            RelaySchemaVersion.Current,
            RelayRenderVersion.V1,
            ProjectConcordProjectId.New(),
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            governance,
            Tier0RelaySnapshot.Empty,
            ReadOnlyMemory<byte>.Empty,
            structuralAgreement ?? GovernedRelayPackage.DefaultStructuralAgreement);
    }

    private static RelayGovernanceCriticalState DefaultGovernanceCritical() =>
        new(
            EngineeringAgentMode: EngineeringAgentMode.Plan,
            PriorEngineeringAgentMode: null,
            ModeTransition: null,
            SessionContinuity: SessionWithBothIntents(),
            Stop: RelayStopMetadata.None,
            AuthorizationDispositionPresent: false,
            WorkContextPresent: false,
            DirectiveFlags: new RelayGovernanceDirectiveFlags(
                DirectsImplementationWork: false,
                DirectsTrancheWork: false),
            EdfCorrelation: null);
}
