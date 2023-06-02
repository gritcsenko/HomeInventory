using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests;

public abstract class TestingLogger<T> : ILogger<T>
{
    private readonly AsyncLocal<ITestingScope?> _currentScope = new();

    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull
        =>
        new TestingScope<TState>(this, _currentScope.Value);

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        Log(logLevel, eventId, state!, exception, (object s, Exception? e) => formatter((TState)s, e));

    public abstract void Log(LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter);

    internal interface ITestingScope : IDisposable
    {
        ITestingScope? Parent { get; }
    }

    internal class TestingScope<TState> : Disposable, ITestingScope
    {
        private readonly TestingLogger<T> _logger;

        public TestingScope(TestingLogger<T> logger, ITestingScope? parent)
        {
            _logger = logger;
            Parent = parent;

            _logger._currentScope.Value = this;
        }

        public ITestingScope? Parent { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _logger._currentScope.Value = Parent;
            }
            base.Dispose(disposing);
        }
    }

    internal sealed class Stub : TestingLogger<T>
    {
        public override void Log(LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter)
        {
        }
    }
}
