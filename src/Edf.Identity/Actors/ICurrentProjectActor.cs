namespace Edf.Identity.Actors;

public interface ICurrentProjectActor
{
    string DisplayName { get; }

    ProjectRole Role { get; }

    bool IsAdministrator { get; }
}
