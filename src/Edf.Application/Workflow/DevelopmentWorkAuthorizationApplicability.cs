using Edf.Domain.Workflow;

namespace Edf.Application.Workflow;

public static class DevelopmentWorkAuthorizationApplicability
{
    public static bool IsRecordActive(DevelopmentWorkAuthorization record) =>
        record.Disposition == DevelopmentWorkAuthorizationDisposition.Active;

    public static bool IsCurrentlyApplicable(
        DevelopmentWorkAuthorization record,
        WorkflowInstance instance,
        IPrescribedWorkflowRegistry registry)
    {
        if (!IsRecordActive(record))
        {
            return false;
        }

        if (record.WorkflowInstanceId != instance.InstanceId)
        {
            return false;
        }

        if (instance.Lifecycle != WorkflowInstanceLifecycle.Active)
        {
            return false;
        }

        if (!registry.TryGetDefinition(instance.WorkflowId, instance.DefinitionVersion, out var definition)
            || definition is null)
        {
            return false;
        }

        return true;
    }
}
