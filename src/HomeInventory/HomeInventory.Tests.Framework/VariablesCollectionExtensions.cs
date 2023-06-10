﻿namespace HomeInventory.Tests.Framework;

public static class VariablesCollectionExtensions
{
    public static IEnumerable<T> Get<T>(this VariablesContainer collection, IVariable<T> variable, int count)
        where T : notnull =>
        Enumerable.Range(0, count)
            .Select(index => collection.Get(variable.WithIndex(index)));

    public static T Get<T>(this VariablesContainer collection, IIndexedVariable<T> variable)
        where T : notnull
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        return collection
            .TryGet(variable)
            .OrInvoke(() => throw new InvalidOperationException($"Failed to get {variable.Name} of type {typeof(T)} at index {variable.Index}"));
    }
}