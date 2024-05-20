using Microsoft.Extensions.Logging;
using System.Reactive.Disposables;
using Disposable = System.Reactive.Disposables.Disposable;

namespace HomeInventory.Tests.Framework;

public abstract class TestingLogger<T> : ILogger<T>
{
    public abstract IDisposable BeginScope<TState>(TState state)
        where TState : notnull;

    public bool IsEnabled(LogLevel logLevel) => true;

    void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Log(logLevel, eventId, state!, exception, (object s, Exception? e) => formatter((TState)s, e));

    public abstract void Log(LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter);

    internal sealed class Stub : TestingLogger<T>
    {
        private readonly AsyncLocal<CompositeDisposable?> _currentScope = new();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "False positive")]
        public override IDisposable BeginScope<TState>(TState state)
        {
            var current = _currentScope.Value;
            return _currentScope.Value = [Disposable.Create(() => _currentScope.Value = current)];
        }

        public override void Log(LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter)
        {
        }
    }
}
