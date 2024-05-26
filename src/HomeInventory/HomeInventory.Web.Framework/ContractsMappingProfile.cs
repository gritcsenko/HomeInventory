using System.Linq.Expressions;
using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using Visus.Cuid;

namespace HomeInventory.Web.Framework;

public abstract class ContractsMappingProfile : Profile
{
    protected ContractsMappingProfile()
    {
    }

    protected void CreateMapForId<TId>()
        where TId : class, ICuidBuildable<TId>, ICuidIdentifierObject<TId>
    {
        var converter = new CuidIdConverter<TId>();
        CreateMap<TId, Cuid>()
            .ConvertUsing(x => x.Value);
        CreateMap<Cuid, TId>()
            .ConvertUsing(id => converter.Convert(id));
    }

    protected void CreateMapForString<TObject>(Expression<Func<string, TObject>> convertFromValue, Expression<Func<TObject, string>> convertToValue)
        where TObject : class, IValueObject<TObject>
    {
        CreateMap<TObject, string>()
            .ConvertUsing(convertToValue);
        CreateMap<string, TObject>()
            .ConvertUsing(convertFromValue);
    }
}
