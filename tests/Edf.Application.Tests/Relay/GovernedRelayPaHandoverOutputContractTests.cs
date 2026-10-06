using Edf.Application.Composition;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Workflow;
using Edf.Application.Workflow.PlanningEntry;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Domain.Workflow;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Tests.Relay;

public class GovernedRelayPaHandoverOutputContractTests
{
    private readonly ProjectArchitectManualAdapter _paAdapter = new();

    [Fact]
    public void PlanningEntry_PaReviewExport_ContainsCompleteOutputContract()
    {
        var harness = GeneratePlanningEntryReviewExport();
        var reviewPackage = harness.ReviewPackage;
        var rendered = harness.Rendered;

        Assert.Contains(GovernedRelayPaHandoverOutputContract.SectionHeading, rendered, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayPaHandoverOutputContract.ContractVersion, rendered, StringComparison.Ordinal);
        Assert.Contains("PLANNING ENTRY", rendered, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.RenderVersionLinePrefix, rendered, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayV1Format.MachineBlockFenceLanguage, rendered, StringComparison.Ordinal);
        Assert.Contains("paHandoverImport", rendered, StringComparison.Ordinal);
        Assert.Contains(reviewPackage.ProjectId.Value.ToString(), rendered, StringComparison.Ordinal);
        Assert.Contains(reviewPackage.CorrelationId.Value.ToString(), rendered, StringComparison.Ordinal);
        Assert.Contains("softwareDevelopmentProfile", rendered, StringComparison.Ordinal);
        Assert.Contains("authorizationDisposition", rendered, StringComparison.Ordinal);
        Assert.Contains("Machine / projection agreement", rendered, StringComparison.Ordinal);
        Assert.Contains("RETURN ONLY ONE OUTER PLAIN-TEXT COPY SURFACE", rendered, StringComparison.Ordinal);
        Assert.Contains($"```{GovernedRelayManualPasteCopyFence.OuterFenceLanguage}", rendered, StringComparison.Ordinal);
        Assert.Contains(GovernedRelayPaHandoverOutputContract.PlaceholderPackageId, rendered, StringComparison.Ordinal);
        Assert.DoesNotContain("Cursor", rendered, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("ACP", rendered, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void InstructionalTemplate_IsRejected()
    {
        var reviewPackage = GeneratePlanningEntryReviewExport().ReviewPackage;
        var template = GovernedRelayPaHandoverOutputContract.RenderStructuralTemplateWithPlaceholders(
            reviewPackage,
            PaHandoverResponseProfile.PlanningEntry);

        var result = _paAdapter.TryParsePaHandoverImport(template);

        Assert.NotEqual(RelayValidationState.Valid, result.Validation.State);
    }

    [Fact]
    public void CompletedPositivePlanningEntry_ImportsValid_AndSatisfiesPlanningEntryContract()
    {
        var reviewPackage = GeneratePlanningEntryReviewExport().ReviewPackage;
        var completed = GovernedRelayPaHandoverOutputContract.RenderCompletedPlanningEntryHandover(
            reviewPackage,
            planningAuthorized: true,
            "Planning entry authorized for governed planning stage.");

        var result = _paAdapter.TryParsePaHandoverImport(completed);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.Package);
        Assert.True(
            PlanningEntryPaHandoverContract.SatisfiesPlanningEntryContract(
                result.Package!,
                result.Validation,
                out _));
    }

    [Fact]
    public void CompletedNegativePlanningEntry_ImportsValid_ButNotPlanningEntryEligible()
    {
        var harness = GeneratePlanningEntryReviewExport();
        var persistence = harness.Persistence;
        var projectId = harness.ProjectId;
        var reviewPackage = harness.ReviewPackage;
        var completed = GovernedRelayPaHandoverOutputContract.RenderCompletedPlanningEntryHandover(
            reviewPackage,
            planningAuthorized: false,
            "Planning entry not authorized at this time.");

        var result = _paAdapter.TryParsePaHandoverImport(completed);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.Package);
        Assert.False(
            PlanningEntryPaHandoverContract.SatisfiesPlanningEntryContract(
                result.Package!,
                result.Validation,
                out var reason));
        Assert.Equal(PlanningEntryTransitionReasonCodes.ContractNotSatisfied, reason);

        var workflow = WorkflowApplicationServicesFactory.Create(persistence);
        CreateIntakeInstance(workflow, projectId);
        var eligibility = workflow.IntakePlanningEntryTransitions.EvaluateEligibility(
            projectId,
            result.Package,
            result.Validation);
        Assert.False(eligibility.IsEligible);
    }

    [Fact]
    public void ImplementationDirected_CompletedExample_ImportsValid_ForEngineeringHandover()
    {
        var reviewPackage = GenerateImplementationDirectedReviewExport().ReviewPackage;
        var completed = GovernedRelayPaHandoverOutputContract.RenderCompletedImplementationDirectedHandover(
            reviewPackage,
            "IMPLEMENTATION AUTHORIZED",
            "A2-MVR",
            "Authorized tranche work");

        var result = _paAdapter.TryParsePaHandoverImport(completed);

        Assert.Equal(RelayValidationState.Valid, result.Validation.State);
        Assert.NotNull(result.Package);
        Assert.True(result.Validation.IsEligibleForValidatedEngineeringAgentHandover);
        Assert.False(
            PlanningEntryPaHandoverContract.SatisfiesPlanningEntryContract(
                result.Package!,
                result.Validation,
                out _));
    }

    [Fact]
    public void GeneratePaReviewExport_IncludesContractInSameRenderedPayload()
    {
        var rendered = GeneratePlanningEntryReviewExport().Rendered;

        Assert.Contains(GovernedPackageKind.PaReviewExport.ToString(), rendered, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(GovernedRelayPaHandoverOutputContract.SectionHeading, rendered, StringComparison.Ordinal);
    }

    private ReviewExportHarness GeneratePlanningEntryReviewExport() =>
        GenerateReviewExport(
            EngineeringAgentMode.Plan,
            null,
            PaHandoverResponseProfile.PlanningEntry);

    private ReviewExportHarness GenerateImplementationDirectedReviewExport()
    {
        var (persistence, service, projectId, root) = CreateService();
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.Continue);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.New);

        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(
                EngineeringAgentMode.Agent,
                EngineeringAgentMode.Plan,
                PaHandoverResponseProfile.ImplementationDirected));

        Assert.NotNull(result.RenderedPackage);
        var reviewPackage = ParseReviewPackageFromExport(result.RenderedPackage!);
        return new ReviewExportHarness(persistence, projectId, reviewPackage, result.RenderedPackage!);
    }

    private ReviewExportHarness GenerateReviewExport(
        EngineeringAgentMode mode,
        EngineeringAgentMode? priorMode,
        PaHandoverResponseProfile responseProfile)
    {
        var (persistence, service, projectId, root) = CreateService();
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.Continue);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);

        var result = service.GeneratePaReviewExport(
            projectId,
            root,
            new RelayPaReviewExportOptions(mode, priorMode, responseProfile));

        Assert.NotNull(result.RenderedPackage);
        Assert.Equal(RelayValidationState.Valid, result.Validation.State);

        var reviewPackage = ParseReviewPackageFromExport(result.RenderedPackage!);
        return new ReviewExportHarness(persistence, projectId, reviewPackage, result.RenderedPackage!);
    }

    private sealed record ReviewExportHarness(
        InMemoryUserApplicationStatePersistence Persistence,
        ProjectConcordProjectId ProjectId,
        GovernedRelayPackage ReviewPackage,
        string Rendered);

    private static GovernedRelayPackage ParseReviewPackageFromExport(string fullRendered)
    {
        var machineEnd = fullRendered.IndexOf(GovernedRelayPaHandoverOutputContract.SectionHeading, StringComparison.Ordinal);
        var reviewOnly = machineEnd > 0
            ? fullRendered[..machineEnd]
            : fullRendered;
        var importer = new GovernedRelayV1Importer();
        var attempt = importer.Import(reviewOnly.TrimEnd());
        if (attempt.Package is null)
        {
            throw new InvalidOperationException(
                "Failed to parse review export: "
                + string.Join("; ", attempt.StructuralResult.Diagnostics.Select(d => d.Message)));
        }

        Assert.Equal(GovernedPackageKind.PaReviewExport, attempt.Package.Kind);
        return attempt.Package;
    }

    private static void CreateIntakeInstance(WorkflowApplicationServices workflow, ProjectConcordProjectId projectId)
    {
        workflow.WorkflowInstances.CreateGewInstance(
            projectId,
            GovernedCorrelationId.New(),
            projectRootAbsolutePath: null,
            WorkflowProfileId.Parse(GewV1ProfileIds.Standard));
    }

    private static (InMemoryUserApplicationStatePersistence Persistence, IGovernedRelayP0WorkflowService Service, ProjectConcordProjectId ProjectId, ProjectRoot Root) CreateService()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var service = GovernedRelayP0WorkflowService.Create(persistence);
        var workspace = new Edf.Application.Projects.ProjectWorkspaceService(
            new ProjectRootResolver(),
            new DegenerateAdministratorActor(),
            persistence,
            new LocalProjectRuntime());
        var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-pa-contract-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        Assert.True(open.Success);
        return (persistence, service, open.ProjectId!.Value, open.Root!);
    }
}
