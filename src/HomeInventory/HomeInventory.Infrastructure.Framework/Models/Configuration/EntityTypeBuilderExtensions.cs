using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(this PropertyBuilder<TId> builder)
        where TId : class, IUlidIdentifierObject<TId> =>
        builder.HasConversion(new UlidIdValueConverter<TId>());
}
