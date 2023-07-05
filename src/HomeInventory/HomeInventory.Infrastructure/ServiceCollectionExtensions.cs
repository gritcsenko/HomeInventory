using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Application;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using HomeInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();
        services.TryAddSingleton<ISpecificationEvaluator>(SpecificationEvaluator.Default);
        services.AddRepository<StorageArea, IStorageAreaRepository, StorageAreaRepository>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);

        services.AddSingleton<AmountObjectConverter>();
        services.AddScoped<IEventsPersistenceService, EventsPersistenceService>();
        services.AddScoped<IDatabaseConfigurationApplier, OutboxDatabaseConfigurationApplier>();
        services.AddScoped<PolymorphicDomainEventTypeResolver>();

        services.AddHealthChecks()
             .AddCheck<PersistenceHealthCheck>("Persistence", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services) =>
        services
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
