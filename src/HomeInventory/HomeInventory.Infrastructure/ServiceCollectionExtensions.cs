using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Mapping;
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
        services.AddRepository<User, IUserRepository, UserRepository>();
        services.AddRepository<StorageArea, IStorageAreaRepository, StorageAreaRepository>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);

        services.AddSingleton<AmountObjectConverter>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.AddHealthChecks()
             .AddCheck<PersistenceHealthCheck>("Persistence", HealthStatus.Unhealthy, new[] { HealthCheckTags.Ready });
        return services;
    }

    private static IServiceCollection AddRepository<TEntity, TRepository, TRepositoryImplementation>(this IServiceCollection services)
        where TEntity : class, Domain.Primitives.IEntity<TEntity>
        where TRepository : class, IRepository<TEntity>
        where TRepositoryImplementation : class, TRepository =>
        services
            .AddScoped<TRepository, TRepositoryImplementation>()
            .AddScoped<IRepository<TEntity>>(sp => sp.GetRequiredService<TRepository>());

    private static IServiceCollection AddDatabase(this IServiceCollection services) =>
        services
            .AddScoped<PublishDomainEventsInterceptor>()
            .AddDbContext<DatabaseContext>((sp, builder) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                builder.UseInMemoryDatabase("HomeInventory").UseApplicationServiceProvider(sp);
                builder.EnableDetailedErrors(!env.IsProduction());
                builder.EnableSensitiveDataLogging(!env.IsProduction());
            })
            .AddScoped<IDatabaseContext>(sp => sp.GetRequiredService<DatabaseContext>())
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DatabaseContext>());
}
