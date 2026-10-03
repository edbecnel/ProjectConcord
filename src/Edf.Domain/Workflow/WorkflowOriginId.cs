namespace Edf.Domain.Workflow;

public readonly record struct WorkflowOriginId(Guid Value)
{
    public static WorkflowOriginId New() => new(Guid.NewGuid());

    public static WorkflowOriginId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Workflow origin id must be a GUID.");
        }

        return new WorkflowOriginId(guid);
    }

    public override string ToString() => Value.ToString();
}
