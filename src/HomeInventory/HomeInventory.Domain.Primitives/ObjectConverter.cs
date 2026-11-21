namespace HomeInventory.Domain.Primitives;

public abstract class ObjectConverter<TSource, TDestination>
{
    public TDestination Convert(TSource source) =>
        TryConvert(source)
            .ThrowIfFailed(static error => error.ToException());

    public abstract Validated<TDestination, Error> TryConvert(TSource source);
}
