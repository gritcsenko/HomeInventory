using HomeInventory.Domain.Primitives.Ids;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Visus.Cuid;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(this PropertyBuilder<TId> builder)
        where TId : class, IValuableIdentifierObject<TId, Cuid> =>
        builder.HasIdConversion<TId, Cuid>();

    public static PropertyBuilder<TId> HasIdConversion<TId, TValue>(this PropertyBuilder<TId> builder)
        where TId : class, IValuableIdentifierObject<TId, TValue>
        where TValue : notnull =>
        builder.HasConversion(new IdValueConverter<TId, TValue>(TId.Converter));
}
