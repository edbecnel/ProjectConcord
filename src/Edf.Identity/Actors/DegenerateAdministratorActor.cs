namespace Edf.Identity.Actors;

/// <summary>
/// Local solo-project actor with full Administrator permissions (ADR-0010 degenerate seam).
/// </summary>
public sealed class DegenerateAdministratorActor : ICurrentProjectActor
{
    public DegenerateAdministratorActor(string? displayName = null)
    {
        DisplayName = string.IsNullOrWhiteSpace(displayName)
            ? Environment.UserName
            : displayName;
    }

    public string DisplayName { get; }

    public ProjectRole Role => ProjectRole.Administrator;

    public bool IsAdministrator => true;
}
