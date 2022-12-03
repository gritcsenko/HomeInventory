using System.Linq.Expressions;
using AutoMapper;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class ModelMappings : Profile
{
    public ModelMappings()
    {
        CreateMapForId<UserId>();
        CreateMapForId<StorageAreaId>();

        CreateMapForValue<Email, string, EmailConverter>(x => x.Value);

        CreateMap<User, UserModel>();
        CreateMap<UserModel, User>();

        CreateMap<StorageArea, StorageAreaModel>();
        CreateMap<StorageAreaModel, StorageArea>();
    }

    private void CreateMapForId<TId>()
        where TId : GuidIdentifierObject<TId>
    {
        CreateMapForValue<TId, Guid, GuidIdConverter<TId>>(x => x.Id);
    }

    private void CreateMapForValue<TObject, TValue, TConverter>(Expression<Func<TObject, TValue>> convertToValue)
        where TObject : ValueObject<TObject>
        where TConverter : ValueObjectConverter<TObject, TValue>
    {
        CreateMap<TObject, TValue>()
            .ConstructUsing(convertToValue);
        CreateMap<TValue, TObject>()
            .ConvertUsing<TConverter>();
    }
}
