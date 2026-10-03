namespace Edf.Domain.Workflow;

public readonly record struct WorkflowDependencyId(Guid Value)
{
    public static WorkflowDependencyId New() => new(Guid.NewGuid());

    public static WorkflowDependencyId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("Workflow dependency id must be a GUID.");
        }

        return new WorkflowDependencyId(guid);
    }

    public override string ToString() => Value.ToString();
}
