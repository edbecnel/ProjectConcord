namespace Edf.Domain.Workflow;

public readonly record struct WorkflowInstanceId(Guid Value)
{
    public static WorkflowInstanceId New() => new(Guid.NewGuid());

    public static WorkflowInstanceId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Workflow instance id must be a GUID.");
        }

        return new WorkflowInstanceId(guid);
    }

    public bool IsEmpty => Value == Guid.Empty;

    public override string ToString() => Value.ToString();
}
