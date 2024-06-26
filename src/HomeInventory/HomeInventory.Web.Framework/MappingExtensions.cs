using AutoMapper;

namespace HomeInventory.Web.Framework;

public static class MappingExtensions
{
    public static TDestination MapOrFail<TDestination>(this IMapperBase mapper, object source) =>
        mapper.Map<TDestination>(source)
            ?? throw new InvalidOperationException($"Failed to map {source ?? "<null>"} of type {source?.GetType() ?? typeof(object)} to {typeof(TDestination)}");

    public static TDestination MapOrFail<TDestination>(this IMapper mapper, object source, Action<IMappingOperationOptions<object, TDestination>> opts) =>
        mapper.Map(source, opts)
            ?? throw new InvalidOperationException($"Failed to map {source ?? "<null>"} of type {source?.GetType() ?? typeof(object)} to {typeof(TDestination)}");
}
