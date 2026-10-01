namespace Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

/// <summary>
/// Provider-internal stdio transport for Cursor ACP (one NDJSON line per read/write).
/// </summary>
internal interface ICursorAcpTransport : IAsyncDisposable
{
    ValueTask StartAsync(CancellationToken cancellationToken);

    ValueTask WriteLineAsync(string ndjsonLine, CancellationToken cancellationToken);

    ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken);
}
