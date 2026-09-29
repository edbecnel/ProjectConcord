namespace Edf.Application.Relay;

using Edf.Domain.Projects;
using Edf.Domain.Relay;

/// <summary>
/// Application port for shallow, read-only Tier-0 relay context at package assembly time.
/// </summary>
public interface ITier0RelaySnapshotProvider
{
    Tier0RelaySnapshot CaptureSnapshot(ProjectRoot projectRoot);
}
