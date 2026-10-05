namespace Edf.Domain.Workflow;

public readonly record struct WorkflowInstanceStopEventId(Guid Value)
{
    public static WorkflowInstanceStopEventId New() => new(Guid.NewGuid());

    public static WorkflowInstanceStopEventId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Workflow instance stop event id must be a GUID.");
        }

        return new WorkflowInstanceStopEventId(guid);
    }

    public override string ToString() => Value.ToString();
}
