namespace HomeInventory.Core;

public static class Execute
{
    public static async Task AndCatchAsync<TException>(Func<Task> asyncAction, Action<TException> exceptionHandler)
        where TException : Exception
    {
        try
        {
            await asyncAction();
        }
        catch (TException ex)
        {
            exceptionHandler(ex);
        }
    }

    public static async Task<TResult> AndCatchAsync<TResult, TException>(Func<Task<TResult>> asyncAction, Func<TException, TResult> exceptionHandler)
        where TException : Exception
    {
        try
        {
            return await asyncAction();
        }
        catch (TException ex)
        {
            return exceptionHandler(ex);
        }
    }

    public static TResult AndCatch<TResult, TException>(Func<TResult> func, Func<TException, TResult> exceptionHandler)
        where TException : Exception
    {
        try
        {
            return func();
        }
        catch (TException ex)
        {
            return exceptionHandler(ex);
        }
    }
}
