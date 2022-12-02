using AutoMapper;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Mapping;
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
