namespace HomeInventory.Tests.Framework;

public sealed class VariablesContainer : IAsyncDisposable
{
    private readonly Dictionary<string, ValuesCollection> _variables = [];

    public Option<T> TryAdd<T>(IVariable<T> variable, Func<T> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryAdd(createValueFunc);
    }

    public async Task<Option<T>> TryAddAsync<T>(IVariable<T> variable, Func<Task<T>> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return await collection.TryAddAsync(createValueFunc);
    }

    public Option<T> TryGet<T>(IIndexedVariable<T> variable)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryGet<T>(variable.Index);
    }

    public Option<T> TryGetOrAdd<T>(IIndexedVariable<T> variable, Func<T> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryGetOrAdd<T>(variable.Index, createValueFunc);
    }

    public IEnumerable<T> GetAll<T>(IVariable<T> variable)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.IsAsignable<T>() ? collection.GetAll<T>() : ([]);
    }

    public Option<T> TryUpdate<T>(IIndexedVariable<T> variable, Func<T> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TrySet(variable.Index, createValueFunc);
    }

    private static ValuesCollection CreateValues<T>(string key) => new(typeof(T));

    private ValuesCollection GetAllValues<T>(IVariable<T> variable) =>
        _variables.GetOrAdd(variable.Name, CreateValues<T>);

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        foreach (var collection in _variables.Values)
        {
            await collection.DisposeAsync();
        }
        _variables.Clear();
    }
}
