namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

/// <summary>
/// Provider-internal session context for ACP <c>session/new</c> (working directory only).
/// </summary>
internal sealed class CursorAcpSessionContext
{
    public CursorAcpSessionContext(string workingDirectory)
    {
        if (string.IsNullOrWhiteSpace(workingDirectory))
        {
            throw new ArgumentException("Working directory is required for ACP session/new.", nameof(workingDirectory));
        }

        WorkingDirectory = workingDirectory;
    }

    public string WorkingDirectory { get; }

    /// <summary>
    /// Test/diagnostic helper only — production forward must supply governed Project Root explicitly.
    /// </summary>
    internal static CursorAcpSessionContext FromEnvironmentForDiagnostics()
    {
        var cwd = Environment.CurrentDirectory;
        return new CursorAcpSessionContext(cwd);
    }
}
