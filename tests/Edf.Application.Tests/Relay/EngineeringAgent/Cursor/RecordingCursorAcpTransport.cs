using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

internal sealed class RecordingCursorAcpTransport : ICursorAcpTransport
{
    private readonly ICursorAcpTransport _inner;

    public RecordingCursorAcpTransport(ICursorAcpTransport inner) => _inner = inner;

    public List<string> WrittenLines { get; } = [];

    public ValueTask StartAsync(CancellationToken cancellationToken) => _inner.StartAsync(cancellationToken);

    public async ValueTask WriteLineAsync(string ndjsonLine, CancellationToken cancellationToken)
    {
        WrittenLines.Add(ndjsonLine);
        await _inner.WriteLineAsync(ndjsonLine, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken) =>
        _inner.ReadLineAsync(cancellationToken);

    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
