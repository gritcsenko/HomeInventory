namespace HomeInventory.Domain.Primitives;

public abstract class ObjectConverter<TSource, TDestination>
{
    public TDestination Convert(TSource source) =>
        TryConvert(source)
            .Match(Functional.Identity<TDestination>(), seq => Error.Many(seq).Throw().Return(ShouldNotBeCalled));

    public abstract Validation<Error, TDestination> TryConvert(TSource source);

    private TDestination ShouldNotBeCalled() => throw new ExceptionalException(nameof(ShouldNotBeCalled), -1_000_000_000);
}
