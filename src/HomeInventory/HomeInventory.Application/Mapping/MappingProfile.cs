using System.Linq.Expressions;
using AutoMapper;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public abstract class MappingProfile : Profile
{
    protected MappingProfile()
    {
    }

    protected void CreateMapForId<TId>()
        where TId : class, IGuidIdentifierObject<TId>
    {
        var converter = new GuidIdConverter<TId>();
        CreateMap<TId, Guid>()
            .ConstructUsing(x => x.Id);
        CreateMap<Guid, TId>()
            .ConstructUsing(id => converter.Convert(id));
    }

    protected void CreateMapForString<TObject>(Expression<Func<string, TObject>> convertFromValue, Expression<Func<TObject, string>> convertToValue)
        where TObject : notnull
    {
        CreateMap<TObject, string>()
            .ConstructUsing(convertToValue);
        CreateMap<string, TObject>()
            .ConstructUsing(convertFromValue);
    }

    protected void CreateMapForValue<TObject, TValue, TConverter>(Expression<Func<TObject, TValue>> convertToValue)
        where TObject : ValueObject<TObject>
        where TConverter : GenericValueObjectConverter<TObject, TValue>
    {
        CreateMap<TObject, TValue>()
            .ConstructUsing(convertToValue);
        CreateMap<TValue, TObject>()
            .ConvertUsing<TConverter>();
    }
}
