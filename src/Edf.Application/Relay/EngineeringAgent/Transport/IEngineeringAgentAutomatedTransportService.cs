namespace Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Application-facing automated transport boundary (A4-T3). Implemented by <see cref="EngineeringAgentTransportOrchestrator"/>.
/// </summary>
public interface IEngineeringAgentAutomatedTransportService
{
    Task<EngineeringAgentAutomatedTransportResult> ForwardGovernedHandoverAsync(
        EngineeringAgentAutomatedForwardRequest request,
        CancellationToken cancellationToken = default);

    Task<EngineeringAgentAutomatedTransportResult> CancelTransportOperationAsync(
        EngineeringAgentAutomatedCancelRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Provider-neutral transport orchestrator port (A4-T3). Same surface as the application service.
/// </summary>
public interface IEngineeringAgentTransportOrchestrator : IEngineeringAgentAutomatedTransportService;
