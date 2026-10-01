namespace Edf.Application.Operator;

/// <summary>
/// One derived Attention condition for operator presentation (ADR-0020 §3). Not authoritative lifecycle state.
/// </summary>
public sealed record OperatorAttentionItem(
    string Code,
    OperatorAttentionSourceDomain SourceDomain,
    string Message,
    bool IsBlocking,
    OperatorAttentionUrgency Urgency,
    string? Detail = null);
