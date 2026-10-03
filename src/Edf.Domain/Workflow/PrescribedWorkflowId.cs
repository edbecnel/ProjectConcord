namespace Edf.Domain.Workflow;

public readonly record struct PrescribedWorkflowId(string Value)
{
    public static PrescribedWorkflowId GovernedEngineering => new(PrescribedWorkflowIds.GovernedEngineering);

    public static PrescribedWorkflowId Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException("Prescribed workflow id must be non-empty.");
        }

        return new PrescribedWorkflowId(value);
    }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;
}
