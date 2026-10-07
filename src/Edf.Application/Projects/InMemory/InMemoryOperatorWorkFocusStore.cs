namespace Edf.Application.Projects.InMemory;

using Edf.Application.Operator.WorkContinuity;
using Edf.Domain.Operator;
using Edf.Domain.Projects;

public sealed class InMemoryOperatorWorkFocusStore : IOperatorWorkFocusStore
{
    private readonly Dictionary<ProjectConcordProjectId, OperatorActiveWorkFocus> _byProject = new();

    public OperatorActiveWorkFocus? GetActiveForProject(ProjectConcordProjectId projectId) =>
        _byProject.TryGetValue(projectId, out var record) ? record : null;

    public void Upsert(OperatorActiveWorkFocus record)
    {
        ArgumentNullException.ThrowIfNull(record);
        _byProject[record.ProjectId] = record;
    }

    public bool TryUpdateWithExpectedVersion(OperatorActiveWorkFocus record, long expectedResourceVersion)
    {
        if (!_byProject.TryGetValue(record.ProjectId, out var existing))
        {
            return false;
        }

        if (existing.ResourceVersion != expectedResourceVersion)
        {
            return false;
        }

        _byProject[record.ProjectId] = record;
        return true;
    }
}
