namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

using System.Diagnostics;
using System.Text;

/// <summary>
/// Starts local Cursor CLI <c>agent acp</c> with redirected stdio (provider-internal).
/// </summary>
internal sealed class CursorAcpSubprocessTransport : ICursorAcpTransport
{
    private readonly Func<string> _resolveExecutable;
    private Process? _process;
    private StreamWriter? _stdin;
    private StreamReader? _stdout;

    public CursorAcpSubprocessTransport(Func<string>? resolveExecutable = null)
    {
        _resolveExecutable = resolveExecutable ?? CursorCliLocator.ResolveAgentExecutable;
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        if (_process is not null)
        {
            return;
        }

        var executable = _resolveExecutable();
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = "acp",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardInputEncoding = Encoding.UTF8,
        };

        try
        {
            _process = Process.Start(startInfo)
                ?? throw new InvalidOperationException("Failed to start Cursor agent ACP process.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unable to start Cursor CLI ACP ({executable} acp): {ex.Message}", ex);
        }

        _stdin = _process.StandardInput;
        _stdout = _process.StandardOutput;
        await Task.CompletedTask;
    }

    public async ValueTask WriteLineAsync(string ndjsonLine, CancellationToken cancellationToken)
    {
        if (_stdin is null)
        {
            throw new InvalidOperationException("ACP transport is not started.");
        }

        await _stdin.WriteLineAsync(ndjsonLine.AsMemory(), cancellationToken).ConfigureAwait(false);
        await _stdin.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        if (_stdout is null)
        {
            throw new InvalidOperationException("ACP transport is not started.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        return await _stdout.ReadLineAsync(cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stdin is not null)
        {
            try
            {
                await _stdin.DisposeAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
            }
        }

        if (_process is { HasExited: false })
        {
            try
            {
                _process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
            }
        }

        _process?.Dispose();
        _process = null;
        _stdin = null;
        _stdout = null;
    }
}

internal static class CursorCliLocator
{
    internal static string ResolveAgentExecutable()
    {
        var fromEnv = Environment.GetEnvironmentVariable("PROJECTCONCORD_CURSOR_AGENT_PATH");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv.Trim();
        }

        return "agent";
    }
}
