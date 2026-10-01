namespace Edf.Application.Operator.Relay;

using Edf.Application.Operator;
using Edf.Application.Relay.EngineeringAgent.Transport;
using Edf.Domain.Relay;

/// <summary>
/// Derives Recommended manual P0 governed relay when eligibility holds and automated transport needs operator fallback (ADR-0022 §13–14).
/// </summary>
public static class GovernedRelayManualP0NextActionContributor
{
    public static IReadOnlyList<OperatorNextActionItem> Contribute(
        RelayWorkflowOperatorProjectionInput input,
        IReadOnlyList<OperatorAttentionItem> transportAttention)
    {
        if (!IsManualP0RelayEligible(input))
        {
            return [];
        }

        if (!ShouldRecommendManualFallback(input, transportAttention))
        {
            return [];
        }

        return
        [
            new OperatorNextActionItem(
                OperatorNextActionCodes.GovernedRelayManualP0,
                OperatorNextActionClass.Recommended,
                "Use governed manual relay: Prepare Engineering Agent handover, Copy, and Import result when ready.",
                OperatorAttentionSourceDomain.GovernedRelay),
        ];
    }

    internal static bool IsManualP0RelayEligible(RelayWorkflowOperatorProjectionInput input)
    {
        if (input.ImportedPaHandoverPackage is not { } package
            || input.ImportedPaHandoverValidation is not { } validation)
        {
            return false;
        }

        return validation.IsEligibleForValidatedEngineeringAgentHandover
               && package.GovernanceCritical.Stop.State != RelayStopState.Active;
    }

    private static bool ShouldRecommendManualFallback(
        RelayWorkflowOperatorProjectionInput input,
        IReadOnlyList<OperatorAttentionItem> transportAttention)
    {
        if (transportAttention.Count > 0)
        {
            return true;
        }

        if (input.LastAutomatedTransportResult is { Outcome: not EngineeringAgentAutomatedTransportOutcome.Dispatched
            and not EngineeringAgentAutomatedTransportOutcome.ImportCompleted
            and not EngineeringAgentAutomatedTransportOutcome.Unavailable
            and not EngineeringAgentAutomatedTransportOutcome.GovernanceIneligible })
        {
            return true;
        }

        return false;
    }
}
