using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Mapping;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();
        services.TryAddSingleton<IDateTimeService, SystemDateTimeService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);

        services.AddSingleton<ValueObjectConverter<Email, string>>();
        services.AddSingleton<GuidIdConverter<UserId>>();
        services.AddSingleton<GuidIdConverter<ProductId>>();
        services.AddSingleton<AmountValueObjectConverter>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services)
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
