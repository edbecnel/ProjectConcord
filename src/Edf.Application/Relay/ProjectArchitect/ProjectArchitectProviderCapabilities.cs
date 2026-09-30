namespace Edf.Application.Relay.ProjectArchitect;

/// <summary>
/// Static P0 capability declaration (PC-PAR-019).
/// </summary>
public sealed record ProjectArchitectProviderCapabilities(
    bool SupportsManualPaste,
    int SupportedRenderVersionMajor,
    bool SupportsGovernanceCriticalProjections,
    bool SupportsSoftwareDevelopmentProfilePayload);
