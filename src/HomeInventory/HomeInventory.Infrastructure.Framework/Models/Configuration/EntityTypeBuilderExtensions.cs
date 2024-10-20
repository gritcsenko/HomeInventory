using HomeInventory.Domain.Primitives.Ids;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Framework.Models.Configuration;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(this PropertyBuilder<TId> builder)
        where TId : class, IValuableIdentifierObject<TId, Ulid> =>
        builder.HasIdConversion<TId, Ulid>();

    public static PropertyBuilder<TId> HasIdConversion<TId, TValue>(this PropertyBuilder<TId> builder)
        where TId : class, IValuableIdentifierObject<TId, TValue>
        where TValue : notnull =>
        builder.HasConversion(new IdValueConverter<TId, TValue>(TId.Converter));
}
