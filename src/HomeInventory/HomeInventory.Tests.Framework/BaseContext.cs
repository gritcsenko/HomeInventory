
namespace HomeInventory.Tests.Framework;

public abstract class BaseContext(VariablesContainer variables) : IAsyncDisposable
{
    private readonly VariablesContainer _variables = variables;

    protected internal VariablesContainer Variables => _variables;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return _variables.DisposeAsync();
    }

    protected T GetValue<T>(IVariable<T> variable)
        where T : notnull =>
        GetValue(variable[0]);

    protected T GetValue<T>(IIndexedVariable<T> variable)
        where T : notnull =>
        _variables.Get(variable).Value;
}
