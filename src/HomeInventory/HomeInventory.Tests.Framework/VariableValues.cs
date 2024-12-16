namespace HomeInventory.Tests.Framework;

public sealed class VariableValues<T> : IVariableValues
    where T : notnull
{
    private readonly List<PropertyValue<T>> _values = [];

    public int Count => _values.Count;

    public PropertyValue<T> Add(Func<T> createValueFunc) => AddCore(createValueFunc());

    public async Task<PropertyValue<T>> AddAsync(Func<Task<T>> createValueFunc) => AddCore(await createValueFunc());

#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
    public Option<PropertyValue<T>> TrySet(int index, Func<T> createValueFunc) =>
        index < 0 || index >= _values.Count
            ? Option<PropertyValue<T>>.None
            : _values[index] = createValueFunc();
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions

    public Option<PropertyValue<T>> TryGet(int index) =>
        index < 0 || index >= _values.Count
            ? Option<PropertyValue<T>>.None
            : _values[index];

    public Option<PropertyValue<T>> TryGetOrAdd(int index, Func<T> createValueFunc) =>
        index < 0 || index > _values.Count
            ? Option<PropertyValue<T>>.None
            : GetOrAdd(index, createValueFunc);

    public IEnumerable<T> Values() => _values.Select(static x => x.Value);

    private PropertyValue<T> GetOrAdd(int index, Func<T> createValueFunc) =>
        index == _values.Count
            ? AddCore(createValueFunc())
            : _values[index];

    private PropertyValue<T> AddCore(T value)
    {
        _values.Add(value);
        return value;
    }
}
