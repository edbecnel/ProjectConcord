using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

/// <summary>
/// Shared PA handover paste artifacts for guided-exchange consumption-gate tests.
/// </summary>
public static class PaHandoverGuidedExchangeTestArtifacts
{
    public static string RenderIncompleteMissingEngineeringAgentMode(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId correlationId)
    {
        var package = RelaySerializationFixtures.ValidImplementationHandover() with
        {
            ProjectId = projectId,
            CorrelationId = correlationId,
            GovernanceCritical = RelaySerializationFixtures.ValidImplementationHandover().GovernanceCritical with
            {
                EngineeringAgentMode = null,
                PriorEngineeringAgentMode = null,
                ModeTransition = null,
            },
        };
        return new GovernedRelayV1Renderer().Render(package);
    }

    public static string RenderValidPlanningDevelopmentWorkAuthorizationHandover(
        ProjectConcordProjectId projectId,
        GovernedCorrelationId correlationId,
        Func<SoftwareDevelopmentProfilePayload, SoftwareDevelopmentProfilePayload>? configurePayload = null)
    {
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(
            projectId,
            configurePayload) with
        {
            CorrelationId = correlationId,
        };
        return new GovernedRelayV1Renderer().Render(handover);
    }
}
