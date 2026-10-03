using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

/// <summary>
/// Records which governed provenance a durable workflow mutation is invoked under.
/// Does not determine whether policy grants authorization.
/// </summary>
public sealed record GovernedWorkflowTransitionAuthority
{
    private GovernedWorkflowTransitionAuthority(
        GovernedCorrelationId correlationId,
        GovernedPackageId? originatingPackageId,
        string? authorityReference)
    {
        CorrelationId = correlationId;
        OriginatingPackageId = originatingPackageId;
        AuthorityReference = authorityReference;
    }

    public GovernedCorrelationId CorrelationId { get; }

    public GovernedPackageId? OriginatingPackageId { get; }

    public string? AuthorityReference { get; }

    public static GovernedWorkflowTransitionAuthority FromRelayProvenance(
        GovernedCorrelationId correlationId,
        GovernedPackageId? originatingPackageId = null)
    {
        if (correlationId.IsEmpty)
        {
            throw new ArgumentException("Correlation id must be non-default.", nameof(correlationId));
        }

        if (originatingPackageId is { IsEmpty: true })
        {
            throw new ArgumentException("Originating package id must be non-default when supplied.", nameof(originatingPackageId));
        }

        return new GovernedWorkflowTransitionAuthority(correlationId, originatingPackageId, null);
    }

    public static GovernedWorkflowTransitionAuthority FromAuthorityReference(
        GovernedCorrelationId correlationId,
        string authorityReference)
    {
        if (correlationId.IsEmpty)
        {
            throw new ArgumentException("Correlation id must be non-default.", nameof(correlationId));
        }

        if (string.IsNullOrWhiteSpace(authorityReference))
        {
            throw new ArgumentException("Authority reference must be non-empty.", nameof(authorityReference));
        }

        return new GovernedWorkflowTransitionAuthority(correlationId, null, authorityReference.Trim());
    }

    internal static GovernedWorkflowTransitionAuthority ForTestHarness(GovernedCorrelationId correlationId) =>
        FromRelayProvenance(correlationId);

    public bool IsWellFormed() => !CorrelationId.IsEmpty;
}
