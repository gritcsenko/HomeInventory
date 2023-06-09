using Microsoft.Extensions.Logging;

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

        public override IDisposable BeginScope<TState>(TState state)
        {
            var current = _currentScope.Value;

            var disposable = new CompositeDisposable();
            disposable.AddDisposable(() => _currentScope.Value = current);

            return _currentScope.Value = disposable;
        }

        public override void Log(LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter)
        {
        }
    }
}
