namespace Edf.Application.Relay;

using Edf.Domain.Projects;
using Edf.Domain.Relay;
using InfrastructureTier0Provider = Edf.ProjectServices.Relay.Tier0RelaySnapshotProvider;

/// <summary>
/// Application-facing Tier-0 provider delegating to ProjectServices infrastructure.
/// </summary>
public sealed class Tier0RelaySnapshotProvider : ITier0RelaySnapshotProvider
{
    private readonly InfrastructureTier0Provider _capture;

    public Tier0RelaySnapshotProvider()
        : this(new InfrastructureTier0Provider())
    {
    }

    public Tier0RelaySnapshotProvider(InfrastructureTier0Provider capture)
    {
        _capture = capture ?? throw new ArgumentNullException(nameof(capture));
    }

    public Tier0RelaySnapshot CaptureSnapshot(ProjectRoot projectRoot) =>
        _capture.Capture(projectRoot);
}
