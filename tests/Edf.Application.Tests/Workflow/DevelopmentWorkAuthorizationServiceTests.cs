using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Workflow;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;

namespace Edf.Application.Tests.Workflow;

public class DevelopmentWorkAuthorizationServiceTests
{
    [Fact]
    public void Grant_InsertsActiveAuthorization_WithProvenance()
    {
        var (services, instance) = CreateInstanceWithProfile();
        var authority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());

        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            authorizedTrancheId: "tranche-a",
            authorizedScopeMarkers: new[] { "docs-only" },
            authorityReference: "policy-ref",
            authority);

        Assert.Equal(DevelopmentWorkAuthorizationDisposition.Active, granted.Disposition);
        Assert.Equal(DevelopmentWorkAuthorizationKind.Planning, granted.AuthorizationKind);
        Assert.Equal("tranche-a", granted.AuthorizedTrancheId);
        Assert.Equal(authority.CorrelationId, granted.GrantCorrelationId);
    }

    [Fact]
    public void Grant_SupersedesExistingActive_ForSameContext()
    {
        var (services, instance) = CreateInstanceWithProfile();
        var first = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var second = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.NotEqual(first.AuthorizationId, second.AuthorizationId);
        var reloaded = services.WorkStateRecovery.RecoverForProject(instance.ProjectId);
        var firstReloaded = reloaded.DevelopmentWorkAuthorizations.Single(a => a.AuthorizationId == first.AuthorizationId);
        Assert.Equal(DevelopmentWorkAuthorizationDisposition.Superseded, firstReloaded.Disposition);
        Assert.Equal(DevelopmentWorkAuthorizationDisposition.Active, second.Disposition);
    }

    [Fact]
    public void PlanningAndImplementation_MayBothBeActive()
    {
        var (services, instance) = CreateInstanceWithProfile();
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));
        services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var projection = services.WorkStateOperatorProjection.ProjectForProject(instance.ProjectId, null);
        var work = projection.CurrentWork.Single();
        Assert.Contains(DevelopmentWorkAuthorizationKind.Planning, work.ApplicableActiveAuthorizationKinds);
        Assert.Contains(DevelopmentWorkAuthorizationKind.Implementation, work.ApplicableActiveAuthorizationKinds);
    }

    [Fact]
    public void Supersede_Succeeds_WhenProfileIdMissing_OnInstance()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var (services, instance) = CreateInstanceWithProfile(persistence);
        var grantAuthority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            grantAuthority);

        var withoutProfile = instance with { ProfileId = null, ResourceVersion = instance.ResourceVersion + 1 };
        Assert.True(persistence.WorkflowInstances.TryUpdateWithExpectedVersion(withoutProfile, instance.ResourceVersion));

        var supersedeAuthority = GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New());
        var superseded = services.DevelopmentWorkAuthorizations.RecordGovernedSupersede(
            granted.AuthorizationId,
            granted.ResourceVersion,
            supersedeAuthority);

        Assert.Equal(DevelopmentWorkAuthorizationDisposition.Superseded, superseded.Disposition);
        Assert.Equal(supersedeAuthority.CorrelationId, superseded.SupersessionCorrelationId);
    }

    [Fact]
    public void Supersede_Succeeds_WhenProfileIdUnknown_OnInstance()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var (services, instance) = CreateInstanceWithProfile(persistence);
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Implementation,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var unknownProfile = instance with
        {
            ProfileId = WorkflowProfileId.Parse("gew.unknown"),
            ResourceVersion = instance.ResourceVersion + 1,
        };
        Assert.True(persistence.WorkflowInstances.TryUpdateWithExpectedVersion(unknownProfile, instance.ResourceVersion));

        var superseded = services.DevelopmentWorkAuthorizations.RecordGovernedSupersede(
            granted.AuthorizationId,
            granted.ResourceVersion,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.Equal(DevelopmentWorkAuthorizationDisposition.Superseded, superseded.Disposition);
    }

    [Fact]
    public void Supersede_RequiresValidGovernedMutationAuthority()
    {
        var (services, instance) = CreateInstanceWithProfile();
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.Throws<ArgumentNullException>(() =>
            services.DevelopmentWorkAuthorizations.RecordGovernedSupersede(
                granted.AuthorizationId,
                granted.ResourceVersion,
                authority: null!));
    }

    [Fact]
    public void Supersede_StaleResourceVersion_FailsClosed()
    {
        var (services, instance) = CreateInstanceWithProfile();
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        Assert.Throws<DevelopmentWorkAuthorizationConcurrencyException>(() =>
            services.DevelopmentWorkAuthorizations.RecordGovernedSupersede(
                granted.AuthorizationId,
                0,
                GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New())));
    }

    [Fact]
    public void Supersede_DoesNotMutateRecord_BeforeSuccessfulSupersession()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var (services, instance) = CreateInstanceWithProfile(persistence);
        var granted = services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
            instance.InstanceId,
            DevelopmentWorkAuthorizationKind.Planning,
            null,
            null,
            null,
            GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New()));

        var withoutProfile = instance with { ProfileId = null, ResourceVersion = instance.ResourceVersion + 1 };
        Assert.True(persistence.WorkflowInstances.TryUpdateWithExpectedVersion(withoutProfile, instance.ResourceVersion));

        var before = persistence.DevelopmentWorkAuthorizations.GetById(granted.AuthorizationId)!;
        Assert.Throws<DevelopmentWorkAuthorizationConcurrencyException>(() =>
            services.DevelopmentWorkAuthorizations.RecordGovernedSupersede(
                granted.AuthorizationId,
                0,
                GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New())));

        var afterFailed = persistence.DevelopmentWorkAuthorizations.GetById(granted.AuthorizationId)!;
        Assert.Equal(before.Disposition, afterFailed.Disposition);
        Assert.Equal(before.ResourceVersion, afterFailed.ResourceVersion);
        Assert.Null(afterFailed.SupersessionCorrelationId);
    }

    [Fact]
    public void Grant_WithoutResolvableProfile_FailsClosed()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-dwa-").FullName),
            "dwa",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var instance = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null);

        Assert.Throws<EffectiveConfigurationRequiredException>(() =>
            services.DevelopmentWorkAuthorizations.RecordGovernedGrant(
                instance.InstanceId,
                DevelopmentWorkAuthorizationKind.Planning,
                null,
                null,
                null,
                GovernedWorkflowMutationAuthorityTestSupport.ForTestHarness(GovernedCorrelationId.New())));
    }

    private static (WorkflowApplicationServices Services, WorkflowInstance Instance) CreateInstanceWithProfile(
        InMemoryUserApplicationStatePersistence? persistence = null)
    {
        persistence ??= new InMemoryUserApplicationStatePersistence();
        var project = persistence.ProjectRegistry.RegisterNewProjectAtLocator(
            ProjectLocator.FromPath(Directory.CreateTempSubdirectory("edf-dwa-").FullName),
            "dwa",
            DateTimeOffset.UtcNow);
        var services = WorkflowApplicationServicesFactory.Create(persistence);
        var instance = services.WorkflowInstances.CreateGewInstance(
            project.ProjectId,
            GovernedCorrelationId.New(),
            null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
        return (services, instance);
    }
}
