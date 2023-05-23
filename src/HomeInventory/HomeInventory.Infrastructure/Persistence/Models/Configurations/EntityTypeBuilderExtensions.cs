using DotNext;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal static class EntityTypeBuilderExtensions
{
    public static PropertyBuilder<TId> HasIdConversion<TId>(this PropertyBuilder<TId> builder)
        where TId : notnull, GuidIdentifierObject<TId>, IBuildable<TId, GuidIdentifierObject<TId>.Builder>
    {
        return builder.HasConversion(new GuidIdValueConverter<TId>());
    }
}
