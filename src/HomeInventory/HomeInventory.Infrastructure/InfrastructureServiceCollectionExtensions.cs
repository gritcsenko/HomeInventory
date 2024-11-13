using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Application;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();
        services.TryAddSingleton<ISpecificationEvaluator>(SpecificationEvaluator.Default);
        services.AddMappingAssemblySource(HomeInventory.Infrastructure.AssemblyReference.Assembly);

        services.AddSingleton<AmountObjectConverter>();
        services.AddScoped<IEventsPersistenceService, EventsPersistenceService>();
        services.AddScoped<IDatabaseConfigurationApplier, OutboxDatabaseConfigurationApplier>();
        services.AddScoped<PolymorphicDomainEventTypeResolver>();

        services.AddHealthChecks()
             .AddCheck<PersistenceHealthCheck>("Persistence", HealthStatus.Unhealthy, [HealthCheckTags.Ready]);
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services) =>
        services
            .AddScoped<PublishDomainEventsInterceptor>()
            .AddDbContext<DatabaseContext>(static (sp, builder) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                builder.UseInMemoryDatabase("HomeInventory");
                builder.EnableDetailedErrors(!env.IsProduction());
                builder.EnableSensitiveDataLogging(!env.IsProduction());
            })
            .AddScoped<IDatabaseContext>(static sp => sp.GetRequiredService<DatabaseContext>())
            .AddScoped<IUnitOfWork>(static sp => sp.GetRequiredService<DatabaseContext>());
}
