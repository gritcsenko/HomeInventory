using OneOf;

namespace HomeInventory.Core;

public static class FunctionalExtensions
{
    public static TResult Pipe<TValue, TResult>(this TValue value, Func<TValue, TResult> resultFunc) =>
        resultFunc(value);

    public static TResult Pipe<TValue, TIntermediate, TResult>(this TValue value, Func<TValue, TIntermediate> intermediateFunc, Func<TIntermediate, TResult> resultFunc) =>
       value.Pipe(intermediateFunc.Flow(resultFunc));

    public static TResult Pipe<TValue, TIntermediate1, TIntermediate2, TResult>(this TValue value, Func<TValue, TIntermediate1> intermediate1Func, Func<TIntermediate1, TIntermediate2> intermediate2Func, Func<TIntermediate2, TResult> resultFunc) =>
       value.Pipe(intermediate1Func.Flow(intermediate2Func, resultFunc));

    public static Func<TValue, TResult> Flow<TValue, TIntermediate, TResult>(this Func<TValue, TIntermediate> intermediateFunc, Func<TIntermediate, TResult> resultFunc) =>
        value => resultFunc(intermediateFunc(value));

    public static Func<TValue, TResult> Flow<TValue, TIntermediate1, TIntermediate2, TResult>(this Func<TValue, TIntermediate1> intermediate1Func, Func<TIntermediate1, TIntermediate2> intermediate2Func, Func<TIntermediate2, TResult> resultFunc) =>
        intermediate1Func.Flow(intermediate2Func).Flow(resultFunc);

    public static Func<TValue1, TValue2, TResult> Flow<TValue1, TValue2, TIntermediate, TResult>(this Func<TValue1, TValue2, TIntermediate> intermediateFunc, Func<TIntermediate, TResult> resultFunc) =>
        (value1, value2) => resultFunc(intermediateFunc(value1, value2));

    public static Func<TValue1, TValue2, TResult> Flow<TValue1, TValue2, TIntermediate1, TIntermediate2, TResult>(this Func<TValue1, TValue2, TIntermediate1> intermediate1Func, Func<TIntermediate1, TIntermediate2> intermediate2Func, Func<TIntermediate2, TResult> resultFunc) =>
        intermediate1Func.Flow(intermediate2Func).Flow(resultFunc);

    public static Action<TValue> Terminate<TValue, TResult>(this Func<TValue, TResult> func) =>
        value => func(value);

    public static TResult Match<TValue, TResult>(this Optional<TValue> optional, Func<TValue, TResult> someFunc, Func<TResult> noneFunc) =>
        optional.Convert(someFunc.AsConverter()).OrInvoke(noneFunc);



    public static OneOf<TSomeResult, TNoneResult> Match<TValue, TSomeResult, TNoneResult>(this Optional<TValue> optional, Func<TValue, TSomeResult> someFunc, Func<TNoneResult> noneFunc)
    {
        if (optional.TryGet(out var value))
        {
            return someFunc(value);
        }
        else
        {
            return noneFunc();
        }
    }

    public static void Switch<TValue>(this Optional<TValue> optional, Action<TValue> someAction, Action noneAction)
    {
        if (optional.TryGet(out var value))
        {
            someAction(value);
        }
        else
        {
            noneAction();
        }
    }

    public static Optional<TValue> Flatten<TValue>(this Optional<Optional<TValue>> optional) =>
        optional.Convert(Converter.Identity<Optional<TValue>>());

    public static Optional<TResult> Chain<TValue, TResult>(this Optional<TValue> optional, Func<TValue, Optional<TResult>> convertFunc) =>
        optional.Convert(convertFunc.AsConverter());
}
