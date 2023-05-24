namespace HomeInventory.Tests;

public sealed class ValuesCollection
{
    private readonly List<ValueContainer> _values = new();
    private readonly Type _valueType;

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
        container.Update(Optional.Some<object>(value));
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
        var value = container.Value;
        return value.Convert(x => (T)x);
    }

    private bool IsAsignable<T>() => _valueType.IsAssignableFrom(typeof(T));

    private void AddCore<T>(T value)
        where T : notnull =>
        _values.Add(new ValueContainer(Optional.Some<object>(value), _valueType));
}
