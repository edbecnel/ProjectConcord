namespace Edf.Domain.Workflow;

public readonly record struct WorkflowDefinitionVersion(int Value)
{
    public static WorkflowDefinitionVersion GewV1 => new(1);

    public static WorkflowDefinitionVersion Parse(int value)
    {
        if (value < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Workflow definition version must be positive.");
        }

        return new WorkflowDefinitionVersion(value);
    }
}
