namespace Edf.Application.Operator.Relay;

using Edf.Application.Operator;
using Edf.Application.Relay.EngineeringAgent.Transport;

/// <summary>
/// Derived relay-workflow operator projection bundle (ADR-0020). Noncanonical aggregate.
/// </summary>
public sealed record RelayWorkflowOperatorProjection(
    IReadOnlyList<OperatorAttentionItem> AttentionItems,
    IReadOnlyList<OperatorNextActionItem> NextActions,
    string? AutomatedTransportStatusSummary,
    bool CanAttemptAutomatedForward,
    TransportOperationId? ActiveTransportOperationId);
