using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace HomeInventory.Tests.DependencyInjection;

public abstract class BaseDependencyInjectionTest : BaseTest
{
    private readonly IServiceCollection _services = new ServiceCollection();

    protected ServiceProviderOptions DefaultOptions { get; } = new();

    protected IServiceCollection Services => _services;

    protected ServiceProvider CreateProvider(ServiceProviderOptions? options = null) => _services.BuildServiceProvider(options ?? DefaultOptions);

    protected IConfigurationManager AddConfiguration(IEnumerable<KeyValuePair<string, string?>> inlineData)
    {
        var source = new MemoryConfigurationSource
        {
            InitialData = inlineData,
        };

#pragma warning disable CA2000 // Dispose objects before losing scope
        var manager = new ConfigurationManager();
#pragma warning restore CA2000 // Dispose objects before losing scope
        manager.Sources.Add(source);

        Services.AddSingleton<IConfiguration>(_ => ((IConfigurationBuilder)manager).Build());

        return manager;
    }

    protected void AddDateTime() => Services.AddScoped(_ => DateTime);
}
