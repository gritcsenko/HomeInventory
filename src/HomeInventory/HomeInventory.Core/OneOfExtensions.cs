using HomeInventory.Domain.Primitives.Errors;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Core;

public static class OneOfExtensions
{
    public static async Task<OneOf<Success, IError>> OnSuccessAsync(this Task<OneOf<Success, IError>> oneOfTask, Func<Task> action)
    {
        var oneOf = await oneOfTask;
        return await oneOf.OnSuccessAsync(action);
    }

    public static async Task<OneOf<Success, IError>> OnSuccessAsync(this OneOf<Success, IError> oneOf, Func<Task> action) =>
        await oneOf.MapT0Async(async s =>
        {
            await action();
            return s;
        });

    public static async Task<OneOf<TResult, IError>> MapSuccessAsync<TResult>(this OneOf<Success, IError> oneOf, Func<Task<TResult>> mapFunc) =>
        await oneOf.MapT0Async(async _ => await mapFunc());

    public static async Task<OneOf<TResult, T1>> MapT0Async<T0, T1, TResult>(this OneOf<T0, T1> oneOf, Func<T0, Task<TResult>> mapFunc) =>
        oneOf.Index switch
        {
            0 => await mapFunc(oneOf.AsT0),
            1 => oneOf.AsT1,
            _ => throw new InvalidOperationException(),
        };
}
