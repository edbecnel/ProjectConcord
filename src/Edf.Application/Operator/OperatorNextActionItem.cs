namespace Edf.Application.Operator;

/// <summary>
/// One derived Next Action suggestion (ADR-0020 §4). Noncanonical.
/// </summary>
public sealed record OperatorNextActionItem(
    string Code,
    OperatorNextActionClass ActionClass,
    string Message,
    OperatorAttentionSourceDomain SourceDomain);
