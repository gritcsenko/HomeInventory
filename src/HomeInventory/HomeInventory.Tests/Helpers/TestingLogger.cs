using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Helpers;

public abstract class TestingLogger<T> : ILogger<T>
{
    private readonly AsyncLocal<ITestingScope?> _currentScope = new();

    public IDisposable BeginScope<TState>(TState state)
    {
        var parent = _currentScope.Value;
        var scope = new TestingScope<TState>(state, this, parent);
        _currentScope.Value = scope;
        return scope;
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => Log(logLevel, eventId, state!, exception, (object s, Exception? e) => formatter((TState)s, e));

    public abstract void Log(LogLevel logLevel, EventId eventId, object state, Exception? exception, Func<object, Exception?, string> formatter);

    private void EndScope(ITestingScope current) => _currentScope.Value = current.Parent;

    private IEnumerable<ITestingScope> GetScopes()
    {
        var scope = _currentScope.Value;
        while (scope != null)
        {
            yield return scope;
            scope = scope.Parent;
        }
    }

    internal interface ITestingScope : IDisposable
    {
        ITestingScope? Parent { get; }
    }

    internal class TestingScope<TState> : ITestingScope
    {
        private readonly TState _state;
        private readonly TestingLogger<T> _logger;
        private bool _isDisposed;

        public TestingScope(TState state, TestingLogger<T> logger, ITestingScope? parent)
        {
            _state = state;
            _logger = logger;
            Parent = parent;
        }

        public ITestingScope? Parent { get; }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _logger.EndScope(this);

            GC.SuppressFinalize(this);
            _isDisposed = true;
        }
    }
}
