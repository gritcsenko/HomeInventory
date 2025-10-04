using System.Diagnostics.CodeAnalysis;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Framework.Models.Configuration;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;

namespace HomeInventory.Tests.Modules;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class InfrastructureDatabaseModuleTests() : BaseModuleTest<InfrastructureDatabaseModule>(static () => new())
{
    protected override void EnsureRegistered(IServiceCollection services) =>
        services.Should().ContainSingleScoped<IEventsPersistenceService>()
            .And.ContainSingleScoped<IDatabaseConfigurationApplier>()
            .And.ContainSingleScoped<IDatabaseContext>()
            .And.ContainSingleScoped<IUnitOfWork>()
            .And.ContainSingleScoped<PolymorphicDomainEventTypeResolver>()
            .And.ContainSingleScoped<PublishDomainEventsInterceptor>()
            .And.ContainSingleScoped<DatabaseContext>();
}
