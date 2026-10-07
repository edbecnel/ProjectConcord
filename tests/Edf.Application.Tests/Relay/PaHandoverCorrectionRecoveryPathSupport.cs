using Edf.Application.Operator;
using Edf.Application.Projects.InMemory;
using Edf.Application.Relay;
using Edf.Application.Relay.Serialization;
using Edf.Domain.Projects;
using Edf.Domain.Relay;
using Edf.Engine.Projects;
using Edf.Identity.Actors;
using Edf.ProjectServices.Local;

namespace Edf.Application.Tests.Relay;

/// <summary>
/// Shared helpers for integrated PA handover correction/recovery path tests.
/// </summary>
public static class PaHandoverCorrectionRecoveryPathSupport
{
    public sealed record WorkflowWithReview(
        IGovernedRelayP0WorkflowService Service,
        ProjectConcordProjectId ProjectId,
        ProjectRoot Root,
        InMemoryUserApplicationStatePersistence Persistence,
        GovernedRelayPackage TrustedReviewExport);

    internal sealed record RecoveryPathObservation(
        PaHandoverImportOperationResult ValidateResult,
        PaHandoverCorrectionFailureClass FailureClass,
        string CorrectionRequest);

    public static WorkflowWithReview CreateWorkflowWithTrustedPlanningAuthorizationReview()
    {
        var persistence = new InMemoryUserApplicationStatePersistence();
        var service = GovernedRelayP0WorkflowService.Create(persistence);
        var resolver = new ProjectRootResolver();
        var workspace = new Edf.Application.Projects.ProjectWorkspaceService(
            resolver,
            new DegenerateAdministratorActor(),
            persistence,
            new LocalProjectRuntime());
        var dir = Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), "edf-pa-recovery-" + Guid.NewGuid().ToString("N")));
        var open = workspace.OpenProjectRoot(dir.FullName);
        if (!open.Success)
        {
            throw new InvalidOperationException("Failed to open disposable project root.");
        }

        var projectId = open.ProjectId!.Value;
        service.SetProjectArchitectSessionIntent(projectId, AgentSessionIntent.New);
        service.SetEngineeringAgentSessionIntent(projectId, AgentSessionIntent.Continue);
        var export = service.GeneratePaReviewExport(
            projectId,
            open.Root!,
            new RelayPaReviewExportOptions(
                EngineeringAgentMode.Plan,
                null,
                PaHandoverResponseProfile.PlanningDevelopmentWorkAuthorization));
        if (export.PackageId is null)
        {
            throw new InvalidOperationException("Review export package id missing.");
        }

        var review = persistence.RelayOperational.GetPackage(export.PackageId.Value)!.Package;
        return new WorkflowWithReview(service, projectId, open.Root!, persistence, review);
    }

    internal static RecoveryPathObservation RunRecoveryPath(
        WorkflowWithReview workflow,
        string pastedText,
        PaHandoverResponseProfile responseProfile,
        PaHandoverCorrectionFailureClass expectedFailureClass)
    {
        var before = CountPackageConsumedEvents(workflow.Persistence, workflow.ProjectId);
        var validateResult = workflow.Service.TryValidatePaHandoverImport(
            workflow.ProjectId,
            pastedText,
            workflow.TrustedReviewExport.CorrelationId);

        var after = CountPackageConsumedEvents(workflow.Persistence, workflow.ProjectId);
        if (before != after)
        {
            throw new InvalidOperationException(
                $"PackageConsumed provenance changed from {before} to {after} during TryValidate.");
        }

        var failureClass = PaHandoverExchangeCorrectionSupport.ClassifyValidationFailure(validateResult);
        if (failureClass != expectedFailureClass)
        {
            throw new InvalidOperationException(
                $"Expected failure class {expectedFailureClass} but got {failureClass}.");
        }

        if (!PaHandoverExchangeCorrectionSupport.ShouldOfferCorrectionRequest(failureClass))
        {
            throw new InvalidOperationException("Correction request was not offered for retryable failure.");
        }

        var correctionRequest = GovernedRelayPaHandoverCorrectionRequest.RenderCompleteCorrectionRequest(
            workflow.TrustedReviewExport,
            responseProfile,
            failureClass);

        return new RecoveryPathObservation(validateResult, failureClass, correctionRequest);
    }

    internal static void AssertTrustedIdentityInCorrectionRequest(
        GovernedRelayPackage trustedReview,
        PaHandoverResponseProfile profile,
        string correctionRequest)
    {
        Assert.Contains(trustedReview.ProjectId.Value.ToString(), correctionRequest, StringComparison.Ordinal);
        Assert.Contains(trustedReview.CorrelationId.Value.ToString(), correctionRequest, StringComparison.Ordinal);
        Assert.Contains(trustedReview.PackageId.Value.ToString(), correctionRequest, StringComparison.Ordinal);
        Assert.Contains("PLANNING DEVELOPMENT WORK AUTHORIZATION", correctionRequest, StringComparison.Ordinal);
        if (profile == PaHandoverResponseProfile.PlanningEntry)
        {
            Assert.Contains("PLANNING ENTRY", correctionRequest, StringComparison.Ordinal);
        }
    }

    internal static void AssertUntrustedSubstringsAbsent(string correctionRequest, IEnumerable<string> untrusted)
    {
        foreach (var fragment in untrusted)
        {
            Assert.DoesNotContain(fragment, correctionRequest, StringComparison.Ordinal);
        }
    }

    /// <summary>
    /// Mirrors guided-exchange commit gate in Planning Authorization / Planning Entry VMs.
    /// </summary>
    internal static bool GuidedExchangeWouldCommitAfterValidate(PaHandoverImportOperationResult result) =>
        PaHandoverExchangeCorrectionSupport.IsAuthorizedForGuidedDurableConsumption(result);

    internal static int CountPackageConsumedEvents(
        InMemoryUserApplicationStatePersistence persistence,
        ProjectConcordProjectId projectId) =>
        persistence.RelayOperational
            .ListProvenanceEvents(projectId)
            .Count(e => e.EventType == RelayProvenanceEventType.PackageConsumed);
}
