using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives;

public abstract class ObjectConverter<TSource, TDestination>
{
    public TDestination Convert(TSource source) =>
        TryConvert(source)
            .Match(Func.Identity<TDestination>(), error => throw CreateException(error));

    public OneOf<TDestination, IError> TryConvert(TSource source) => TryConvertCore(source);

    protected abstract OneOf<TDestination, IError> TryConvertCore(TSource source);

    private static InvalidOperationException CreateException(IError error)
    {
        var exception = new InvalidOperationException($"Cannot convert '{typeof(TSource).FullName}' to '{typeof(TDestination).FullName}'. Reason: '{error.Message}'");
        foreach (var (key, value) in error.Metadata)
        {
            exception.Data.Add(key, value);
        }

        return exception;
    }
}
