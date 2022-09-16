using HomeInventory.Tests.Helpers;
using Mapster;
using MapsterMapper;
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
        where TMapper : IRegister, new()
    {
        var config = new TypeAdapterConfig();
        config.Apply(new TMapper());
        return new ServiceMapper(_factory.CreateServiceProvider(_services), config);
    }
}
