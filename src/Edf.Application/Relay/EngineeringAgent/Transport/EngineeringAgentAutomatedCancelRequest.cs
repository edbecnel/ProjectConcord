namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Domain.Projects;

/// <summary>
/// Application request to cancel one known transport operation (A4-T3).
/// </summary>
public sealed record EngineeringAgentAutomatedCancelRequest(
    ProjectConcordProjectId ProjectId,
    TransportOperationId TransportOperationId);
