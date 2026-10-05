using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed record GewV1EffectiveConfigurationView(
    PrescribedWorkflowId WorkflowId,
    WorkflowDefinitionVersion DefinitionVersion,
    WorkflowProfileId ProfileId,
    IReadOnlyList<GewV1EffectiveConfigurationDwaBound> ApplicableDwaBounds,
    bool ProfileSynchronizationFloorsUnspecified,
    bool ProfileEvidenceFloorsUnspecified);

public sealed record GewV1EffectiveConfigurationDwaBound(
    DevelopmentWorkAuthorizationKind AuthorizationKind,
    string? AuthorizedTrancheId,
    IReadOnlyList<string> AuthorizedScopeMarkers);
