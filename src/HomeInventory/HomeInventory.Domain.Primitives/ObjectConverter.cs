namespace HomeInventory.Domain.Primitives;

public abstract class ObjectConverter<TSource, TDestination>
{
    public TDestination Convert(TSource source) =>
        TryConvert(source)
            .MatchOrThrow(Functional.Identity<TDestination>());

    public abstract Validation<Error, TDestination> TryConvert(TSource source);
}
