namespace Edf.Application.Relay.Serialization;

/// <summary>
/// A2 serialization v1 markers (plan §8).
/// </summary>
public static class GovernedRelayV1Format
{
    public const string RenderVersionLinePrefix = "ProjectConcord-Relay-Render:";
    public const string MachineBlockFenceLanguage = "projectconcord-relay-v1";
    public const string GovernanceCriticalHeading = "## Governance-Critical";
    public const string StopHeading = "## STOP";
    public const string AuthorizationDispositionHeading = "## Authorization-Disposition";
    public const string WorkContextHeading = "## Work-Context";
    public const string EdfCorrelationHeading = "## EDF-Correlation";
    public const string PaCursorReminder =
        "Reminder: the next Cursor handover must include Cursor-Mode (and Cursor-Mode-Transition when mode changes).";
}
