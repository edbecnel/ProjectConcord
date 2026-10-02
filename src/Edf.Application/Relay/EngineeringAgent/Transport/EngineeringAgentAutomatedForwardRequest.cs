namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Application request to forward one governed handover package through automated transport (A4-T3).
/// </summary>
public sealed record EngineeringAgentAutomatedForwardRequest(
    ProjectConcordProjectId ProjectId,
    GovernedPackageId SourcePackageId,
    EngineeringAgentMode RoutingIntent,
    /// <summary>
    /// Authoritative governed Project Root locator for automated provider execution.
    /// </summary>
    ProjectLocator GovernedProjectRoot);
