using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests.Framework;

public sealed class VariablesCollection
{
    private readonly Dictionary<string, ValuesCollection> _variables = new();

    public bool TryAdd<T>(IVariable<T> variable, Func<T> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryAdd(createValueFunc);
    }

    public async Task<bool> TryAddAsync<T>(IVariable<T> variable, Func<Task<T>> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return await collection.TryAddAsync(createValueFunc);
    }

    public Optional<T> TryGet<T>(IIndexedVariable<T> variable)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryGet<T>(variable.Index);
    }

    public bool TryUpdate<T>(IIndexedVariable<T> variable, Func<T> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TrySet(variable.Index, createValueFunc);
    }

    private static ValuesCollection CreateValues<T>(string key) => new(typeof(T));

    private ValuesCollection GetAllValues<T>(IVariable<T> variable) =>
        _variables.GetOrAdd(variable.Name, CreateValues<T>);
}
