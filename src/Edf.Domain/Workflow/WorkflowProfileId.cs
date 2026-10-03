namespace Edf.Domain.Workflow;

public readonly record struct WorkflowProfileId(string Value)
{
    public static WorkflowProfileId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException("Workflow profile id must be non-empty.");
        }

        return new WorkflowProfileId(value);
    }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}
