namespace Edf.Application.Relay;

public static class RelayValidationCodes
{
    public const string PackageIdentityInvalid = "relay.package.identity.invalid";
    public const string MachineBlockMissing = "relay.structural.machine_block.missing";
    public const string GovernanceProjectionMismatch = "relay.structural.governance_projection.mismatch";
    public const string ModeTransitionContradictory = "relay.governance.mode_transition.contradictory";
    public const string EngineeringAgentModeMissing = "relay.governance.engineering_agent_mode.missing";
    public const string EngineeringAgentSessionIntentMissing = "relay.governance.engineering_agent_session_intent.missing";
    public const string ProjectArchitectSessionIntentMissing = "relay.governance.project_architect_session_intent.missing";
    public const string ModeTransitionMissing = "relay.governance.mode_transition.missing";
    public const string AuthorizationDispositionMissing = "relay.governance.authorization_disposition.missing";
    public const string WorkContextMissing = "relay.governance.work_context.missing";
    public const string ProfileBoundaryViolation = "relay.profile.boundary.violation";

    public const string ProfilePayloadMalformed = "relay.profile.payload.malformed";
    public const string ProfilePayloadVersionUnsupported = "relay.profile.payload.version.unsupported";
    public const string HandoverDevelopmentWorkAuthorizationConflation = "relay.profile.handover_dwa.conflation";
    public const string HandoverDoesNotImplyAuthorization = "relay.profile.handover.not_authorization";
    public const string AuthorizationDispositionPayloadMissing = "relay.profile.authorization_disposition.payload.missing";
    public const string AuthorizationDispositionPayloadUnexpected = "relay.profile.authorization_disposition.payload.unexpected";
    public const string AuthorizationDispositionNotDevelopmentWorkAuthorization = "relay.profile.authorization_disposition.not_dwa";
    public const string ImplementationAuthorizationMissing = "relay.profile.implementation_authorization.missing";
    public const string PlanningCannotSatisfyImplementation = "relay.profile.planning_cannot_satisfy_implementation";
    public const string WorkContextPayloadMissing = "relay.profile.work_context.payload.missing";
    public const string WorkContextPayloadUnexpected = "relay.profile.work_context.payload.unexpected";
    public const string AuthorizedTrancheMissing = "relay.profile.authorized_tranche.missing";
    public const string TrancheScopeExceeded = "relay.profile.tranche_scope.exceeded";
    public const string StopDoesNotAuthorize = "relay.profile.stop.not_authorization";
    public const string StopBlocksImplicitImplementationAuthorization = "relay.profile.stop.blocks_implicit_authorization";
    public const string ArchitecturalAcceptanceNotImplementationAuthorization = "relay.profile.architectural_acceptance.not_implementation_authorization";
    public const string ProjectWorkRecordAuthorityConflation = "relay.profile.pwr_authority.conflation";
    public const string HumanInitiatedWorkItemAuthorityConflation = "relay.profile.hiw_authority.conflation";
    public const string AuthorityGrantNotSupported = "relay.profile.authority_grant.not_supported";
}
