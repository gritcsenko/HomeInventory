using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.AddSingleton<IAuthenticationTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeService, SystemDateTimeService>();
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
