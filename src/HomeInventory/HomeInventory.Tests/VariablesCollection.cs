using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

public sealed class VariablesCollection
{
    private readonly Dictionary<string, ValuesCollection> _variables = new();

    public bool Add<T>(IVariable<T> variable, Func<T> createValueFunc)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryAdd(createValueFunc);
    }

    public Option<T> TryGet<T>(IIndexedVariable<T> variable)
        where T : notnull
    {
        var collection = GetAllValues(variable);
        return collection.TryGet<T>(variable.Index);
    }

    private static ValuesCollection CreateValues<T>(string key) => new(typeof(T));

    private ValuesCollection GetAllValues<T>(IVariable<T> variable) =>
        _variables.GetOrAdd(variable.Name, CreateValues<T>);
}
