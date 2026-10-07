namespace Edf.Application.Relay;

public static class RelayValidationCodes
{
    public const string PackageIdentityInvalid = "relay.package.identity.invalid";
    public const string MachineBlockMissing = "relay.structural.machine_block.missing";
    public const string MachineBlockInvalidJson = "relay.structural.machine_block.invalid_json";
    public const string GovernanceProjectionMismatch = "relay.structural.governance_projection.mismatch";
    public const string RenderVersionMissing = "relay.render.version.missing";
    public const string RenderVersionUnsupported = "relay.render.version.unsupported";
    public const string SchemaVersionUnsupported = "relay.schema.version.unsupported";
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

    public const string EngineeringAgentHandoverBlockedByActiveStop = "relay.engineering_agent.handover.stop_active";
    public const string EngineeringAgentHandoverPackageKindUnsupported = "relay.engineering_agent.handover.package_kind.unsupported";
    public const string EngineeringResultPackageKindMismatch = "relay.engineering_agent.engineering_result.kind.mismatch";

    public const string TransportExtractionStartMarkerMissing = "relay.transport.extraction.start_marker.missing";
    public const string TransportExtractionStartMarkerAmbiguous = "relay.transport.extraction.start_marker.ambiguous";
    public const string TransportExtractionEndBoundaryMissing = "relay.transport.extraction.end_boundary.missing";
    public const string TransportExtractionEndBoundaryAmbiguous = "relay.transport.extraction.end_boundary.ambiguous";
    public const string TransportExtractionMachineBlockAmbiguous = "relay.transport.extraction.machine_block.ambiguous";
    public const string TransportExtractionBoundaryOrderInvalid = "relay.transport.extraction.boundary.order.invalid";

    public const string ManualPasteEmpty = "relay.manual_paste.empty";
    public const string ManualPasteJsonOnlyRejected = "relay.manual_paste.json_only.rejected";
    public const string ManualPasteMachineBlockOnlyRejected = "relay.manual_paste.machine_block_only.rejected";
    public const string ManualPasteRenderMarkerMissing = "relay.manual_paste.render_marker.missing";
    public const string ManualPasteRenderMarkerAmbiguous = "relay.manual_paste.render_marker.ambiguous";
    public const string ManualPasteMachineBlockMissing = "relay.manual_paste.machine_block.missing";
    public const string ManualPasteIncomplete = "relay.manual_paste.incomplete";
    public const string HandoverCorrelationMismatch = "relay.handover.correlation.mismatch";
}
