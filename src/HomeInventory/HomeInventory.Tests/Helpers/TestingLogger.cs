using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Helpers;

public abstract class TestingLogger<T> : ILogger<T>
{
    private readonly AsyncLocal<ITestingScope?> _currentScope = new();

    public IDisposable BeginScope<TState>(TState state)
#if NET7_0_OR_GREATER
        where TState : notnull
#endif
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

    internal class TestingScope<TState> : ITestingScope
    {
        private readonly TestingLogger<T> _logger;
        private bool _isDisposed;

        public TestingScope(TestingLogger<T> logger, ITestingScope? parent)
        {
            _logger = logger;
            Parent = parent;

            _logger._currentScope.Value = this;
        }

        public ITestingScope? Parent { get; }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _logger._currentScope.Value = Parent;

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
