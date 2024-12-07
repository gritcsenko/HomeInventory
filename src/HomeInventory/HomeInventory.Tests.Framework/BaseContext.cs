
namespace HomeInventory.Tests.Framework;

public abstract class BaseContext(VariablesContainer variables) : IAsyncDisposable
{
    protected internal VariablesContainer Variables { get; } = variables;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return Variables.DisposeAsync();
    }

    protected T GetValue<T>(IVariable<T> variable)
        where T : notnull =>
        GetValue(variable[0]);

    protected T GetValue<T>(IIndexedVariable<T> variable)
        where T : notnull =>
        Variables.Get(variable).Value;
}
