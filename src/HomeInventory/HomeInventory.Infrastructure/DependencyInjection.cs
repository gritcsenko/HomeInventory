using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Authentication;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuth();
        services.AddDatbase();
        services.TryAddSingleton<IDateTimeService, SystemDateTimeService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddMappingSourceFromCurrentAssembly();
        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionsSetup>();

        services.AddSingleton<IJwtIdentityGenerator, GuidJwtIdentityGenerator>();
        services.AddSingleton<IAuthenticationTokenGenerator, JwtTokenGenerator>();

        services.AddAuthorization(); // Read https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-6.0

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { });
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        return services;
    }

    private static IServiceCollection AddDatbase(this IServiceCollection services)
    {
        return services.AddDbContext<IDatabaseContext, DatabaseContext>((sp, builder) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();
            builder.UseInMemoryDatabase("HomeInventory").UseApplicationServiceProvider(sp);
            builder.EnableDetailedErrors(!env.IsProduction());
            builder.EnableSensitiveDataLogging(!env.IsProduction());
        });
    }
}
