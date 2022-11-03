using AutoMapper;
using HomeInventory.Domain.Entities;
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

internal class EmailConverter : IValueConverter<string, Email>, ITypeConverter<string, Email>
{
    private readonly IEmailFactory _factory;

    public EmailConverter(IEmailFactory factory) => _factory = factory;

    public Email Convert(string sourceMember, ResolutionContext context) => _factory.CreateFrom(sourceMember).Value;

    public Email Convert(string source, Email destination, ResolutionContext context) => _factory.CreateFrom(source).Value;
}

internal class UserIdConverter : IValueConverter<Guid, UserId>, ITypeConverter<Guid, UserId>
{
    private readonly IIdFactory<UserId, Guid> _factory;

    public UserIdConverter(IIdFactory<UserId, Guid> factory) => _factory = factory;

    public UserId Convert(Guid sourceMember, ResolutionContext context) => _factory.CreateFrom(sourceMember).Value;

    public UserId Convert(Guid source, UserId destination, ResolutionContext context) => _factory.CreateFrom(source).Value;
}
