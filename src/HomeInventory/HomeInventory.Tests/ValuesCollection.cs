using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

public sealed class ValuesCollection
{
    private readonly List<ValueContainer> _values = new();
    private readonly Type _valueType;

    public ValuesCollection(Type valueType)
    {
        _valueType = valueType;
    }

    public bool TryAdd<T>(T value)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return false;
        }

        var container = new ValueContainer(Option.Some<object>(value), _valueType);
        _values.Add(container);
        return true;
    }

    public bool TryAdd<T>(Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return false;
        }

        var value = createValueFunc();
        var container = new ValueContainer(Option.Some<object>(value), _valueType);
        _values.Add(container);
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
        container.Update(Option.Some<object>(value));
        return true;
    }

    public Option<T> TryGet<T>(int index)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index >= _values.Count)
        {
            return Option.None<T>();
        }

        var container = _values[index];
        var value = container.Value;
        return value.Select(x => (T)x);
    }

    private bool IsAsignable<T>() => _valueType.IsAssignableFrom(typeof(T));
}
