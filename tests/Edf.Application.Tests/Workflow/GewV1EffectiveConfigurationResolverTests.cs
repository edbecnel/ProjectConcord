using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class GewV1EffectiveConfigurationResolverTests
{
    private readonly GewV1EffectiveConfigurationResolver _resolver = new(new GewV1PrescribedWorkflowRegistry());

    [Fact]
    public void Resolve_Succeeds_WhenProfileKnown()
    {
        var instance = BuildInstance(withProfile: true);
        var result = _resolver.Resolve(new EffectiveConfigurationResolveInput(instance, Array.Empty<DevelopmentWorkAuthorization>()));

        var resolved = Assert.IsType<EffectiveConfigurationResolved>(result);
        Assert.Equal(GewV1ProfileIds.Standard, resolved.View.ProfileId.Value);
        Assert.True(resolved.View.ProfileSynchronizationFloorsUnspecified);
    }

    [Fact]
    public void Resolve_FailsClosed_WhenProfileMissing()
    {
        var instance = BuildInstance(withProfile: false);
        var result = _resolver.Resolve(new EffectiveConfigurationResolveInput(instance, Array.Empty<DevelopmentWorkAuthorization>()));

        var unresolved = Assert.IsType<EffectiveConfigurationUnresolved>(result);
        Assert.Equal(EffectiveConfigurationResolveFailureCode.MissingProfile, unresolved.Code);
    }

    private static WorkflowInstance BuildInstance(bool withProfile)
    {
        var now = DateTimeOffset.UtcNow;
        return new WorkflowInstance(
            WorkflowInstanceId.New(),
            ProjectConcordProjectId.New(),
            PrescribedWorkflowId.GovernedEngineering,
            WorkflowDefinitionVersion.GewV1,
            withProfile ? WorkflowProfileId.Parse(GewV1ProfileIds.Standard) : null,
            WorkflowInstanceLifecycle.Active,
            TopologyPlaceId.Parse(GewV1TopologyPlaces.Intake),
            TraversalOccurrenceId.New(),
            GovernedBaselineReference.Unspecified,
            GovernedCorrelationId.New(),
            null,
            null,
            null,
            1,
            now,
            now);
    }
}
