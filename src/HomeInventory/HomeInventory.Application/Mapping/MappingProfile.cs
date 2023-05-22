using System.Linq.Expressions;
using AutoMapper;
using DotNext;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public abstract class MappingProfile : Profile
{
    protected MappingProfile()
    {
        CreateMap<RegisterCommand, FilterSpecification<User>>()
            .ConstructUsing(c => UserSpecifications.HasEmail(c.Email));
        CreateMap<AuthenticateQuery, FilterSpecification<User>>()
            .ConstructUsing(c => UserSpecifications.HasEmail(c.Email));
        CreateMap<RegisterCommand, CreateUserSpecification>()
            .ConstructUsing(c => new CreateUserSpecification(c.Email, c.Password, new DelegatingSupplier<Guid>(Guid.NewGuid)));
    }

    protected void CreateMapForId<TId>()
        where TId : notnull, GuidIdentifierObject<TId>, IBuildable<TId, GuidIdentifierObject<TId>.Builder>
    {
        CreateMap<TId, Guid>()
            .ConstructUsing(x => x.Id);
        CreateMap<Guid, TId>()
            .ConvertUsing(new GuidIdConverter<TId>());
    }

    protected void CreateMapForString<TObject>(Expression<Func<TObject, string>> convertToValue)
        where TObject : notnull, ValueObject<TObject>, IBuildable<TObject, ValueObject<TObject>.Builder<string>>
    {
        CreateMap<TObject, string>()
            .ConstructUsing(convertToValue);
        CreateMap<string, TObject>()
            .ConvertUsing(new StringObjectConverter<TObject>());
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
