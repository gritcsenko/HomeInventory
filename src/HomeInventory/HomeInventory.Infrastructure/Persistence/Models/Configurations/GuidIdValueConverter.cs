using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OneOf;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public sealed class GuidIdValueConverter<TId> : ValueConverter<TId, Guid>
    where TId : notnull, GuidIdentifierObject<TId>, IBuildable<TId, GuidIdentifierObject<TId>.Builder>
{
    public GuidIdValueConverter()
        : base(id => Convert(id), value => Convert(value))
    {
    }

    private static Guid Convert(TId id) => id.Id;

    private static TId Convert(Guid value) =>
        InternalConvert(value)
            .Match(
                id => id,
                error => throw CreateException(error));

    private static Exception CreateException(IError error)
    {
        var exception = new InvalidOperationException(error.Message);
        foreach (var (key, value) in error.Metadata)
        {
            exception.Data[key] = value;
        }
        return exception;
    }

    private static OneOf<TId, IError> InternalConvert(Guid source)
    {
        if (source == Guid.Empty)
        {
            return new ObjectValidationError<Guid>(source);
        }

#pragma warning disable CA2252 // This API requires opting into preview features
        var builder = TId.CreateBuilder();
#pragma warning restore CA2252 // This API requires opting into preview features
        builder.WithValue(new ValueSupplier<Guid>(source));
        return builder.Invoke();
    }
}
