namespace Edf.Application.Relay;

using Edf.Application.Projects;
using Edf.Application.Relay.EngineeringAgent;
using Edf.Application.Relay.ProjectArchitect;
using Edf.Application.Relay.Serialization;
using Edf.Application.Relay.SoftwareDevelopment;
using Edf.Domain.Projects;
using Edf.Domain.Relay;

public sealed class GovernedRelayP0WorkflowService : IGovernedRelayP0WorkflowService
{
    private readonly IGovernedInteractionRelayService _relay;
    private readonly IProjectArchitectProvider _projectArchitect;
    private readonly IEngineeringAgentRelayBridge _engineeringAgent;
    private readonly GovernedRelayPackageValidator _validator;
    private readonly ITier0RelaySnapshotProvider _tier0;
    private readonly TimeProvider _clock;

    public GovernedRelayP0WorkflowService(
        IGovernedInteractionRelayService relay,
        IProjectArchitectProvider projectArchitect,
        IEngineeringAgentRelayBridge engineeringAgent,
        ITier0RelaySnapshotProvider tier0,
        TimeProvider? clock = null)
        : this(
            relay,
            projectArchitect,
            engineeringAgent,
            new GovernedRelayPackageValidator(SoftwareDevelopmentRelayProfileValidator.Instance),
            tier0,
            clock)
    {
    }

    public GovernedRelayP0WorkflowService(
        IGovernedInteractionRelayService relay,
        IProjectArchitectProvider projectArchitect,
        IEngineeringAgentRelayBridge engineeringAgent,
        GovernedRelayPackageValidator validator,
        ITier0RelaySnapshotProvider tier0,
        TimeProvider? clock = null)
    {
        _relay = relay ?? throw new ArgumentNullException(nameof(relay));
        _projectArchitect = projectArchitect ?? throw new ArgumentNullException(nameof(projectArchitect));
        _engineeringAgent = engineeringAgent ?? throw new ArgumentNullException(nameof(engineeringAgent));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _tier0 = tier0 ?? throw new ArgumentNullException(nameof(tier0));
        _clock = clock ?? TimeProvider.System;
    }

    public static GovernedRelayP0WorkflowService Create(IUserApplicationStatePersistence persistence, TimeProvider? clock = null)
    {
        ArgumentNullException.ThrowIfNull(persistence);
        var relay = new GovernedInteractionRelayService(persistence.RelayOperational, new Tier0RelaySnapshotProvider(), clock);
        return new GovernedRelayP0WorkflowService(
            relay,
            new ProjectArchitectManualAdapter(),
            new EngineeringAgentManualRelayBridge(),
            new Tier0RelaySnapshotProvider(),
            clock);
    }

    public RelayP0SessionState GetSessionState(ProjectConcordProjectId projectId)
    {
        var continuity = _relay.GetSessionContinuity(projectId);
        return new RelayP0SessionState(
            continuity.ProjectArchitectSessionIntent,
            continuity.EngineeringAgentSessionIntent);
    }

    public void SetProjectArchitectSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent) =>
        _relay.SetProjectArchitectSessionIntent(projectId, intent);

    public void SetEngineeringAgentSessionIntent(ProjectConcordProjectId projectId, AgentSessionIntent intent) =>
        _relay.SetEngineeringAgentSessionIntent(projectId, intent);

    public PaReviewExportOperationResult GeneratePaReviewExport(
        ProjectConcordProjectId projectId,
        ProjectRoot projectRoot,
        RelayPaReviewExportOptions options)
    {
        ArgumentNullException.ThrowIfNull(projectRoot);
        ArgumentNullException.ThrowIfNull(options);

        var continuity = _relay.GetSessionContinuity(projectId);
        if (continuity.ProjectArchitectSessionIntent is null || continuity.EngineeringAgentSessionIntent is null)
        {
            return new PaReviewExportOperationResult(
                null,
                RelayValidationResult.Incomplete(
                [
                    new RelayValidationDiagnostic(
                        RelayValidationCodes.ProjectArchitectSessionIntentMissing,
                        "Both Project Architect and Engineering Agent session intents must be set explicitly before generating a PA review package.",
                        RelayValidationDiagnosticSeverity.Incomplete),
                ]),
                null);
        }

        var utc = _clock.GetUtcNow();
        var modeTransition = BuildModeTransition(options.EngineeringAgentMode, options.PriorEngineeringAgentMode);
        var governance = new RelayGovernanceCriticalState(
            options.EngineeringAgentMode,
            options.PriorEngineeringAgentMode,
            modeTransition,
            continuity,
            RelayStopMetadata.None,
            AuthorizationDispositionPresent: false,
            WorkContextPresent: false,
            new RelayGovernanceDirectiveFlags(false, false),
            EdfCorrelation: null);

        var package = new GovernedRelayPackage(
            GovernedPackageId.New(),
            GovernedCorrelationId.New(),
            GovernedPackageKind.PaReviewExport,
            RelaySchemaVersion.Current,
            RelayRenderVersion.V1,
            projectId,
            utc,
            utc,
            governance,
            Tier0Snapshot: null,
            SoftwareDevelopmentProfilePayloadSerializer.Serialize(SoftwareDevelopmentProfilePayloadSerializer.Empty),
            GovernedRelayPackage.DefaultStructuralAgreement);

        var validation = _validator.Validate(package);
        if (validation.State != RelayValidationState.Valid)
        {
            return new PaReviewExportOperationResult(null, validation, null);
        }

        var recorded = _relay.RecordProducedPackage(
            package,
            validation,
            projectRoot,
            renderedBodyHash: null,
            paReviewResponseProfile: options.PaHandoverResponseProfile);
        var reviewBody = _projectArchitect.RenderPaReviewPackage(recorded.Package);
        var contract = GovernedRelayPaHandoverResponseInstruction.Render(
            recorded.Package,
            options.PaHandoverResponseProfile);
        var rendered = reviewBody.TrimEnd()
            + Environment.NewLine
            + Environment.NewLine
            + contract
            + Environment.NewLine;
        return new PaReviewExportOperationResult(rendered, validation, recorded.Package.PackageId);
    }

    public PaHandoverImportOperationResult TryValidatePaHandoverImport(
        ProjectConcordProjectId projectId,
        string renderedText,
        GovernedCorrelationId? requiredReviewCorrelationId = null)
    {
        var import = _projectArchitect.TryParsePaHandoverImport(renderedText);
        if (import.Package is null)
        {
            return new PaHandoverImportOperationResult(import, ProjectIdMatched: true, ProjectIdMismatchMessage: null);
        }

        if (import.Package.ProjectId != projectId)
        {
            return new PaHandoverImportOperationResult(
                import,
                ProjectIdMatched: false,
                ProjectIdMismatchMessage:
                "Imported package project id does not match the active ProjectConcord project.");
        }

        if (requiredReviewCorrelationId is not null
            && import.Package.CorrelationId != requiredReviewCorrelationId)
        {
            var mismatch = RelayValidationResult.RejectedMalformed(
            [
                new RelayValidationDiagnostic(
                    RelayValidationCodes.HandoverCorrelationMismatch,
                    "Handover correlation id does not match the active PA review export.",
                    RelayValidationDiagnosticSeverity.Malformed),
            ]);
            return new PaHandoverImportOperationResult(
                new PaHandoverImportResult(import.Package, mismatch),
                ProjectIdMatched: true,
                ProjectIdMismatchMessage: null);
        }

        return new PaHandoverImportOperationResult(import, ProjectIdMatched: true, ProjectIdMismatchMessage: null);
    }

    public PaHandoverImportOperationResult CommitConsumedPaHandoverImport(
        ProjectConcordProjectId projectId,
        GovernedRelayPackage package,
        RelayValidationResult validation)
    {
        ArgumentNullException.ThrowIfNull(package);
        ArgumentNullException.ThrowIfNull(validation);

        if (package.ProjectId != projectId)
        {
            return new PaHandoverImportOperationResult(
                new PaHandoverImportResult(package, validation),
                ProjectIdMatched: false,
                ProjectIdMismatchMessage:
                "Imported package project id does not match the active ProjectConcord project.");
        }

        _relay.RecordConsumedPackage(package, validation);
        return new PaHandoverImportOperationResult(
            new PaHandoverImportResult(package, validation),
            ProjectIdMatched: true,
            ProjectIdMismatchMessage: null);
    }

    public PaHandoverImportOperationResult ImportPaHandover(ProjectConcordProjectId projectId, string renderedText)
    {
        var validated = TryValidatePaHandoverImport(projectId, renderedText);
        if (validated.Import.Package is null
            || !validated.ProjectIdMatched
            || validated.Import.Validation.State == RelayValidationState.RejectedMalformed)
        {
            return validated;
        }

        return CommitConsumedPaHandoverImport(
            projectId,
            validated.Import.Package,
            validated.Import.Validation);
    }

    public EngineeringAgentHandoverPreparationResult PrepareEngineeringAgentHandover(
        GovernedRelayPackage validatedImportPackage,
        RelayValidationResult importValidation)
    {
        ArgumentNullException.ThrowIfNull(validatedImportPackage);
        ArgumentNullException.ThrowIfNull(importValidation);

        var preparation = _engineeringAgent.TryRenderValidatedHandover(validatedImportPackage, importValidation);
        if (preparation.IsReadyForManualTransfer
            && preparation.ExportPackage is not null
            && preparation.RenderedHandover is not null)
        {
            _relay.RecordProducedPackage(
                preparation.ExportPackage,
                preparation.Validation,
                renderedBodyHash: null);
        }

        return preparation;
    }

    public EngineeringResultImportOperationResult ImportEngineeringResult(
        ProjectConcordProjectId projectId,
        string renderedText)
    {
        var import = _engineeringAgent.TryParseEngineeringResult(renderedText);
        if (import.Package is null)
        {
            return new EngineeringResultImportOperationResult(import, ProjectIdMatched: true, ProjectIdMismatchMessage: null);
        }

        if (import.Package.ProjectId != projectId)
        {
            return new EngineeringResultImportOperationResult(
                import,
                ProjectIdMatched: false,
                ProjectIdMismatchMessage:
                "Imported engineering result project id does not match the active ProjectConcord project.");
        }

        _relay.RecordConsumedPackage(import.Package, import.Validation);
        return new EngineeringResultImportOperationResult(import, ProjectIdMatched: true, ProjectIdMismatchMessage: null);
    }

    public IReadOnlyList<RelayProvenanceEvent> ListRecentProvenance(ProjectConcordProjectId projectId, int maxEvents = 20)
    {
        if (maxEvents <= 0)
        {
            return Array.Empty<RelayProvenanceEvent>();
        }

        var events = _relay.ListProvenanceEvents(projectId);
        if (events.Count <= maxEvents)
        {
            return events;
        }

        return events.Skip(events.Count - maxEvents).ToArray();
    }

    private static EngineeringAgentModeTransition? BuildModeTransition(
        EngineeringAgentMode current,
        EngineeringAgentMode? prior)
    {
        if (prior is null || prior == current)
        {
            return null;
        }

        return new EngineeringAgentModeTransition(prior.Value, current);
    }
}
