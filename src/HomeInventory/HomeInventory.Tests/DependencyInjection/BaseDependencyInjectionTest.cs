using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace HomeInventory.Tests.DependencyInjection;

public abstract class BaseDependencyInjectionTest : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();

    protected ServiceProviderOptions DefaultOptions { get; } = new();

    protected IServiceCollection Services => _services;

    protected ServiceProvider CreateProvider(ServiceProviderOptions? options = null) => _services.BuildServiceProvider(options ?? DefaultOptions);

    protected void AddConfiguration(IEnumerable<KeyValuePair<string, string?>> inlineData)
    {
        var source = new MemoryConfigurationSource
        {
            InitialData = inlineData,
        };
        var providers = new IConfigurationProvider[]
        {
            new MemoryConfigurationProvider(source)
        };
#pragma warning disable CA2000 // Dispose objects before losing scope
        Services.AddSingleton<IConfiguration>(new ConfigurationRoot(providers));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }

    protected void AddDateTime() => Services.AddScoped(_ => DateTime);
}
