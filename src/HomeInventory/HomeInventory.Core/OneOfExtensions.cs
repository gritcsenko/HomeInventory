using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Core;

public static class OneOfExtensions
{
    public static async Task<OneOf<TResult, IError>> OnResultAsync<TResult>(this Task<OneOf<TResult, IError>> oneOfTask, Func<Task> action)
    {
        var oneOf = await oneOfTask;
        return await oneOf.OnResultAsync(action);
    }

    public static async Task<OneOf<TResult, IError>> OnResultAsync<TResult>(this OneOf<TResult, IError> oneOf, Func<Task> action) =>
        await oneOf.MapT0Async(async s =>
        {
            await action();
            return s;
        });

    public static async Task<OneOf<TResult, T1>> MapT0Async<T0, T1, TResult>(this OneOf<T0, T1> oneOf, Func<T0, Task<TResult>> mapFunc) =>
        oneOf.Index switch
        {
            0 => await mapFunc(oneOf.AsT0),
            1 => oneOf.AsT1,
            _ => throw new InvalidOperationException(),
        };
}
