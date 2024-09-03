using AutoMapper;

namespace HomeInventory.Tests.Systems.Mapping;

public abstract class BaseMappingsTests : BaseTest
{
    private readonly ServiceCollection _services = new();
    private readonly DefaultServiceProviderFactory _factory = new();

    protected BaseMappingsTests()
    {
        Services.AddMappingAssemblySource();
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

    protected IMapper CreateSut<TMapper1, TMapper2>()
        where TMapper1 : Profile, new()
        where TMapper2 : Profile, new()
    {
        var config = new MapperConfiguration(x =>
        {
            x.AddProfile<TMapper1>();
            x.AddProfile<TMapper2>();
        });
        var serviceProvider = _factory.CreateServiceProvider(_services);
        return new Mapper(config, serviceProvider.GetService);
    }
}
