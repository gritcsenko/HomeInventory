using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace HomeInventory.Core;

public static class ExceptionExtensions
{
    /// <summary>Throw the error as an exception</summary>
    [DoesNotReturn]
    public static void Rethrow(this Exception e) =>
        ExceptionDispatchInfo.Capture(e).Throw();

    /// <summary>Throw the error as an exception</summary>
    [DoesNotReturn]
    public static T Rethrow<T>(this Exception e)
    {
        e.Rethrow();
        return default;
    }
}
