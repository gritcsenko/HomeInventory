using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class UlidIdValueConverter<TId> : ValueConverter<TId, Ulid>
    where TId : class, IUlidIdentifierObject<TId>
{
    private static readonly UlidIdConverter<TId> _converter = new();

    public UlidIdValueConverter()
        : base(
            id => id.Value,
            value => _converter.Convert(value))
    {
    }
}
