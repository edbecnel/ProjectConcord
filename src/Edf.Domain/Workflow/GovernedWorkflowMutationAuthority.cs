using Edf.Domain.Relay;

namespace Edf.Domain.Workflow;

/// <summary>
/// Records governed provenance for durable workflow relationship mutations.
/// </summary>
public sealed record GovernedWorkflowMutationAuthority
{
    private GovernedWorkflowMutationAuthority(
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

    public static GovernedWorkflowMutationAuthority FromRelayProvenance(
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

        return new GovernedWorkflowMutationAuthority(correlationId, originatingPackageId, null);
    }

    public static GovernedWorkflowMutationAuthority FromAuthorityReference(
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

        return new GovernedWorkflowMutationAuthority(correlationId, null, authorityReference.Trim());
    }

    internal static GovernedWorkflowMutationAuthority ForTestHarness(GovernedCorrelationId correlationId) =>
        FromRelayProvenance(correlationId);

    public bool IsWellFormed() => !CorrelationId.IsEmpty;
}
