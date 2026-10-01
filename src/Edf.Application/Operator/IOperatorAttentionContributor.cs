namespace Edf.Application.Operator;

using Edf.Application.Operator.Relay;

/// <summary>
/// Derives zero or more Attention items from authoritative inputs (ADR-0020 §3). Contributors MUST NOT mutate canonical state.
/// </summary>
public interface IOperatorAttentionContributor
{
    IReadOnlyList<OperatorAttentionItem> Contribute(RelayWorkflowOperatorProjectionInput input);
}
