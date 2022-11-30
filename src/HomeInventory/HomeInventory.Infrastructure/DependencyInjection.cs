using HomeInventory.Application;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Services;
using HomeInventory.Infrastructure.Specifications;
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
        services.TryAddSingleton<ISpecificationEvaluator, SpecificationEvaluator>();
        services.AddRepository<User, IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddMappingAssemblySource(AssemblyReference.Assembly);
        return services;
    }

    private static IServiceCollection AddRepository<TEntity, TRepository, TRepositoryImplementation>(this IServiceCollection services)
        where TEntity : class, IEntity<TEntity>
        where TRepository : class, IRepository<TEntity>
        where TRepositoryImplementation : class, TRepository
    {
        return services.AddScoped<TRepository, TRepositoryImplementation>()
            .AddScoped<IRepository<TEntity>>(sp => sp.GetRequiredService<TRepository>());
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
