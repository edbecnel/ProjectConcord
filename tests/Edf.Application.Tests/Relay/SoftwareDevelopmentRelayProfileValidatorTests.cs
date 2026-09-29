using Edf.Application.Relay;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class SoftwareDevelopmentRelayProfileValidatorTests
{
    private readonly GovernedRelayPackageValidator _validator =
        new(SoftwareDevelopmentRelayProfileValidator.Instance);

    [Fact]
    public void EmptyProfile_OnNonImplementationPackage_RemainsValid()
    {
        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty);

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Valid, result.State);
    }

    [Fact]
    public void GenericPackage_WithoutProfile_DoesNotBecomeDevelopmentWorkAuthorization()
    {
        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            payloadBytes: ReadOnlyMemory<byte>.Empty);

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Valid, result.State);
    }

    [Fact]
    public void HandoverAlone_DoesNotSatisfyImplementationAuthorization()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            HandoverContext = new HandoverContextProjection("Inherited context", true),
        };

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: payload);

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.HandoverDoesNotImplyAuthorization);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ImplementationAuthorizationMissing);
    }

    [Fact]
    public void ExplicitImplementationAuthorization_SatisfiesImplementationDirective()
    {
        var payload = BuildImplementationPayload("A2-T4");

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, true),
            authorizationDispositionPresent: true,
            workContextPresent: true,
            payload: payload,
            configureGovernance: g => g with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Valid, result.State);
    }

    [Fact]
    public void MissingImplementationAuthorization_ForImplementationDirective_IsIncomplete()
    {
        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty,
            configureGovernance: g => g with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ImplementationAuthorizationMissing);
    }

    [Fact]
    public void PlanningAuthorization_CannotSatisfyImplementationDirective()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                SoftwareDevelopmentAuthorizationKind.Planning,
                "A2-T4",
                ["planning"],
                "plan-ref",
                false),
        };

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: payload,
            configureGovernance: g => g with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.PlanningCannotSatisfyImplementation);
    }

    [Fact]
    public void AuthorizedTranche_CannotAuthorizeNextTranche()
    {
        var payload = BuildImplementationPayload("A2-T4") with
        {
            WorkContext = new WorkContextProjection("A2-T5", "Next tranche work"),
        };

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, true),
            authorizationDispositionPresent: true,
            workContextPresent: true,
            payload: payload,
            configureGovernance: g => g with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.TrancheScopeExceeded);
    }

    [Fact]
    public void StopAcknowledgment_DoesNotCreateAuthorization()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                SoftwareDevelopmentAuthorizationKind.Implementation,
                "A2-T4",
                null,
                null,
                AuthorizedByStopAcknowledgment: true),
        };

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: payload);

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.StopDoesNotAuthorize);
    }

    [Fact]
    public void Tier0GitHead_DoesNotImplyAuthorization()
    {
        var tier0 = new Tier0RelaySnapshot(
            "75d894bc19c5e5dbed13f75f98b24dc8cc356358",
            ["ARCHITECTURE_DECISIONS.md"],
            new Dictionary<string, string> { ["roadmapStatus"] = "COMPLETE" });

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty,
            tier0: tier0,
            configureGovernance: g => g with
            {
                EngineeringAgentMode = EngineeringAgentMode.Plan,
                SessionContinuity = RelayTestFixtures.SessionWithBothIntents(),
            });

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.DoesNotContain(result.Diagnostics, d => d.Code == RelayValidationCodes.ProfilePayloadMalformed);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ImplementationAuthorizationMissing);
    }

    [Fact]
    public void PaAcceptance_DoesNotSubstituteForImplementationAuthorization()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            ArchitecturalReview = new ArchitecturalReviewProjection(
                ArchitecturalReviewPaDisposition.Accepted,
                "AAR-EXAMPLE",
                ImpliesImplementationAuthorization: false),
        };

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: payload);

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(
            result.Diagnostics,
            d => d.Code == RelayValidationCodes.ArchitecturalAcceptanceNotImplementationAuthorization);
    }

    [Fact]
    public void ProfilePayload_RoundTrips_ThroughSerializer()
    {
        var original = BuildImplementationPayload("A2-T4");
        var bytes = SoftwareDevelopmentProfilePayloadSerializer.Serialize(original);

        Assert.True(
            SoftwareDevelopmentProfilePayloadSerializer.TryDeserialize(bytes, out var roundTrip, out _));
        Assert.NotNull(roundTrip);
        Assert.Equal(original.DevelopmentWorkAuthorization!.AuthorizedTrancheId, roundTrip!.DevelopmentWorkAuthorization!.AuthorizedTrancheId);
    }

    [Fact]
    public void ProfileDiagnostics_FlowThroughCoreValidator()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            HandoverContext = new HandoverContextProjection("merged", false),
        };

        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            payload: payload);

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.HandoverDevelopmentWorkAuthorizationConflation);
    }

    [Fact]
    public void NullProfileValidator_DoesNotApplyBSemantics()
    {
        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(true, false),
            authorizationDispositionPresent: true,
            payload: SoftwareDevelopmentProfilePayloadSerializer.Empty);

        var result = new GovernedRelayPackageValidator().Validate(package);

        Assert.Equal(RelayValidationState.Valid, result.State);
        Assert.DoesNotContain(
            result.Diagnostics,
            d => d.Code == RelayValidationCodes.ImplementationAuthorizationMissing);
    }

    [Fact]
    public void ProjectWorkRecord_MustNotSubstituteForDevelopmentWorkAuthorization()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            ProjectWorkRecord = new ProjectWorkRecordCorrelation("pwr-1", TreatsRecordAsAuthorization: true),
        };

        var result = _validator.Validate(CreatePackage(directives: new(false, false), payload: payload));

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ProjectWorkRecordAuthorityConflation);
    }

    [Fact]
    public void HumanInitiatedWorkItem_MustNotSubstituteForAuthorization()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            HumanInitiatedWorkItem = new HumanInitiatedWorkItemCorrelation("hiw-1", TreatsIntakeAsAuthorization: true),
        };

        var result = _validator.Validate(CreatePackage(directives: new(false, false), payload: payload));

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.HumanInitiatedWorkItemAuthorityConflation);
    }

    [Fact]
    public void AuthorityGrantPlaceholder_IsRejected()
    {
        var payload = SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            AuthorityGrant = new AuthorityGrantPlaceholder("grant-1"),
        };

        var result = _validator.Validate(CreatePackage(directives: new(false, false), payload: payload));

        Assert.Equal(RelayValidationState.Incomplete, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.AuthorityGrantNotSupported);
    }

    [Fact]
    public void MalformedProfilePayload_IsRejectedMalformed()
    {
        var package = CreatePackage(
            directives: new RelayGovernanceDirectiveFlags(false, false),
            payloadBytes: "{ not-json"u8.ToArray());

        var result = _validator.Validate(package);

        Assert.Equal(RelayValidationState.RejectedMalformed, result.State);
        Assert.Contains(result.Diagnostics, d => d.Code == RelayValidationCodes.ProfilePayloadMalformed);
    }

    private static SoftwareDevelopmentProfilePayload BuildImplementationPayload(string tranche) =>
        SoftwareDevelopmentProfilePayloadSerializer.Empty with
        {
            AuthorizationDisposition = new AuthorizationDispositionProjection(
                "IMPLEMENTATION AUTHORIZED",
                PlanningAuthorized: false,
                ImplementationAuthorized: true,
                AuthorizedTrancheId: tranche),
            DevelopmentWorkAuthorization = new DevelopmentWorkAuthorizationProjection(
                SoftwareDevelopmentAuthorizationKind.Implementation,
                tranche,
                ["A2-T4-scope"],
                "dwa-projection-ref",
                false),
            WorkContext = new WorkContextProjection(tranche, "Authorized tranche work"),
        };

    private static GovernedRelayPackage CreatePackage(
        RelayGovernanceDirectiveFlags directives,
        SoftwareDevelopmentProfilePayload? payload = null,
        ReadOnlyMemory<byte>? payloadBytes = null,
        bool authorizationDispositionPresent = false,
        bool workContextPresent = false,
        Tier0RelaySnapshot? tier0 = null,
        Func<RelayGovernanceCriticalState, RelayGovernanceCriticalState>? configureGovernance = null)
    {
        var bytes = payloadBytes
            ?? SoftwareDevelopmentProfilePayloadSerializer.Serialize(payload ?? SoftwareDevelopmentProfilePayloadSerializer.Empty);

        var governance = new RelayGovernanceCriticalState(
            EngineeringAgentMode.Plan,
            null,
            null,
            RelayTestFixtures.SessionWithBothIntents(),
            RelayStopMetadata.None,
            authorizationDispositionPresent,
            workContextPresent,
            directives,
            null);

        if (configureGovernance is not null)
        {
            governance = configureGovernance(governance);
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
            tier0,
            bytes,
            GovernedRelayPackage.DefaultStructuralAgreement);
    }
}
