using AutoMapper;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.UserManagement.Mapping;
using HomeInventory.Web.UserManagement;
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
