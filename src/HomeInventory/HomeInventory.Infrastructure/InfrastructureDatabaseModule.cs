using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Modules.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public sealed class InfrastructureDatabaseModule : BaseModule
{
    public override void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventsPersistenceService, EventsPersistenceService>();
        services.AddScoped<IDatabaseConfigurationApplier, OutboxDatabaseConfigurationApplier>();
        services.AddScoped<PolymorphicDomainEventTypeResolver>();

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddDbContext<DatabaseContext>((sp, builder) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                builder.UseInMemoryDatabase("HomeInventory");
                builder.EnableDetailedErrors(!env.IsProduction());
                builder.EnableSensitiveDataLogging(!env.IsProduction());
            });
        services.AddScoped<IDatabaseContext>(sp => sp.GetRequiredService<DatabaseContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatabaseContext>());

    }
}
