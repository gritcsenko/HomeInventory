using AutoMapper;

namespace HomeInventory.Tests.Systems.Mapping;

public abstract class BaseMappingsTests : BaseTest
{
    private readonly ServiceCollection _services = new();
    private readonly DefaultServiceProviderFactory _factory = new();
    private readonly Lazy<IServiceProvider> _lazyServiceProvider;

    protected BaseMappingsTests()
    {
        _services
            .AddDomain()
            .AddMessageHubCore()
            .AddMappingTypeConverter();
        _lazyServiceProvider = new Lazy<IServiceProvider>(() => _factory.CreateServiceProvider(_services));
    }

    public IServiceCollection Services => _services;

    public IServiceProvider ServiceProvider => _lazyServiceProvider.Value;

    protected IMapper CreateSut<TMapper>()
        where TMapper : Profile, new()
    {
        var config = new MapperConfiguration(x =>
        {
            x.AddProfile<TMapper>();
        });
        return new Mapper(config, ServiceProvider.GetService);
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
        return new Mapper(config, ServiceProvider.GetService);
    }
}
