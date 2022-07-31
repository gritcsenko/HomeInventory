using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<JwtSettings>().FromConfiguration();
        services.AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>();
        services.AddSingleton<IAuthenticationTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeService, SystemDateTimeService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddMappingSourceFromCurrentAssembly();
        return services;
    }
}
