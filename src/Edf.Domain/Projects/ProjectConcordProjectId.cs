namespace Edf.Domain.Projects;

/// <summary>
/// Stable logical ProjectConcord project identity (independent of filesystem path).
/// </summary>
public readonly record struct ProjectConcordProjectId(Guid Value)
{
    public static ProjectConcordProjectId New() => new(Guid.NewGuid());

    public static ProjectConcordProjectId Parse(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new FormatException("ProjectConcord Project ID must be a GUID.");
        }

        return new ProjectConcordProjectId(guid);
    }

    public override string ToString() => Value.ToString();
}
