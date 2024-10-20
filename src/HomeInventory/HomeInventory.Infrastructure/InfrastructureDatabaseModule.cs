using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Modules.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Infrastructure;

public sealed class InfrastructureDatabaseModule : BaseModule
{
    public override async Task AddServicesAsync(ModuleServicesContext context)
    {
        await base.AddServicesAsync(context);

        context.Services
            .AddScoped<IEventsPersistenceService, EventsPersistenceService>()
            .AddScoped<IDatabaseConfigurationApplier, OutboxDatabaseConfigurationApplier>()
            .AddScoped<PolymorphicDomainEventTypeResolver>()
            .AddScoped<PublishDomainEventsInterceptor>()
            .AddDbContext<DatabaseContext>((sp, builder) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                builder.UseInMemoryDatabase("HomeInventory");
                builder.EnableDetailedErrors(!env.IsProduction());
                builder.EnableSensitiveDataLogging(!env.IsProduction());
            })
            .AddScoped<IDatabaseContext>(sp => sp.GetRequiredService<DatabaseContext>())
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatabaseContext>());
    }
}
