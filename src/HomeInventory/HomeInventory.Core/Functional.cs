namespace HomeInventory.Core;

public static class Functional
{
    public static Func<T, T> Identity<T>() => Identity<T, T>();

    public static Func<TInput, TOutput> Identity<TInput, TOutput>()
    where TInput : TOutput
        => Identity<TInput, TOutput>;

    internal static TOutput Identity<TInput, TOutput>(TInput input)
        where TInput : TOutput
        => input;
}
