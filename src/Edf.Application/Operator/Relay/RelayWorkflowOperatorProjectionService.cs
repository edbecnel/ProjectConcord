namespace Edf.Application.Operator.Relay;

using Edf.Application.Operator;
using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

/// <summary>
/// Aggregates derived relay-workflow operator projections (ADR-0020). Does not orchestrate transport.
/// </summary>
public sealed class RelayWorkflowOperatorProjectionService
{
    private readonly IReadOnlyList<IOperatorAttentionContributor> _attentionContributors;

    public RelayWorkflowOperatorProjectionService(
        IEnumerable<IOperatorAttentionContributor> attentionContributors)
    {
        _attentionContributors = (attentionContributors ?? throw new ArgumentNullException(nameof(attentionContributors))).ToArray();
    }

    public RelayWorkflowOperatorProjectionService(
        EngineeringAgentTransportOperatorAttentionContributor transportContributor)
    {
        ArgumentNullException.ThrowIfNull(transportContributor);
        _attentionContributors = [transportContributor];
    }

    public RelayWorkflowOperatorProjection Project(RelayWorkflowOperatorProjectionInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var attention = new List<OperatorAttentionItem>();
        foreach (var contributor in _attentionContributors)
        {
            attention.AddRange(contributor.Contribute(input));
        }

        var nextActions = GovernedRelayManualP0NextActionContributor.Contribute(input, attention);

        var canForward = CanAttemptAutomatedForward(input);
        var status = BuildAutomatedTransportStatus(input, attention, canForward);
        var activeOperationId = input.LastAutomatedTransportResult?.Operation?.OperationId;

        return new RelayWorkflowOperatorProjection(
            attention,
            nextActions,
            status,
            canForward,
            activeOperationId);
    }

    private static bool CanAttemptAutomatedForward(RelayWorkflowOperatorProjectionInput input)
    {
        if (input.ProjectId is null
            || input.ImportedPaHandoverPackage is null
            || input.ImportedPaHandoverValidation is null)
        {
            return false;
        }

        return input.ImportedPaHandoverValidation.IsEligibleForValidatedEngineeringAgentHandover
               && input.ImportedPaHandoverPackage.GovernanceCritical.Stop.State != RelayStopState.Active;
    }

    private static string? BuildAutomatedTransportStatus(
        RelayWorkflowOperatorProjectionInput input,
        IReadOnlyList<OperatorAttentionItem> attention,
        bool canForward)
    {
        if (input.ProjectId is null)
        {
            return "Automated transport: open a Project to evaluate provider readiness.";
        }

        if (input.LastAutomatedTransportResult is { } last)
        {
            var lifecycle = last.Operation?.LifecycleState;
            var lifecycleSuffix = lifecycle is null ? string.Empty : $" · lifecycle {lifecycle}";
            return $"Last automated transport: {last.Outcome}{lifecycleSuffix}";
        }

        if (attention.Count > 0)
        {
            return canForward
                ? "Automated transport: provider attention conditions apply; forward may fail — manual P0 relay remains available when eligible."
                : "Automated transport: attention conditions apply.";
        }

        return canForward
            ? "Automated transport: ready to forward when a provider is configured and healthy."
            : "Automated transport: import a valid PA handover to enable forward.";
    }
}
