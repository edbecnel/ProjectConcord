using Edf.Application.Relay.EngineeringAgent.Providers.Cursor;

namespace Edf.Application.Tests.Relay.EngineeringAgent.Cursor;

internal sealed class ScriptedCursorAcpTransport : ICursorAcpTransport
{
    private readonly Queue<string> _responses;

    public ScriptedCursorAcpTransport(IEnumerable<string> scriptedResponses)
    {
        _responses = new Queue<string>(scriptedResponses);
    }

    public List<string> WrittenLines { get; } = [];

    public ValueTask StartAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask WriteLineAsync(string ndjsonLine, CancellationToken cancellationToken)
    {
        WrittenLines.Add(ndjsonLine);
        return ValueTask.CompletedTask;
    }

    public ValueTask<string?> ReadLineAsync(CancellationToken cancellationToken)
    {
        if (_responses.Count == 0)
        {
            return ValueTask.FromResult<string?>(null);
        }

        return ValueTask.FromResult<string?>(_responses.Dequeue());
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
