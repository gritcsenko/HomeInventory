using AutoMapper;

namespace HomeInventory.Infrastructure.Framework.Mapping;

public static class MappingExtensions
{
    public static TDestination MapOrThrow<TSource, TDestination>(this IMapperBase mapper, TSource source) =>
        mapper.Map<TDestination>(source)
            ?? throw new InvalidOperationException($"Failed to map {source?.ToString() ?? "<null>"} of type {source?.GetType() ?? typeof(TSource)} to {typeof(TDestination)}");
}
