using System.Linq.Expressions;
using AutoMapper;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public class MappingProfile : Profile
{
    protected void CreateMapForId<TId>()
        where TId : GuidIdentifierObject<TId>
    {
        CreateMapForValue<TId, Guid, GuidIdConverter<TId>>(x => x.Id);
    }

    protected void CreateMapForValue<TObject, TValue>(Expression<Func<TObject, TValue>> convertToValue)
        where TObject : ValueObject<TObject>
    {
        CreateMapForValue<TObject, TValue, ValueObjectConverter<TObject, TValue>>(convertToValue);
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
