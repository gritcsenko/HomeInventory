using AutoMapper;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Web.Mapping;

internal class ContractsMappings : Profile
{
    public ContractsMappings()
    {
        CreateMap<UserId, Guid>().ConstructUsing(x => x.Id);
        CreateMap<string, Email>().ConvertUsing<EmailConverter>();

        CreateMap<RegisterRequest, RegisterCommand>();
        CreateMap<RegistrationResult, RegisterResponse>();

        CreateMap<LoginRequest, AuthenticateQuery>();
        CreateMap<AuthenticateResult, LoginResponse>();
    }
}

internal class EmailConverter : IValueConverter<string, Email>, ITypeConverter<string, Email>
{
    private readonly IEmailFactory _factory;

    public EmailConverter(IEmailFactory factory) => _factory = factory;

    public Email Convert(string sourceMember, ResolutionContext context) => _factory.CreateFrom(sourceMember).Value;

    public Email Convert(string source, Email destination, ResolutionContext context) => _factory.CreateFrom(source).Value;
}
