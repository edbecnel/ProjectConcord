namespace Edf.Application.Operator.WorkContinuity;

using Edf.Domain.Operator;
using Edf.Domain.Projects;

public interface IOperatorWorkFocusStore
{
    OperatorActiveWorkFocus? GetActiveForProject(ProjectConcordProjectId projectId);

    void Upsert(OperatorActiveWorkFocus record);

    bool TryUpdateWithExpectedVersion(OperatorActiveWorkFocus record, long expectedResourceVersion);
}
