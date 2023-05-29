using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public sealed class GuidIdValueConverter<TId> : ValueConverter<TId, Guid>
    where TId : class, IGuidIdentifierObject<TId>
{
    private static readonly GuidIdConverter<TId> _converter = new();

    public GuidIdValueConverter()
        : base(
            id => id.Id,
            value => _converter.Convert(value))
    {
    }
}
