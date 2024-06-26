using System.Collections;

namespace HomeInventory.Tests.Framework;

public sealed class ValuesCollection(Type valueType) : IReadOnlyCollection<ValueContainer>, IAsyncDisposable
{
    private readonly List<ValueContainer> _values = [];
    private readonly Type _valueType = valueType;

    public int Count => _values.Count;

    public Optional<T> TryAdd<T>(Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return Optional.None<T>();
        }

        return AddCore(createValueFunc());
    }

    public async Task<Optional<T>> TryAddAsync<T>(Func<Task<T>> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>())
        {
            return Optional.None<T>();
        }

        return AddCore(await createValueFunc());
    }

    public Optional<T> TrySet<T>(int index, Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index >= _values.Count)
        {
            return Optional.None<T>();
        }

        var container = _values[index];
        var value = createValueFunc();
        container.Update(value);
        return value;
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

    public Optional<T> TryGetOrAdd<T>(int index, Func<T> createValueFunc)
        where T : notnull
    {
        if (!IsAsignable<T>() || index < 0 || index > _values.Count)
        {
            return Optional.None<T>();
        }

        if (index == _values.Count)
        {
            return AddCore(createValueFunc());
        }

        var container = _values[index];
        return (T)container.Value;
    }

    public IEnumerable<T> GetAll<T>()
        where T : notnull =>
        _values.Select(x => x.Value).Cast<T>();

    public bool IsAsignable<T>() =>
        typeof(T).IsAssignableTo(_valueType);

    private T AddCore<T>(T value)
        where T : notnull
    {
        _values.Add(new ValueContainer(value, _valueType));
        return value;
    }

    public IEnumerator<ValueContainer> GetEnumerator() =>
        _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        _values.GetEnumerator();

    public async ValueTask DisposeAsync()
    {
        var values = _values.ToArray();
        _values.Clear();
        foreach (var value in values)
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
    }
}
