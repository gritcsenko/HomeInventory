using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(this PropertyBuilder<TId> builder, IIdFactory<TId, Guid> idFactory)
       where TId : GuidIdentifierObject<TId>
    {
        return builder.HasConversion(new GuidIdValueConverter<TId>(idFactory));
    }
}
