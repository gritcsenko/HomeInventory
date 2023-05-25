using System.Linq.Expressions;
using AutoMapper;
using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public abstract class MappingProfile : Profile
{
    protected MappingProfile()
    {
    }

    protected void CreateMapForId<TId>()
        where TId : notnull, GuidIdentifierObject<TId>, IBuildable<TId, GuidIdentifierObject<TId>.Builder>
    {
        CreateMap<TId, Guid>()
            .ConstructUsing(x => x.Id);
        CreateMap<Guid, TId>()
            .ConvertUsing(new GuidIdConverter<TId>());
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
