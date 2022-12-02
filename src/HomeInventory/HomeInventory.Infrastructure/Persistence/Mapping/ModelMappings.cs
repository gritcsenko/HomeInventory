using AutoMapper;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal class ModelMappings : Profile
{
    public ModelMappings()
    {
        CreateMap<UserId, Guid>()
            .ConstructUsing(id => id.Id);
        CreateMap<Guid, UserId>()
            .ConvertUsing<UserIdConverter>();

        CreateMap<Email, string>()
            .ConstructUsing(email => email.Value);
        CreateMap<string, Email>()
            .ConvertUsing<EmailConverter>();

        CreateMap<User, UserModel>();
        CreateMap<UserModel, User>();
    }
}
