namespace Edf.Application.Relay.EngineeringAgent.Transport;

using Edf.Domain.Projects;

/// <summary>
/// Restart recovery and idempotency for persisted transport operations (ADR-0022 §7–8, §10; A4-T4).
/// Explicitly invoked — no automatic startup execution.
/// </summary>
public interface IEngineeringAgentTransportRecoveryService
{
    IReadOnlyList<TransportOperation> DiscoverRecoverableOperations(ProjectConcordProjectId projectId);

    Task<EngineeringAgentTransportRecoveryAssessment> AssessOperationAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default);

    Task<EngineeringAgentTransportRecoveryActionResult> ResumeCreatedNotForwardedAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default);

    Task<EngineeringAgentTransportRecoveryAssessment> ReconcileProviderStateAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default);

    Task<EngineeringAgentTransportRecoveryActionResult> ConfirmRecoveredResultImportAsync(
        ProjectConcordProjectId projectId,
        TransportOperationId transportOperationId,
        CancellationToken cancellationToken = default);
}
