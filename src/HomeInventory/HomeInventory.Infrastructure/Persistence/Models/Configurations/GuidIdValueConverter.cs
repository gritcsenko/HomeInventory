using System.Runtime.Versioning;
using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OneOf;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public sealed class GuidIdValueConverter<TId> : ValueConverter<TId, Guid>
    where TId : class, IGuidIdentifierObject<TId>
{
    public GuidIdValueConverter()
        : base(
            id => Convert(id),
#pragma warning disable CA2252 // This API requires opting into preview features
            value => Convert(value))
#pragma warning restore CA2252 // This API requires opting into preview features
    {
    }

    private static Guid Convert(TId id) => id.Id;

    [RequiresPreviewFeatures]
    private static TId Convert(Guid value) =>
        InternalConvert(value)
            .Match(
                id => id,
                error => throw CreateException(error));

    [RequiresPreviewFeatures]
    private static OneOf<TId, IError> InternalConvert(Guid source)
    {
        if (!GuidIdentifierObjectBuilder<TId>.IsValid(source))
        {
            return new ObjectValidationError<Guid>(source);
        }

        var builder = TId.CreateBuilder();
        builder.WithValue(new ValueSupplier<Guid>(source));
        return builder.Invoke();
    }

    private static Exception CreateException(IError error)
    {
        var exception = new InvalidOperationException(error.Message);
        foreach (var (key, value) in error.Metadata)
        {
            exception.Data[key] = value;
        }
        return exception;
    }
}
