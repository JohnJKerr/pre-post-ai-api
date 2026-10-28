using System.Collections.Concurrent;
using Api.Generating;
using Microsoft.Extensions.Logging;

namespace Api.Tests;

internal class GivenAGenerationLog : ILogger<ReportGenerator>
{
    public ConcurrentQueue<IReadOnlyList<KeyValuePair<string, object?>>> Entries { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (state is IReadOnlyList<KeyValuePair<string, object?>> entry) Entries.Enqueue(entry);
    }
}
