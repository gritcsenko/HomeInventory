using System.Collections;

namespace HomeInventory.Tests.Framework;

public sealed class ValuesCollection : IReadOnlyCollection<ValueContainer>
{
    private readonly List<ValueContainer> _values = [];
    private readonly Type _valueType;

    public int Count => ((IReadOnlyCollection<ValueContainer>)_values).Count;

    public ValuesCollection(Type valueType) => _valueType = valueType;

    public bool TryAdd<T>(Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return false;
        }

        AddCore(createValueFunc());
        return true;
    }

    public async Task<bool> TryAddAsync<T>(Func<Task<T>> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return false;
        }

        AddCore(await createValueFunc());
        return true;
    }

    public bool TrySet<T>(int index, Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index >= _values.Count)
        {
            return false;
        }

        var container = _values[index];
        var value = createValueFunc();
        container.Update(value);
        return true;
    }

    public Optional<T> TryGet<T>(int index)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index >= _values.Count)
        {
            return Optional.None<T>();
        }

        var container = _values[index];
        return (T)container.Value;
    }

    public IEnumerable<T> GetAll<T>()
        where T : notnull
    {
        return _values.Select(x => x.Value).Cast<T>();
    }

    public bool IsAsignable<T>() =>
        typeof(T).IsAssignableTo(_valueType);

    private void AddCore<T>(T value)
        where T : notnull =>
        _values.Add(new ValueContainer(value, _valueType));

    public IEnumerator<ValueContainer> GetEnumerator() =>
        ((IEnumerable<ValueContainer>)_values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)_values).GetEnumerator();
}
