using HomeInventory.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Infrastructure.Framework;

public static class InfrastructureFrameworkServiceCollectionExtensions
{
    public static IServiceCollection AddRepository<TEntity, TRepository, TRepositoryImplementation>(this IServiceCollection services)
        where TEntity : class, IEntity<TEntity>
        where TRepository : class, IRepository<TEntity>
        where TRepositoryImplementation : class, TRepository =>
        services
            .AddScoped<TRepository, TRepositoryImplementation>()
            .AddScoped<IRepository<TEntity>>(sp => sp.GetRequiredService<TRepository>())
            .AddScoped<IReadOnlyRepository<TEntity>>(sp => sp.GetRequiredService<IRepository<TEntity>>());
}
