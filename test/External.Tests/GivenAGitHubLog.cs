using Microsoft.Extensions.Logging;

namespace External.Tests;

internal class GivenAGitHubLog<T> : ILogger<T>
{
    public List<IReadOnlyList<KeyValuePair<string, object?>>> Entries { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (state is IReadOnlyList<KeyValuePair<string, object?>> entry) Entries.Add(entry);
    }
}
