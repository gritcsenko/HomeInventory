using AutoMapper;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Systems.Persistence;

public abstract class BaseRepositoryTest : BaseDatabaseContextTest
{
    protected BaseRepositoryTest()
    {
        var services = new ServiceCollection();
        var factory = new DefaultServiceProviderFactory();

        var config = new MapperConfiguration(x =>
        {
            x.AddProfile<ModelMappings>();
            x.AddProfile<UserManagementModelMappings>();
            x.AddProfile<UserManagementContractsMappings>();
        });
        var serviceProvider = factory.CreateServiceProvider(services);
        Mapper = new Mapper(config, serviceProvider.GetService);
        PersistenceService = Substitute.For<IEventsPersistenceService>();
    }

    protected IMapper Mapper { get; }

    protected IEventsPersistenceService PersistenceService { get; }
}
