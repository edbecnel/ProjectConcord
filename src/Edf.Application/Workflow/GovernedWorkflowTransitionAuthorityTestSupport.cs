using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

/// <summary>
/// Test-only helpers for supplying governed transition provenance in automated tests.
/// </summary>
public static class GovernedWorkflowTransitionAuthorityTestSupport
{
    public static GovernedWorkflowTransitionAuthority ForTestHarness(GovernedCorrelationId correlationId) =>
        GovernedWorkflowTransitionAuthority.ForTestHarness(correlationId);
}
