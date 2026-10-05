using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;

namespace Edf.Application.Projects.Sqlite;

internal sealed class SqliteDevelopmentWorkAuthorizationStoreAdapter : IDevelopmentWorkAuthorizationStore
{
    private readonly SqliteUserApplicationStateStore _store;

    public SqliteDevelopmentWorkAuthorizationStoreAdapter(SqliteUserApplicationStateStore store) =>
        _store = store ?? throw new ArgumentNullException(nameof(store));

    public void Insert(DevelopmentWorkAuthorization authorization) =>
        _store.InsertDevelopmentWorkAuthorization(authorization);

    public DevelopmentWorkAuthorization? GetById(DevelopmentWorkAuthorizationId authorizationId) =>
        _store.GetDevelopmentWorkAuthorization(authorizationId);

    public IReadOnlyList<DevelopmentWorkAuthorization> ListByProject(ProjectConcordProjectId projectId) =>
        _store.ListDevelopmentWorkAuthorizations(projectId);

    public IReadOnlyList<DevelopmentWorkAuthorization> ListByWorkflowInstance(WorkflowInstanceId workflowInstanceId) =>
        _store.ListDevelopmentWorkAuthorizationsByInstance(workflowInstanceId);

    public DevelopmentWorkAuthorization? TryGetActiveByContext(
        WorkflowInstanceId workflowInstanceId,
        DevelopmentWorkAuthorizationKind authorizationKind,
        string normalizedTrancheKey) =>
        _store.TryGetActiveDevelopmentWorkAuthorizationByContext(
            workflowInstanceId,
            authorizationKind,
            normalizedTrancheKey);

    public bool TryUpdateWithExpectedVersion(DevelopmentWorkAuthorization authorization, long expectedResourceVersion) =>
        _store.TryUpdateDevelopmentWorkAuthorizationWithExpectedVersion(authorization, expectedResourceVersion);
}
