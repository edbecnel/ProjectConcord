namespace Edf.Application.Relay.EngineeringAgent.Plugins;

using Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Untrusted provider output candidate for the existing parse/validate/import pipeline (ADR-0022 §3).
/// <see cref="IsReadyForParse"/> means the provider has a completed candidate available for host parsing —
/// not that the text is governed-valid or governance-accepted.
/// </summary>
public sealed record EngineeringAgentTransportResultCandidate(
    TransportOperationId TransportOperationId,
    string UntrustedImportText,
    bool IsReadyForParse);

/// <summary>
/// Result of polling or retrieving a transport result candidate from the provider plugin.
/// </summary>
public sealed record EngineeringAgentResultCandidateResult(
    bool HasCandidate,
    EngineeringAgentTransportResultCandidate? Candidate,
    EngineeringAgentProviderFailure? Failure);
