using LanguageExt.SomeHelp;
using System.Collections;

namespace HomeInventory.Tests.Framework;

public sealed class ValuesCollection(Type valueType) : IReadOnlyCollection<ValueContainer>, IAsyncDisposable
{
    private readonly List<ValueContainer> _values = [];
    private readonly Type _valueType = valueType;

    public int Count => ((IReadOnlyCollection<ValueContainer>)_values).Count;

    public Option<PropertyValue<T>> TryAdd<T>(Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return OptionNone.Default;
        }

        return AddCore(createValueFunc()).ToSome();
    }

    public async Task<Option<PropertyValue<T>>> TryAddAsync<T>(Func<Task<T>> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return OptionNone.Default;
        }

        return AddCore(await createValueFunc());
    }

    public Option<PropertyValue<T>> TrySet<T>(int index, Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index >= _values.Count)
        {
            return OptionNone.Default;
        }

        var container = _values[index];
        var value = createValueFunc();
        container.Update(value);
        return (PropertyValue<T>)value;
    }

    public Option<PropertyValue<T>> TryGet<T>(int index)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index >= _values.Count)
        {
            return OptionNone.Default;
        }

        var container = _values[index];
        return (PropertyValue<T>)(T)container.Value;
    }

    public Option<PropertyValue<T>> TryGetOrAdd<T>(int index, Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index > _values.Count)
        {
            return OptionNone.Default;
        }

        if (index == _values.Count)
        {
            return AddCore(createValueFunc());
        }

        var container = _values[index];
        return (PropertyValue<T>)(T)container.Value;
    }

    public IEnumerable<T> GetAll<T>()
        where T : notnull
    {
        return _values.Select(x => x.Value).Cast<T>();
    }

    public bool IsAsignable<T>() =>
        typeof(T).IsAssignableTo(_valueType);

    private PropertyValue<T> AddCore<T>(T value)
        where T : notnull
    {
        _values.Add(new ValueContainer(value, _valueType));
        return value;
    }

    public IEnumerator<ValueContainer> GetEnumerator() =>
        ((IEnumerable<ValueContainer>)_values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable)_values).GetEnumerator();

    public async ValueTask DisposeAsync()
    {
        foreach (var value in _values)
        {
            switch (value)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
        _values.Clear();
    }
}

public record struct PropertyValue<T>(T Value)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Not needed")]
    public static implicit operator PropertyValue<T>(T value) => new(value);
}
