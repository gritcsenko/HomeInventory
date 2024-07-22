using System.Reactive.Disposables;

namespace HomeInventory.Tests.Framework;

public abstract class BaseContext(VariablesContainer variables) : IAsyncDisposable
{
    private readonly VariablesContainer _variables = variables;
    private readonly CompositeDisposable _compositeDisposable = [];

    protected internal VariablesContainer Variables => _variables;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _compositeDisposable.Dispose();
        return _variables.DisposeAsync();
    }

    protected T GetValue<T>(IVariable<T> variable)
        where T : notnull =>
        GetValue(variable[0]);

    protected T GetValue<T>(IIndexedVariable<T> variable)
        where T : notnull =>
        _variables.Get(variable);

    protected void AddDisposable(IDisposable disposable) => _compositeDisposable.Add(disposable);
}
