using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public static class GovernedWorkflowMutationAuthorityTestSupport
{
    public static GovernedWorkflowMutationAuthority ForTestHarness(GovernedCorrelationId correlationId) =>
        GovernedWorkflowMutationAuthority.ForTestHarness(correlationId);
}
