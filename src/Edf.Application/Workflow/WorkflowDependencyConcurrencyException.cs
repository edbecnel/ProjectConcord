using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed class WorkflowDependencyConcurrencyException : Exception
{
    public WorkflowDependencyConcurrencyException(WorkflowDependencyId dependencyId, long expectedResourceVersion)
        : base(
            $"Workflow dependency '{dependencyId}' update rejected: expected resource version {expectedResourceVersion} is stale or the dependency does not exist.")
    {
        DependencyId = dependencyId;
        ExpectedResourceVersion = expectedResourceVersion;
    }

    public WorkflowDependencyId DependencyId { get; }

    public long ExpectedResourceVersion { get; }
}
