namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Recovery assessment for one persisted operation (A4-T4). Candidate text is untrusted when present.
/// </summary>
public sealed record EngineeringAgentTransportRecoveryAssessment(
    TransportOperation Operation,
    EngineeringAgentTransportRecoveryDisposition Disposition,
    string? Detail,
    string? UntrustedResultCandidateText,
    bool RequiresOperatorConfirmationBeforeImport);
