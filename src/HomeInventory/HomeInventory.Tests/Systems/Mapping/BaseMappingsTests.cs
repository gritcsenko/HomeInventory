using AutoMapper;
using HomeInventory.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Systems.Mapping;

public abstract class BaseMappingsTests : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();
    private readonly IServiceProviderFactory<IServiceCollection> _factory = new DefaultServiceProviderFactory();

    protected BaseMappingsTests()
    {
    }

    public IServiceCollection Services => _services;

    protected IMapper CreateSut<TMapper>()
        where TMapper : Profile, new()
    {
        var config = new MapperConfiguration(x =>
        {
            x.AddProfile<TMapper>();
        });
        var serviceProvider = _factory.CreateServiceProvider(_services);
        return new Mapper(config, serviceProvider.GetService);
    }
}
