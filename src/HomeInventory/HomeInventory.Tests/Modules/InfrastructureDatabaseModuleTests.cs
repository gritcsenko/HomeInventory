using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Modules;

public class InfrastructureDatabaseModuleTests() : BaseModuleTest<InfrastructureDatabaseModule>(() => new())
{
    protected override void EnsureRegistered(IServiceCollection services)
    {
        services.Should().ContainSingleScoped<IEventsPersistenceService>()
            .And.ContainSingleScoped<IDatabaseConfigurationApplier>()
            .And.ContainSingleScoped<IDatabaseContext>()
            .And.ContainSingleScoped<IUnitOfWork>()
            .And.ContainSingleScoped<PolymorphicDomainEventTypeResolver>()
            .And.ContainSingleScoped<PublishDomainEventsInterceptor>()
            .And.ContainSingleScoped<DatabaseContext>()
            .And.ContainSingleSingleton<IDbContextFactory<DatabaseContext>>();
    }
}
