using HomeInventory.Domain.Errors;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeInventory.Domain.Primitives;

public sealed class GuidIdValueConverter<TId> : ValueConverter<TId, Guid>
    where TId : GuidIdentifierObject<TId>
{
    public GuidIdValueConverter(Func<Guid, TId> createIdFunc)
        : this(GuidIdFactory.Create(createIdFunc))
    {
    }
    public GuidIdValueConverter(IIdFactory<TId, Guid> idFactory)
        : base(id => Convert(id), value => Convert(value, idFactory))
    {
    }

    private static Guid Convert(TId id) => id.Id;

    private static TId Convert(Guid value, IIdFactory<TId, Guid> idFactory)
    {
        return idFactory.CreateFrom(value).Match(
            id => id,
            error => throw CreateException(error));
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
