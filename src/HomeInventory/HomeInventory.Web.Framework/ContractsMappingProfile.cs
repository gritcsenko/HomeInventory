using System.Linq.Expressions;
using AutoMapper;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Web.Framework;

public abstract class ContractsMappingProfile : Profile
{
    protected ContractsMappingProfile()
    {
    }

    protected void CreateMapForId<TId>()
        where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>
    {
        var converter = new UlidIdConverter<TId>();
        CreateMap<TId, Ulid>()
            .ConvertUsing(x => x.Value);
        CreateMap<Ulid, TId>()
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
