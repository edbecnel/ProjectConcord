using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class DevelopmentWorkAuthorizationConcurrencyException : Exception
{
    public DevelopmentWorkAuthorizationConcurrencyException(
        DevelopmentWorkAuthorizationId authorizationId,
        long expectedResourceVersion)
        : base(
            $"Development work authorization '{authorizationId}' concurrency conflict (expected resource version {expectedResourceVersion}).")
    {
        AuthorizationId = authorizationId;
        ExpectedResourceVersion = expectedResourceVersion;
    }

    public DevelopmentWorkAuthorizationId AuthorizationId { get; }

    public long ExpectedResourceVersion { get; }
}
