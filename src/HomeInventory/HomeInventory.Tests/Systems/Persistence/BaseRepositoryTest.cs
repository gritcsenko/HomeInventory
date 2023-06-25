using AutoMapper;
using HomeInventory.Infrastructure.Persistence.Mapping;
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
        });
        var serviceProvider = factory.CreateServiceProvider(services);
        Mapper = new Mapper(config, serviceProvider.GetService);
    }

    protected IMapper Mapper { get; }
}
