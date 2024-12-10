using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace HomeInventory.Tests.DependencyInjection;

public abstract class BaseDependencyInjectionTest : BaseTest
{
    private readonly ServiceProviderOptions _defaultOptions= new();

    protected IServiceCollection Services { get; } = new ServiceCollection();

    protected ServiceProvider CreateProvider(ServiceProviderOptions? options = null) => Services.BuildServiceProvider(options ?? _defaultOptions);

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
