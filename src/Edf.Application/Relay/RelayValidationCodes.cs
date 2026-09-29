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
}
