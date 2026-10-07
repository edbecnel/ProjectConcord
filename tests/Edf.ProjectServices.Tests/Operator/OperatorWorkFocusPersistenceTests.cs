using Edf.Domain.Operator;
using Edf.Domain.Projects;
using Edf.Domain.Workflow;
using Edf.ProjectServices.Persistence;

namespace Edf.ProjectServices.Tests.Operator;

public class OperatorWorkFocusPersistenceTests
{
    [Fact]
    public void Migration007_PersistsOperatorWorkFocus_AcrossStoreRestart()
    {
        var path = Path.Combine(Path.GetTempPath(), "edf-focus-" + Guid.NewGuid().ToString("N") + ".db");
        var focusId = Guid.NewGuid();
        ProjectConcordProjectId projectId;
        var instanceId = WorkflowInstanceId.New();

        using (var store = new SqliteUserApplicationStateStore(path))
        {
            var dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "edf-focus-root-" + Guid.NewGuid().ToString("N")));
            var managed = store.RegisterNewProjectAtLocator(
                ProjectLocator.FromPath(dir.FullName),
                "focus-test",
                DateTimeOffset.UtcNow);
            projectId = managed.ProjectId;
            store.UpsertOperatorWorkFocus(
                new OperatorActiveWorkFocus(
                    focusId,
                    projectId,
                    instanceId,
                    "Restart subject",
                    OperatorWorkFocusSubjectProvenance.OperatorConfirmed,
                    null,
                    OperatorWorkFocusResumptionTarget.PlanningRegionWork,
                    "Continuity fact",
                    null,
                    1,
                    DateTimeOffset.UtcNow));
        }

        using var reopened = new SqliteUserApplicationStateStore(path);
        var loaded = reopened.GetOperatorWorkFocus(projectId);
        Assert.NotNull(loaded);
        Assert.Equal(focusId, loaded!.FocusId);
        Assert.Equal("Restart subject", loaded.SubjectLabel);
        Assert.Equal("Continuity fact", loaded.LastContinuitySummary);
        Assert.Equal(OperatorWorkFocusResumptionTarget.PlanningRegionWork, loaded.ResumptionTarget);
    }
}
