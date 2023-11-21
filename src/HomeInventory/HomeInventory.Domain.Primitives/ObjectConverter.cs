using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Mapping;

public abstract class ObjectConverter<TObject, TValue>
{
    public TObject Convert(TValue source) =>
        TryConvert(source)
            .Match(Func.Identity<TObject>(), error => throw CreateException(error));

    public OneOf<TObject, IError> TryConvert(TValue source) => TryConvertCore(source);

    protected abstract OneOf<TObject, IError> TryConvertCore(TValue source);

    private static InvalidOperationException CreateException(IError error)
    {
        var exception = new InvalidOperationException($"Cannot convert '{typeof(TValue).FullName}' to '{typeof(TObject).FullName}'. Reason: '{error.Message}'");
        foreach (var (key, value) in error.Metadata)
        {
            exception.Data.Add(key, value);
        }
        return exception;
    }
}
