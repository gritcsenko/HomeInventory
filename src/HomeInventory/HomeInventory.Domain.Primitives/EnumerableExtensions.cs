﻿namespace HomeInventory.Domain.Primitives;

public static class EnumerableExtensions
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? source) => source ?? Array.Empty<T>();

    public static IReadOnlyCollection<T> ToReadOnly<T>(this IEnumerable<T> source) => source as IReadOnlyCollection<T> ?? source.ToArray();
}
