using AutoMapper;

namespace HomeInventory.Web.Framework;

public static class MappingExtensions
{
    public static TDestination MapOrFail<TDestination>(this IMapperBase mapper, object? source) =>
        mapper.Map<TDestination>(source)
            ?? throw new InvalidOperationException($"Failed to map {source ?? "<null>"} of type {source?.GetType() ?? typeof(object)} to {typeof(TDestination)}");
}
