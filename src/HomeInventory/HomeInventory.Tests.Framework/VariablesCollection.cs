﻿namespace HomeInventory.Tests.Framework;

public sealed class VariablesContainer : IAsyncDisposable
{
    private readonly Dictionary<string, IVariableValues> _variables = [];

    public PropertyValue<T> Add<T>(IVariable<T> variable, Func<T> createValueFunc)
        where T : notnull =>
        AllFor(variable).Add(createValueFunc);

    public async Task<PropertyValue<T>> AddAsync<T>(IVariable<T> variable, Func<Task<T>> createValueFunc)
        where T : notnull =>
        await AllFor(variable).AddAsync(createValueFunc);

    public Option<PropertyValue<T>> TryGet<T>(IIndexedVariable<T> variable)
        where T : notnull =>
        AllFor(variable).TryGet(variable.Index);

    public Option<PropertyValue<T>> TryGetOrAdd<T>(IIndexedVariable<T> variable, Func<T> createValueFunc)
        where T : notnull =>
        AllFor(variable).TryGetOrAdd(variable.Index, createValueFunc);

    public IEnumerable<T> GetAll<T>(IVariable<T> variable)
        where T : notnull =>
        AllFor(variable).Values();

    public Option<PropertyValue<T>> TryUpdate<T>(IIndexedVariable<T> variable, Func<T> createValueFunc)
        where T : notnull =>
        AllFor(variable).TrySet(variable.Index, createValueFunc);

    private VariableValues<T> AllFor<T>(IVariable<T> variable)
        where T : notnull =>
        _variables.GetOrAdd(variable.Name, static _ => new VariableValues<T>());

    public async ValueTask DisposeAsync()
    {
        foreach (var collection in _variables.Values)
        {
            await collection.DisposeAsync();
        }

        _variables.Clear();
    }
}
