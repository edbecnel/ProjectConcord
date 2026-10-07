using Edf.Application.Relay;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Application.Tests.Workflow.PlanningAuthorization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayHumanReadablePackageProjectorTests
{
    private const string RequestedWork = "EXACT_REQUESTED_WORK_SUMMARY_FOR_EA";
    private const string DispositionSummary = "EXACT_DISPOSITION_SUMMARY_TEXT";
    private const string InheritedContext = "EXACT_INHERITED_HANDOVER_CONTEXT";

    [Fact]
    public void Project_IsDeterministic_ForSamePackageAndOptions()
    {
        var package = CreateEaExportPackage();
        var options = new GovernedRelayHumanReadablePackageOptions(
            GovernedRelayHumanReadableCounterparty.EngineeringAgent,
            "Operator subject line");

        var first = GovernedRelayHumanReadablePackageProjector.Project(package, options);
        var second = GovernedRelayHumanReadablePackageProjector.Project(package, options);

        Assert.Equal(first, second);
    }

    [Fact]
    public void Project_IncludesVerbatimWorkDispositionAndInheritedContext()
    {
        var package = CreateEaExportPackage();
        var text = GovernedRelayHumanReadablePackageProjector.Project(
            package,
            new GovernedRelayHumanReadablePackageOptions(
                GovernedRelayHumanReadableCounterparty.EngineeringAgent));

        Assert.Contains(RequestedWork, text, StringComparison.Ordinal);
        Assert.Contains(DispositionSummary, text, StringComparison.Ordinal);
        Assert.Contains(InheritedContext, text, StringComparison.Ordinal);
        Assert.Contains("Inherited handover context (not a new instruction):", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Project_RepresentsPlanningNotImplementation_AndStopInactive()
    {
        var package = CreateEaExportPackage();
        var text = GovernedRelayHumanReadablePackageProjector.Project(
            package,
            new GovernedRelayHumanReadablePackageOptions(
                GovernedRelayHumanReadableCounterparty.EngineeringAgent));

        Assert.Contains("Repository implementation is not authorized", text, StringComparison.Ordinal);
        Assert.Contains("Planning authorized: Yes", text, StringComparison.Ordinal);
        Assert.Contains("Implementation authorized: No", text, StringComparison.Ordinal);
        Assert.Contains("STOP is not active", text, StringComparison.Ordinal);
        Assert.Contains("Engineering Agent", text, StringComparison.Ordinal);
        Assert.Contains("Plan", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Project_OrdinaryView_ExcludesMachineJsonFence()
    {
        var package = CreateEaExportPackage();
        var text = GovernedRelayHumanReadablePackageProjector.Project(
            package,
            new GovernedRelayHumanReadablePackageOptions(
                GovernedRelayHumanReadableCounterparty.EngineeringAgent));

        Assert.DoesNotContain(GovernedRelayV1Format.MachineBlockFenceLanguage, text, StringComparison.Ordinal);
        Assert.DoesNotContain("```", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Project_OperatorWorkSubject_IsPresentationOnly()
    {
        var package = CreateEaExportPackage();
        var text = GovernedRelayHumanReadablePackageProjector.Project(
            package,
            new GovernedRelayHumanReadablePackageOptions(
                GovernedRelayHumanReadableCounterparty.EngineeringAgent,
                "MVR-0005 disposable subject"));

        Assert.Contains("not a canonical package field", text, StringComparison.Ordinal);
        Assert.Contains("MVR-0005 disposable subject", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Project_DoesNotMutatePackage()
    {
        var package = CreateEaExportPackage();
        var kindBefore = package.Kind;
        _ = GovernedRelayHumanReadablePackageProjector.Project(
            package,
            new GovernedRelayHumanReadablePackageOptions(
                GovernedRelayHumanReadableCounterparty.EngineeringAgent));
        Assert.Equal(kindBefore, package.Kind);
    }

    private static GovernedRelayPackage CreateEaExportPackage()
    {
        var projectId = new ProjectConcordProjectId(Guid.NewGuid());
        var handover = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(
            projectId,
            payload => payload with
            {
                WorkContext = new WorkContextProjection(null, RequestedWork),
                HandoverContext = new HandoverContextProjection(InheritedContext, IsInheritedContextOnly: true),
                AuthorizationDisposition = payload.AuthorizationDisposition! with
                {
                    DispositionSummary = DispositionSummary,
                },
            }) with
        {
            GovernanceCritical = PlanningAuthorizationRelayFixtures.CreateQualifyingPlanningAuthorizationHandover(projectId)
                .GovernanceCritical with
            {
                WorkContextPresent = true,
            },
        };

        var bridge = new EngineeringAgentManualRelayBridge();
        var preparation = bridge.TryRenderValidatedHandover(handover, RelayValidationResult.Valid([]));
        Assert.True(preparation.IsReadyForManualTransfer);
        return preparation.ExportPackage!;
    }
}
