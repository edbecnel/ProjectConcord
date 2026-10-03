using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Relay;

namespace Edf.Application.Operator.WorkState;

public sealed class GovernedWorkStateOperatorProjectionService
{
    private readonly IGovernedWorkStateRecoveryService _recovery;
    private readonly IGitHeadCommitResolver _gitHeadCommitResolver;

    public GovernedWorkStateOperatorProjectionService(
        IGovernedWorkStateRecoveryService recovery,
        IGitHeadCommitResolver gitHeadCommitResolver)
    {
        _recovery = recovery ?? throw new ArgumentNullException(nameof(recovery));
        _gitHeadCommitResolver = gitHeadCommitResolver ?? throw new ArgumentNullException(nameof(gitHeadCommitResolver));
    }

    public GovernedWorkStateOperatorProjection ProjectForProject(
        ProjectConcordProjectId projectId,
        string? projectRootAbsolutePath)
    {
        var recovery = _recovery.RecoverForProject(projectId);
        var currentWork = recovery.ActiveInstances
            .Select(i => new GovernedWorkStateCurrentWorkItem(
                i.InstanceId,
                i.WorkflowId,
                i.DefinitionVersion,
                i.ProfileId,
                i.TopologyPlaceId,
                i.TraversalOccurrenceId,
                i.GovernedBaseline,
                i.ResourceVersion,
                TryComputeHeadDrift(projectRootAbsolutePath, i.GovernedBaseline)))
            .ToList();

        return new GovernedWorkStateOperatorProjection(
            currentWork,
            ProjectionAvailability.Unavailable,
            GovernedWorkStateUnavailableReasons.NotImplementedInM7aWf1,
            ProjectionAvailability.Unavailable,
            GovernedWorkStateUnavailableReasons.NotImplementedInM7aWf1);
    }

    private bool? TryComputeHeadDrift(string? projectRootAbsolutePath, GovernedBaselineReference baseline)
    {
        if (baseline.Kind != GovernedBaselineKind.GitCommit || string.IsNullOrWhiteSpace(baseline.Value))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(projectRootAbsolutePath))
        {
            return null;
        }

        var head = _gitHeadCommitResolver.TryResolveHeadCommit(projectRootAbsolutePath);
        if (head is null)
        {
            return null;
        }

        return !string.Equals(head, baseline.Value, StringComparison.Ordinal);
    }
}
