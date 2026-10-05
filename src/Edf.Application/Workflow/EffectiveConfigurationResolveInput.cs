using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public sealed record EffectiveConfigurationResolveInput(
    WorkflowInstance Instance,
    IReadOnlyList<DevelopmentWorkAuthorization> AuthorizationRecordsForInstance);
