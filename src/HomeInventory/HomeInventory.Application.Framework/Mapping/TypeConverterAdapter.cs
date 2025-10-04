using AutoMapper;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Framework.Mapping;

internal sealed class TypeConverterAdapter<TSource, TDestination, TConverter>(TConverter converter) : ITypeConverter<TSource, TDestination>
    where TConverter : ObjectConverter<TSource, TDestination>
{
    private readonly ObjectConverter<TSource, TDestination> _converter = converter;

    TDestination ITypeConverter<TSource, TDestination>.Convert(TSource source, TDestination destination, ResolutionContext context) => _converter.Convert(source);
}
