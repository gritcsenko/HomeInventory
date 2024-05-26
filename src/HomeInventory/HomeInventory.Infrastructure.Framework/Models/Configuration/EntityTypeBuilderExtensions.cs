using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives.Ids;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Visus.Cuid;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(this PropertyBuilder<TId> builder)
        where TId : IdentifierObject<TId, Cuid>, ICuidBuildable<TId>, ICuidIdentifierObject<TId> =>
        builder.HasIdConversion<TId, Cuid, CuidIdConverter<TId>>(new CuidIdConverter<TId>());

    public static PropertyBuilder<TId> HasIdConversion<TId, TValue, TConverter>(this PropertyBuilder<TId> builder, TConverter converter)
        where TId : IdentifierObject<TId, TValue>
        where TValue : notnull
        where TConverter : ObjectConverter<TId, TValue> =>
        builder.HasConversion(new IdValueConverter<TId, TValue, TConverter>(converter));
}
